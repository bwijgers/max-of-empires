using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires
{
    class Terrain
    {
        private static Dictionary<byte, Terrain> allTerrain = new Dictionary<byte, Terrain>();
        private static Terrain plains = new Terrain(TerrainType.PLAINS, 1, 3, 1);
        private static Terrain jungle = new Terrain(TerrainType.JUNGLE, 3, 5, 3);
        private static Terrain desert = new Terrain(TerrainType.DESERT, 2, 3, 3);
        private static Terrain forest = new Terrain(TerrainType.FOREST, 2,5, 1);
        private static Terrain tundra = new Terrain(TerrainType.TUNDRA, 1, 3, 2);
        private static Terrain swamp = new Terrain(TerrainType.SWAMP, 3, 5, 2);
        private static Terrain lake = new Terrain(TerrainType.LAKE, 100, 1, 4);
        private static Terrain mountain = new Terrain(TerrainType.MOUNTAIN, 100, 1, 1);
        private static Terrain tundramountain = new Terrain(TerrainType.TUNDRAMOUNTAIN, 100, 1, 2);
        private static Terrain desertmountain = new Terrain(TerrainType.DESERTMOUNTAIN, 100, 1, 3);

        public static Terrain FromTerrainID(byte ID)
        {
            return allTerrain[ID];
        }

        public static Terrain Plains => plains;
        public static Terrain Jungle => jungle;
        public static Terrain Desert => desert;
        public static Terrain Forest => forest;
        public static Terrain Tundra => tundra;
        public static Terrain Swamp => swamp;
        public static Terrain Lake => lake;
        public static Terrain Mountain => mountain;
        public static Terrain TundraMountain => tundramountain;
        public static Terrain DesertMountain => desertmountain;

        public enum TerrainType : byte
        {
            PLAINS,
            JUNGLE,
            DESERT,
            FOREST,
            TUNDRA,
            SWAMP,
            LAKE,
            MOUNTAIN,
            TUNDRAMOUNTAIN,
            DESERTMOUNTAIN
        }

        public int cost;
        public Point placeInSprite;
        public TerrainType terrainType;
        private Texture2D tex;

        private Terrain(TerrainType terrainType,int cost, int x, int y)
        {
            placeInSprite = new Point(x, y);
            this.cost = cost;
            this.terrainType = terrainType;
            string texName = terrainType.ToString().ToLower();
        //    tex = AssetManager.Instance.getAsset<Texture2D>(@"FE-Sprites\" + texName);
            tex = AssetManager.Instance.getAsset<Texture2D>(@"FE-Sprites\plains");
            allTerrain.Add((byte)terrainType, this);
        }

        public int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }

        public bool IsMountain
        {
            get
            {
                return terrainType == TerrainType.DESERTMOUNTAIN || terrainType == TerrainType.MOUNTAIN || terrainType == TerrainType.TUNDRAMOUNTAIN;
            }
        }
    }
}
