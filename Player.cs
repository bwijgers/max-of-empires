using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaxOfEmpires
{
    class Player
    {
        public struct Stats
        {
            public List<int> money;
            public List<int> population;
            public List<TimeSpan> duration;
            public List<Dictionary<string, int>> units;
            public List<Dictionary<string, int>> buildings;
            public List<Dictionary<string, int>> lostUnits;
            public List<Dictionary<string, int>> lostBuildings;
            public int battlesWon;
            public int battlesLost;

            public Stats(int something)
            {
                money = new List<int>();
                population = new List<int>();
                duration = new List<TimeSpan>();
                units = new List<Dictionary<string, int>>();
                buildings = new List<Dictionary<string, int>>();
                lostUnits = new List<Dictionary<string, int>>();
                lostBuildings = new List<Dictionary<string, int>>();
                battlesWon = 0;
                battlesLost = 0;
            }
        }
        
        private int moneyPerTurn;
        public Dictionary<string, int> soldierTiers;
        public Stats stats;
        private int population;
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
            money = startingMoney;
            this.colorName = colorName;
            this.name = name;
            this.color = color;
            updateMoneyHandlers = new List<Action<Player>>();
            updatePopulationHandlers = new List<Action<Player>>();
            updateMoneyPerTurnHandlers = new List<Action<Player>>();
            stats = new Stats(0);
            soldierTiers = new Dictionary<string, int>();
            foreach (string s in Buildings.BuildingRegistry.GetTrainees("building.trainingGrounds"))
                soldierTiers[s] = 1;
            foreach (string s in Buildings.BuildingRegistry.GetTrainees("building.academy"))
                soldierTiers[s] = 1;
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
         
            // TODO Read soldier tiers and stats later
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
            // TODO: Write soldierTiers, stats

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

            // Write soldier tiers and stats later
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
