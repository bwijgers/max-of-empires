using System;
using System.Collections.Generic;

namespace MaxOfEmpires
{
    class Player
    {
        private int money;
        private string name;
        private string colorName;

        private List<Action<Player>> updateMoneyHandlers;

        public Player(string name, string colorName)
        {
            money = 100;
            this.colorName = colorName;
            this.name = name;
            updateMoneyHandlers = new List<Action<Player>>();
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

        private void UpdateMoney()
        {
            foreach (Action<Player> handler in updateMoneyHandlers)
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

        public string Name => name;
    }
}
