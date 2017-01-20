using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires
{
    partial class Grid : GameObjects.GameObjectGrid
    {

        // A point that radiates (sometimes negative) value to the surrounding tiles. used to create heightmap, and humiditymap.
        private struct Heightpoint
        {
            public Vector2 position;
            public double height;
            public double width;
        }

        // Specifications of one kind of terrain
        private struct TerrainToComposition
        {
            public double coldChance;
            public double hotChance;
            public double humidChance;
            public double mountainchance;
            public double hillschance;
            public double lakechance;
        }

        // A combination of the coordinates of a tile and an integer value (used for height, humidity and temperature). usefull for sorting.
        private struct TileWithHeight
        {
            public TileWithHeight(double height, int x, int y)
            {
                this.height = height;
                this.x = x;
                this.y = y;
            }

            public double height;
            public int x;
            public int y;
        }

        private Heightpoint[] lakeArray;
        private Heightpoint[] mountainArray;
        private double[,] heightMap;
        private Terrain attackBonusTerrain;
        private Terrain defenseBonusTerrain;

        // TODO create this! camera needed for debugging. double mirrors the grid to create a symmetrical field (with double width and height).
        public void BalancedEconomyGrid()
        {

        }

        // Converts min and max percentage into an actual amount
        private int GetAmount(Configuration file, string min, string max)
        {
            int minPercentMountains = file.GetProperty<int>(min);
            int maxPercentMountains = file.GetProperty<int>(max);
            double percentMountainsOnMap = (minPercentMountains + MaxOfEmpires.Random.NextDouble() * (maxPercentMountains - minPercentMountains));
            return (int)((percentMountainsOnMap / 100) * (Width * Height));
        }

        // Collects terrain specifications from files
        private TerrainToComposition GetTerrainSpecsFromFile(Terrain terrain, bool hills)
        {
            string terrainString = "terrain/" + terrain.terrainType.ToString().ToLower() + "Composition";
            if (hills)
                terrainString += "WH";
            TerrainToComposition Composition;
            Configuration file = FileManager.LoadConfig(terrainString);
            Composition.coldChance = file.GetProperty<int>("cold");
            Composition.hotChance = file.GetProperty<int>("hot");
            Composition.humidChance = file.GetProperty<int>("humid");
            Composition.mountainchance = file.GetProperty<int>("mountain");
            Composition.hillschance = file.GetProperty<int>("hills");
            Composition.lakechance = file.GetProperty<int>("lake");
            return Composition;
        }

        // Generates a battlemap: a transition between two economygrid tiles
        public void BattleGenerate(Terrain attackTerrain, bool attackHills, Terrain defendTerrain, bool defendHills)
        {
            defenseBonusTerrain = defendTerrain;
            attackBonusTerrain = attackTerrain;
            Configuration file = FileManager.LoadConfig("TerrainGeneration");
            TerrainToComposition attackComposition = GetTerrainSpecsFromFile(attackTerrain,attackHills);
            TerrainToComposition defendComposition = GetTerrainSpecsFromFile(defendTerrain,defendHills);
            GenerateEnvironmentalValues(file, attackComposition, defendComposition);
        }

        // Generates terain using 3 layers of values: height, humidity, and temperature
        private void GenerateEnvironmentalValues(Configuration file,TerrainToComposition attackComposition, TerrainToComposition defendComposition)
        {
            double[] weighValue = new double[Height];
            int defendingpercentage = file.GetProperty<int>("PercentageDefendingTile");
            int defendingRows = (int)((defendingpercentage / 100.0)*Height);
            for(int i = 0; i < defendingRows; i++)
            {
                weighValue[i] = 1.0; // The upper rows are determined only by the defending tile.
            }
            for (int i = defendingRows; i < Height; i++)
            {
                weighValue[i] = 1-((i - defendingRows)/ (double)(Height - 1 - defendingRows)); // Generates a weighvalue for all transitionrows
            }
            bool validHeightMap = false;
            while (!validHeightMap)
            {
                validHeightMap = GeneratePerlinHeights(file, attackComposition, defendComposition, weighValue); // If not evertything is accessible, generates new heightmap
            }
            GeneratePerlinHumidity(file, attackComposition, defendComposition, weighValue);
            GeneratePerlinTemperature(file, attackComposition, defendComposition, weighValue);

        }

        // Distributes height using perlin noise
        private bool GeneratePerlinHeights(Configuration file,TerrainToComposition attackComposition, TerrainToComposition defendComposition, double[] weighValue)
        {
            int structureSize = file.GetProperty<int>("structureSize");
            double structureSizeApplicable = structureSize / 100.0; // Determines randomness for PerlinGrid
            double[,] heightMap = GeneratePerlinGrid(Width, Height, structureSizeApplicable);
            for(int x = 0; x < Width; x++)
            {
                for(int y = 0; y < Height; y++)
                {
                    Tile currentTile = this[x,y] as Tile;
                    double height = heightMap[x, y];
                    double defenseValue = weighValue[y];
                    double attackValue = 1 - defenseValue;
                    double mountainChance = defenseValue * defendComposition.mountainchance + attackValue * attackComposition.mountainchance;
                    double mountainThreshold = ChanceToThreshold(true, mountainChance);

                    double hillsChance = defenseValue * defendComposition.hillschance + attackValue * attackComposition.hillschance;
                    double hillsThreshold = ChanceToThreshold(true, hillsChance);

                    double lakeChance = defenseValue * defendComposition.lakechance + attackValue * attackComposition.lakechance;
                    double lakeThreshold = ChanceToThreshold(false, lakeChance);

                    if (height > mountainThreshold)
                    {
                        currentTile.Terrain = Terrain.Mountain;
                        currentTile.hills = false;
                    }
                    else if(height > hillsThreshold)
                    {
                        currentTile.Terrain = Terrain.Plains;
                        currentTile.hills = true;
                    }
                    else if(height < lakeThreshold)
                    {
                        currentTile.Terrain = Terrain.Lake;
                        currentTile.hills = false;
                    }
                    else
                    {
                        currentTile.Terrain = Terrain.Plains;
                        currentTile.hills = false;
                    }
                }
            }
            return CheckIfEveryTileIsAccessible(file);
        }

        // Distributes humidity using perlin noise
        private void GeneratePerlinHumidity(Configuration file, TerrainToComposition attackComposition, TerrainToComposition defendComposition, double[] weighValue)
        {
            int structureSize = file.GetProperty<int>("structureSize");
            double structureSizeApplicable = structureSize / 100.0;
            double[,] humidityMap = GeneratePerlinGrid(Width, Height, structureSizeApplicable);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile currentTile = this[x, y] as Tile;
                    double humidity = humidityMap[x, y];
                    double defenseValue = weighValue[y];
                    double attackValue = 1 - defenseValue;
                    double humidChance = defenseValue * defendComposition.humidChance + attackValue * attackComposition.humidChance;
                    double humidThreshold = ChanceToThreshold(true, humidChance);

                    if (humidity > humidThreshold)
                    {
                        if(currentTile.Terrain == Terrain.Plains)
                        {
                            currentTile.Terrain = Terrain.Forest;
                        }
                    }
                }
            }
        }

        // Distributes temperature using perlin noise
        private void GeneratePerlinTemperature(Configuration file, TerrainToComposition attackComposition, TerrainToComposition defendComposition, double[] weighValue)
        {
            int structureSize = file.GetProperty<int>("structureSize");
            double structureSizeApplicable = structureSize / 100.0;
            double[,] temperatureMap = GeneratePerlinGrid(Width, Height, structureSizeApplicable);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile currentTile = this[x, y] as Tile;
                    double temperature = temperatureMap[x, y];
                    double defenseValue = weighValue[y];
                    double attackValue = 1 - defenseValue;
                    double hotChance = defenseValue * defendComposition.hotChance + attackValue * attackComposition.hotChance;
                    double hotThreshold = ChanceToThreshold(true, hotChance);

                    double coldChance = defenseValue * defendComposition.coldChance + attackValue * attackComposition.coldChance;
                    double coldThreshold = ChanceToThreshold(false, coldChance);

                    if (temperature > hotThreshold)
                    {
                        if (currentTile.Terrain == Terrain.Plains)
                        {
                            currentTile.Terrain = Terrain.Desert;
                        }
                        else if(currentTile.Terrain == Terrain.Forest)
                        {
                            currentTile.Terrain = Terrain.Jungle;
                        }
                        else if (currentTile.Terrain == Terrain.Mountain)
                        {
                            currentTile.Terrain = Terrain.DesertMountain;
                        }
                    }
                    else if (temperature < coldThreshold)
                    {
                        if (currentTile.Terrain == Terrain.Plains)
                        {
                            currentTile.Terrain = Terrain.Tundra;
                        }
                        else if (currentTile.Terrain == Terrain.Forest)
                        {
                            currentTile.Terrain = Terrain.Swamp;
                        }
                        else if (currentTile.Terrain == Terrain.Mountain)
                        {
                            currentTile.Terrain = Terrain.TundraMountain;
                        }
                    }
                }
            }
        }

        // Converts spawnchance to required threshold for PerlinNoiseGrid
        private double ChanceToThreshold(bool high, double chance)
        {
            if (!high)
            {
                return chance / 50 - 1;      //1 - (1 - chance/100.0) * 2 
            }
            else
            {
                return 1 - chance / 50;
            }
        }

        // Generates an economy map using terrainGeneration.cfg
        public void EconomyGenerate()
        {
            Configuration file = FileManager.LoadConfig("TerrainGeneration");

            mountainArray = ResetMountains(file);
            lakeArray = ResetLakes(file);
            File.CreateText("generationDump.txt").Close();
            bool terrainFinished = false;
            while (!terrainFinished)
            {
                terrainFinished = GenerateTerrain(file);
            }
            Heightpoint[] humidity = SetHumidityPoints(file,mountainArray, lakeArray);
            GenerateTemperature(file, Width, Height, 0.1);
        }

        // Creates a double grid with Height values based on heightpoints in mountainArran and lakeArray
        private double[,] GenerateHeightmap(Configuration file)
        {
            heightMap = new double[Width, Height];
            // Lakes = hills = mountains = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int i = 0; i < mountainArray.Length; i++)
                    {
                        Vector2 position = new Vector2(x, y);
                        double distance = Math.Abs((mountainArray[i].position - position).Length());

                        // Punttopformule van Luuk
                        double mountainNoise = file.GetProperty<int>("mountainNoise") / (double)100;
                        heightMap[x, y] += ((1 - mountainNoise) + MaxOfEmpires.Random.NextDouble() * mountainNoise * 2) * (mountainArray[i].height - mountainArray[i].height * Math.Abs(Math.Sin(5 / (mountainArray[i].width * Math.PI) * distance)));
                    }
                    for (int i = 0; i < lakeArray.Length; i++)
                    {
                        Vector2 position = new Vector2(x, y);
                        double distance = Math.Abs((lakeArray[i].position - position).Length());
                        if (distance < lakeArray[i].width)
                            // Exponentiele formule van Luuk
                            heightMap[x, y] += lakeArray[i].height * Math.Pow(15, -Math.Pow((distance / lakeArray[i].width), 2));
                    }
                }
            }
            return heightMap;
            }

        // Generates terrain based on height.
        private bool GenerateTerrain(Configuration file)
        {
            heightMap = GenerateHeightmap(file);
            List<TileWithHeight> listOfHeights = SortList(heightMap);
            int numberOfMountains = GetAmount(file, "percentage.mountain.min", "percentage.mountain.max");
            int numberOfLakes = GetAmount(file, "percentage.lake.min", "percentage.lake.max");
            int numberOfHills = GetAmount(file, "percentage.hills.min", "percentage.hills.max");

            int currentHighestTile = 0;
            // Resets terrain
            foreach(TileWithHeight twh in listOfHeights)
            {
                Tile currentTile = this[twh.x, twh.y] as Tile;
                currentTile.Terrain = Terrain.Plains;
                currentTile.hills = false;
            }
            // Generates terrain based on heighest and lowest values in listOfHeight
            for (; currentHighestTile < numberOfMountains; currentHighestTile++)
            {
                Tile currentTile = this[listOfHeights[currentHighestTile].x, listOfHeights[currentHighestTile].y] as Tile;
                currentTile.Terrain = Terrain.Mountain;
                currentTile.hills = false;
            }
            for (; currentHighestTile < numberOfHills+numberOfMountains; currentHighestTile++)
            {
                Tile currentTile = this[listOfHeights[currentHighestTile].x, listOfHeights[currentHighestTile].y] as Tile;
                currentTile.Terrain = Terrain.Plains;
                currentTile.hills = true;
            }
            for (int i = listOfHeights.Count-1; i >= listOfHeights.Count-numberOfMountains; i--)
            {
                Tile currentTile = this[listOfHeights[i].x, listOfHeights[i].y] as Tile;
                currentTile.Terrain = Terrain.Lake;
                currentTile.hills = false;
            }

            return CheckIfEveryTileIsAccessible(file);
        } 

        // Creates a puppet unit to check if every tile is accessible from a certain unobstructed tile
        private bool CheckIfEveryTileIsAccessible(Configuration file)
        {
            // Puts a unit on the first available tile
            int startx = 0;
            int starty = 0;
            while ((this[startx, starty] as Tile).Terrain == Terrain.Mountain || (this[startx, starty] as Tile).Terrain == Terrain.Lake)
            {
                if (startx < Width - 1)
                {
                    startx++;
                }
                else
                {
                    startx = 0;
                    starty++;
                }
            }
            Units.Unit u = Units.UnitRegistry.GetUnit("swordsman", true);
            u.Parent = this;
            u.MovesLeft = u.MoveSpeed = int.MaxValue;
            (this[startx, starty] as Tile).SetUnit(u);
            u.GeneratePaths(u.PositionInGrid);

            // Tests if every other nonobstructed tile is accessible by this unit
            for (int gridX = 0; gridX < Width; ++gridX)
            {
                for (int gridY = 0; gridY < Height; ++gridY)
                {
                    Tile t = this[gridX, gridY] as Tile;
                    Terrain terrain = t.Terrain;
                    if (u.Passable(terrain) && !(u.ReachableTiles().Contains(t.GridPos) || t.GridPos.Equals(u.PositionInGrid)))
                    {
                        mountainArray = ResetMountains(file);
                        lakeArray = ResetLakes(file);
                        t.SetUnit(null);
                        return false;
                    }
                    else
                    {
                        t.SetUnit(null);
                    }
                }
            }
            return true;
        }

        // Creates a new array of random heightpoints
        private Heightpoint[] ResetMountains(Configuration file)
        {
            int mountains = file.GetProperty<int>("mountainPeaks");
            Heightpoint[] returnlist = new Heightpoint[mountains];
            for (int i = 0; i < mountains; i++)
            {
                returnlist[i].position.X =(float) MaxOfEmpires.Random.NextDouble() * Width;
                returnlist[i].position.Y =(float) MaxOfEmpires.Random.NextDouble() * Height;
                returnlist[i].width = (MaxOfEmpires.Random.NextDouble() * 3)+3;
                returnlist[i].height = (MaxOfEmpires.Random.NextDouble() * 20)+3;
            }
            return returnlist;
        }

        // Creates a new arrai of random negative heightpoints
        private Heightpoint[] ResetLakes(Configuration file)
        {
            int lakes = file.GetProperty<int>("lakeBottoms");
            Heightpoint[] returnlist = new Heightpoint[lakes];
            for (int i = 0; i < lakes; i++)
            {
                returnlist[i].position.X =(float) MaxOfEmpires.Random.NextDouble() * Width;
                returnlist[i].position.Y =(float) MaxOfEmpires.Random.NextDouble() * Height;
                returnlist[i].height = -10 + (MaxOfEmpires.Random.NextDouble() * -1);
                returnlist[i].width = 1 + (MaxOfEmpires.Random.NextDouble() * 2);
            }
            return returnlist;
        }

        // Creates humiditypoints based on mountains: lakes make their surroundings humid, mountains make their surroundings dry.
        private Heightpoint[] SetHumidityPoints(Configuration file, Heightpoint[] mountainArray, Heightpoint[] lakeArray)
        {
            int extraHumidityPoints = file.GetProperty<int>("extraHumidityPoints");
            Heightpoint[] HumidityPoints = new Heightpoint[extraHumidityPoints+mountainArray.Length+lakeArray.Length];

            for(int i = 0; i < extraHumidityPoints; i++)
            {
                HumidityPoints[mountainArray.Length + lakeArray.Length + i].position = new Vector2((float)MaxOfEmpires.Random.NextDouble() * Width, (float)MaxOfEmpires.Random.NextDouble() * Height);
                HumidityPoints[mountainArray.Length + lakeArray.Length + i].width = MaxOfEmpires.Random.NextDouble() * 30;
                HumidityPoints[mountainArray.Length + lakeArray.Length + i].height = (MaxOfEmpires.Random.NextDouble()-1) * 10;
            }
            GenerateHumiditySim(file, HumidityPoints);

            return HumidityPoints;
        }

        // Creates a double grid with humidity values
        private void GenerateHumiditySim(Configuration file, Heightpoint[] humidityPoints)
        {
            double[,] gridSim = new double[Width,Height];
            double humidityNoise = file.GetProperty<int>("humidityNoise")/100.0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    gridSim[x, y] = heightMap[x, y] * -((1 - humidityNoise) + MaxOfEmpires.Random.NextDouble() * humidityNoise*2); //Duplicates heightmap with some randomizer
                    for (int i = 0; i < humidityPoints.Length; i++)
                    {
                        Vector2 position = new Vector2(x, y);
                        double distance = Math.Abs((humidityPoints[i].position - position).Length());

                        // Exponentiele formule van Luuk
                        gridSim[x, y] += ((1 - humidityNoise) + (MaxOfEmpires.Random.NextDouble() * humidityNoise)) * 2 * (humidityPoints[i].height * Math.Pow(15, -Math.Pow(((double)distance / humidityPoints[i].width), 2)));
                    }
                }
            }
            ChangeHumidity(file, gridSim);
        }

        // Changes humidity based on a grid of humidity values
        private void ChangeHumidity(Configuration file, double[,] gridSim)
        {
            int numberOfHumid = GetAmount(file, "percentage.humid.min", "percentage.humid.max");


            List<TileWithHeight> listToBeSorted = SortList(gridSim);
            for (int i = 0; i < numberOfHumid; i++)
            {
                int tileX = listToBeSorted[i].x;
                int tileY = listToBeSorted[i].y;
                if ((this[tileX, tileY] as Tile).Terrain == Terrain.Plains)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.Forest;
                }
            }
        }

        // Sorts a two dimensional array on double value and outputs it as a list of tiles with height
        private List<TileWithHeight> SortList(double[,] gridSim)
        {
            List<TileWithHeight> listToBeSorted = new List<TileWithHeight>();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    listToBeSorted.Add(new TileWithHeight(gridSim[x, y], x, y));
                }
            }
            listToBeSorted = new List<TileWithHeight>(listToBeSorted.OrderBy(d => d.height));
            return listToBeSorted;
        }

        // Generates temperature based on Perlin noise
        private void GenerateTemperature(Configuration file, int width, int height, double intensity)
        {
            double[,] gridSim = GeneratePerlinGrid(width, height, intensity);

            int numberOfCold = GetAmount(file, "percentage.cold.min", "percentage.cold.max");
            int numberOfHot = GetAmount(file, "percentage.mountain.min", "percentage.mountain.max");

            List<TileWithHeight> sortedList = SortList(gridSim);
            for (int i = 0; i < numberOfHot; i++)
            {
                int tileX = sortedList[i].x;
                int tileY = sortedList[i].y;
                Terrain tileTerrain = (this[tileX, tileY] as Tile).Terrain;
                if (tileTerrain == Terrain.Plains)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.Desert;
                }
                else if (tileTerrain == Terrain.Forest)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.Jungle;
                }
                else if (tileTerrain == Terrain.Mountain)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.DesertMountain;
                }
            }
            for (int i = sortedList.Count - 1; i >= sortedList.Count-numberOfCold; i--)
            {
                int tileX = sortedList[i].x;
                int tileY = sortedList[i].y;
                Terrain tileTerrain = (this[tileX, tileY] as Tile).Terrain;
                if (tileTerrain == Terrain.Plains)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.Tundra;
                }
                else if(tileTerrain == Terrain.Forest)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.Swamp;
                }
                else if (tileTerrain == Terrain.Mountain)
                {
                    (this[tileX, tileY] as Tile).Terrain = Terrain.TundraMountain;
                }
            }
        }

        //Generates Perlin noise and uses it to create a grid with double values, ranging from -1 to 1
        private double[,] GeneratePerlinGrid(int width, int height, double intensity)
        {
            double positionX = MaxOfEmpires.Random.NextDouble() * 1000;
            double positionY = MaxOfEmpires.Random.NextDouble() * 1000;

            Perlin perlin = new Perlin();
            double[,] returnlist = new double[width, height];
            double highest = double.MinValue;
            double lowest = double.MaxValue;
            for (int x = 0;x<width;x++)
            {
                for (int y = 0; y < height; y++)
                {
                    returnlist[x,y] = perlin.OctavePerlin((x*intensity)+positionX, (y*intensity)+positionY, 0,1,1);
                    if (returnlist[x, y] > highest)
                    {
                        highest = returnlist[x, y];
                    }
                    if (returnlist[x, y] < lowest)
                    {
                        lowest = returnlist[x, y];
                    }
                }
            }
            // Rescales range to -1 > 1
            double middle = highest - (0.5 * (highest - lowest));
            double range = highest - lowest;
            double newRange = 2 / range;
            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    returnlist[x, y] -= middle;
                    returnlist[x, y] *= newRange;
                }
            }
            return returnlist;
        }

    }
}
