using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MaxOfEmpires
{
    class Player
    {
        private int population;
        private int money;
        private string name;
        private string colorName;
        private Color color;

        public EconomyGrid grid;

        private List<Action<Player>> updateMoneyHandlers;
        private List<Action<Player>> updatePopulationHandlers;

        public Player(string name, string colorName, Color color, int startingMoney)
        {
            money = startingMoney;
            this.colorName = colorName;
            this.name = name;
            this.color = color;
            updateMoneyHandlers = new List<Action<Player>>();
            updatePopulationHandlers = new List<Action<Player>>();
        }

        public void Buy(int cost)
        {
            Money -= cost;
        }

        public bool CanAfford(int cost)
        {
            return cost <= Money;
        }

        public void EarnMoney(int amount)
        {
            Money += amount;
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

        public void CalculatePopulation()
        {
            int pop = 0;
            grid.ForEach(obj => {
            Tile t = obj as Tile;
                if (t.BuiltOn && t.Building.Owner == this) {
                    if (t.Building.id.Equals("building.capital"))
                    {
                        pop += 10;
                    }

                    else if (t.Building.id.Equals("building.town"))
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

        public string Name => name;
    }
}
