namespace MaxOfEmpires
{
    class Player
    {
        private int money;
        private string name;
        private string colorName;

        public Player(string name, string colorName)
        {
            money = 100;
            this.colorName = colorName;
            this.name = name;
        }

        public string ColorName => colorName;
        public string Name => name;
    }
}
