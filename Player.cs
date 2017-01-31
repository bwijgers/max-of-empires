using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnitDict = System.Collections.Generic.Dictionary<string, int>;

namespace MaxOfEmpires
{
    class Player
    {
        public struct Stats
        {
            public static Stats LoadFromFile(BinaryReader reader)
            {
                // Create the return value
                Stats retVal = new Stats(0);

                // Read money
                ushort moneyCount = reader.ReadUInt16();
                for (ushort i = 0; i < moneyCount; ++i)
                {
                    retVal.money.Add(reader.ReadInt32());
                }

                // Read the population
                ushort populationCount = reader.ReadUInt16();
                for (ushort i = 0; i < populationCount; ++i)
                {
                    retVal.population.Add(reader.ReadUInt16());
                }

                // Read all turn durations
                ushort durationCount = reader.ReadUInt16();
                for (ushort i = 0; i < durationCount; ++i)
                {
                    retVal.durationInMinutes.Add(reader.ReadDouble());
                }

                // Read all Units
                retVal.units = ReadUnitOrBuildingDataList(reader);

                // Read all Buildings
                retVal.buildings = ReadUnitOrBuildingDataList(reader);

                // Read all lost Units
                retVal.lostUnits = ReadUnitOrBuildingDataList(reader);

                // Read all lost Buildings
                retVal.lostBuildings = ReadUnitOrBuildingDataList(reader);

                // Read amount of battles won and lost
                retVal.battlesWon = reader.ReadUInt16();
                retVal.battlesLost = reader.ReadUInt16();

                return retVal;
            }

            private static List<UnitDict> ReadUnitOrBuildingDataList(BinaryReader reader)
            {
                // Create the return value list
                List<UnitDict> retVal = new List<UnitDict>();

                // Fill the list
                ushort listCount = reader.ReadUInt16();
                for (ushort i = 0; i < listCount; ++i)
                {
                    // Create the new dict
                    UnitDict dict = new UnitDict();

                    // Get the amount in this dict
                    ushort dictCount = reader.ReadByte();

                    // Add every (key, value) pair to the current dict
                    for (ushort j = 0; j < dictCount; ++j)
                    {
                        dict[reader.ReadString()] = reader.ReadInt32();
                    }
                    retVal.Add(dict);
                }

                return retVal;
            }

            public List<int> money;
            public List<int> population;
            public List<double> durationInMinutes;
            public List<UnitDict> units;
            public List<UnitDict> buildings;
            public List<UnitDict> lostUnits;
            public List<UnitDict> lostBuildings;
            public int battlesWon;
            public int battlesLost;

            public Stats(int something)
            {
                money = new List<int>();
                population = new List<int>();
                durationInMinutes = new List<double>();
                units = new List<UnitDict>();
                buildings = new List<UnitDict>();
                lostUnits = new List<UnitDict>();
                lostBuildings = new List<UnitDict>();
                battlesWon = 0;
                battlesLost = 0;
            }

            public void WriteToFile(BinaryWriter stream)
            {
                // Write all moneys
                stream.Write((ushort)(money.Count & 65535));
                for (int i = 0; i < money.Count; ++i)
                {
                    stream.Write(money[i]);
                }

                // Write all populations
                stream.Write((ushort)(population.Count & 65535));
                for (int i = 0; i < population.Count; ++i)
                {
                    stream.Write((ushort)population[i]);
                }

                // Write all turn durations
                stream.Write((ushort)(durationInMinutes.Count & 65535));
                for (int i = 0; i < durationInMinutes.Count; ++i)
                {
                    stream.Write(durationInMinutes[i]);
                }

                // Write all Units
                WriteUnitOrBuildingDataList(units, stream);

                // Write all Buildings
                WriteUnitOrBuildingDataList(buildings, stream);

                // Write all lost Units
                WriteUnitOrBuildingDataList(lostUnits, stream);

                // Write all lost Buildings
                WriteUnitOrBuildingDataList(lostBuildings, stream);

                // Write the amount of battles won and lost
                ushort battlesWon = (ushort)(this.battlesWon & 65535);
                ushort battlesLost = (ushort)(this.battlesLost & 65535);
                stream.Write(battlesWon);
                stream.Write(battlesLost);
            }

