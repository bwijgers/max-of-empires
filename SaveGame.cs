using System;
using System.IO;
using MaxOfEmpires.Files;

namespace MaxOfEmpires.GameStates
{
    class SaveGame
    {
        private EconomyGrid economyGrid;

        public SaveGame(EconomyGrid economyGrid)
        {
            this.economyGrid = economyGrid;
        }

        public void LoadFromFile(BinaryReader reader)
        {
            economyGrid = EconomyGrid.LoadFromFile(reader);
        }

        public void WriteToFile(BinaryWriter stream)
        {
            economyGrid.WriteToFile(stream);
        }

        public EconomyGrid EcoGrid => economyGrid;
    }
}