            private void WriteUnitOrBuildingDataList(List<UnitDict> toWrite, BinaryWriter writer)
            {
                // Write the amount of dicts
                writer.Write((ushort)toWrite.Count);
                for (int i = 0; i < toWrite.Count; ++i)
                {
                    // The current dictionary to write to file
                    UnitDict dict = toWrite[i];

                    // Write the amount in this dict
                    writer.Write((byte)dict.Count);

                    // Write everything in this dict
                    foreach (string name in dict.Keys)
                    {
                        writer.Write(name);
                        writer.Write(dict[name]);
                    }
                }
            }
        }
        
        private int moneyPerTurn;
        public Dictionary<string, int> soldierTiers;
        public Stats stats;
        private int population;
        private readonly int startingMoney;
        private int money;
        private string name;
        private string colorName;
        private Color color;
        private Vector2 ecoCamPos;
        private Vector2 battleCamPos;
        private float zoomValue = 1.0f;

        public EconomyGrid grid;

        private List<Action<Player>> updateMoneyHandlers;
        private List<Action<Player>> updatePopulationHandlers;
        private List<Action<Player>> updateMoneyPerTurnHandlers;

        public Player(string name, string colorName, Color color, int startingMoney)
        {
            money = this.startingMoney = startingMoney;
            this.colorName = colorName;
            this.name = name;
            this.color = color;
            updateMoneyHandlers = new List<Action<Player>>();
            updatePopulationHandlers = new List<Action<Player>>();
            updateMoneyPerTurnHandlers = new List<Action<Player>>();
            Reset();
        }

        public void AddBuildingToStats(string id)
        {
            if (!stats.buildings[stats.money.Count - 1].ContainsKey(id))
            {
                stats.buildings[stats.money.Count - 1][id] = 0;
            }
            stats.buildings[stats.money.Count - 1][id]++;
        }

        public void AddUnitLostToStats(string id)
        {
            if (!stats.buildings[stats.money.Count - 1].ContainsKey(id))
            {
                stats.buildings[stats.money.Count - 1][id] = 0;
            }
            stats.buildings[stats.money.Count - 1][id]++;
        }

        public void AddBuildingLostToStats(string id)
        {
            if (!stats.buildings[stats.money.Count - 1].ContainsKey(id))
            {
                stats.buildings[stats.money.Count - 1][id] = 0;
            }
            stats.buildings[stats.money.Count - 1][id]++;
        }

        public void AddUnits(Dictionary<string,int> unitsAndCounts)
        {
            foreach(string k in unitsAndCounts.Keys)
            {
                if (!stats.units[stats.money.Count - 1].ContainsKey(k))
                {
                    stats.units[stats.money.Count - 1][k] = 0;
                }
                stats.units[stats.money.Count - 1][k]+= unitsAndCounts[k];
            }
        }
        public void Buy(int cost)
        {
            Money -= cost;
        }

        public void CalculateMoneyPerTurn()
        {
            int mpt = 0;
            grid.ForEach(obj => {
                Tile t = obj as Tile;
                if (t.BuiltOn && t.Building.Owner == this)
                {
                    if (t.Building.buildingName.Equals("building.mine"))
                    {
                        mpt += Buildings.Mine.moneyPerTurn;
                    }

                    if (t.Building.buildingName.Equals("building.capital"))
                    {
                        mpt += Buildings.Capital.moneyPerTurn;
                    }

                    else if (t.Building.buildingName.Equals("building.town"))
                    {
                        mpt -= Buildings.Town.upkeep;
                    }
                }
            });
            MoneyPerTurn = mpt;
        }

        public void CalculatePopulation()
        {
            int pop = 0;
            grid.ForEach(obj => {
                Tile t = obj as Tile;
                if (t.BuiltOn && t.Building.Owner == this)
                {
                    if (t.Building.buildingName.Equals("building.capital"))
                    {
                        pop += 10;
                    }

                    else if (t.Building.buildingName.Equals("building.town"))
                    {
                        pop += 5;
                    }
                }
                if ((t.Unit as Units.Army) != null && t.Unit.Owner == this)
                {
                    pop -= (t.Unit as Units.Army).GetTotalUnitCount();
                }
            });
            Population = pop;
        }

        public bool CanAfford(int cost)
        {
            return cost <= Money;
        }

        public void EarnMoney(int amount)
        {
            Money += amount;
        }

        public static Player LoadFromFile(BinaryReader stream)
        {
            // Read information we need to create a Player
            string name = stream.ReadString();
            string colorName = stream.ReadString();
            Color color = new Color(stream.ReadByte(), stream.ReadByte(), stream.ReadByte());

            // Create the player
            Player retVal = new Player(name, colorName, color, 0);

            // Read cameras
            // Battlecam
            retVal.battleCamPos = new Vector2(stream.ReadSingle(), stream.ReadSingle());

            // Ecocam
            retVal.ecoCamPos = new Vector2(stream.ReadSingle(), stream.ReadSingle());

            // Zoom
            retVal.zoomValue = stream.ReadSingle();

            // Read money
            retVal.money = stream.ReadInt32();

            // Read stats
            retVal.stats = Stats.LoadFromFile(stream);

            // TODO: Read soldier tiers
            
            return retVal;
        }

        public void OnUpdateMoney(Action<Player> action)
        {
            if (action != null && action.GetInvocationList().Length > 0)
                updateMoneyHandlers.Add(action);
        }

        public void OnUpdatePopulation(Action<Player> action)
        {
            if (action != null && action.GetInvocationList().Length > 0)
                updatePopulationHandlers.Add(action);
        }

        public void OnUpdateMoneyPerTurn(Action<Player> action)
        {
            if (action != null && action.GetInvocationList().Length > 0)
                updateMoneyPerTurnHandlers.Add(action);
        }

        public void Reset()
        {
        	money = startingMoney;
            ResetCamera();
            stats = new Stats(0);
            soldierTiers = new Dictionary<string, int>();
            foreach (string s in Buildings.BuildingRegistry.GetTrainees("building.trainingGrounds"))
                soldierTiers[s] = 1;
            foreach (string s in Buildings.BuildingRegistry.GetTrainees("building.academy"))
                soldierTiers[s] = 1;
        }

        public void ResetCamera()
        {
            EcoCameraPosition = new Vector2(0, 0);
            BattleCameraPosition = new Vector2(0, 0);
            ZoomValue = 1.0f;
        }

        private void UpdateMoney()
        {
            foreach (Action<Player> handler in updateMoneyHandlers)
            {
                handler(this);
            }
        }

        private void UpdatePopulation()
        {
            foreach (Action<Player> handler in updatePopulationHandlers)
            {
                handler(this);
            }
        }

        private void UpdateMoneyPerTurn()
        {
            foreach (Action<Player> handler in updateMoneyPerTurnHandlers)
            {
                handler(this);
            }
        }

        public void WriteToFile(BinaryWriter stream)
        {
            // Write name
            stream.Write(name);

            // Write colorName
            stream.Write(colorName);

            // Write color
            stream.Write(color.R);
            stream.Write(color.G);
            stream.Write(color.B);

            // Write cameras
            stream.Write(battleCamPos.X);
            stream.Write(battleCamPos.Y);
            stream.Write(ecoCamPos.X);
            stream.Write(ecoCamPos.Y);
            stream.Write(zoomValue);

            // Write money
            stream.Write(money);

            // Write stats
            stats.WriteToFile(stream);

            // TODO: Write soldier tiers
        }

        public string ColorName => colorName;

        public int Money
        {
            get
            {
                return money;
            }
            private set
            {
                money = value;
                UpdateMoney();
            }
        }

        public Color Color => color;

        public int Population
        {
            get
            {
                return population;
            }
            set
            {
                population = value;
                UpdatePopulation();
            }
        }

        public int MoneyPerTurn
        {
            get
            {
                return moneyPerTurn;
            }
            set
            {
                moneyPerTurn = value;
                UpdateMoneyPerTurn();
            }
        }

        public string Name => name;

        public Vector2 BattleCameraPosition
        {
            get
            {
                return battleCamPos;
            }
            set
            {
                battleCamPos = value;
            }
        }

        public Vector2 EcoCameraPosition
        {
            get
            {
                return ecoCamPos;
            }
            set
            {
                ecoCamPos = value;
            }
        }

        public float ZoomValue
        {
            get
            {
                return zoomValue;
            }
            set
            {
                zoomValue = value;
            }
        }
    }
}
