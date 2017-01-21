using Ebilkill.Gui.Elements;
using MaxOfEmpires.Files;
using MaxOfEmpires.GameStates.Overlays;
using MaxOfEmpires.Units;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Capital : Building
    {
        private static int moneyPerTurn;

        public Capital(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.capital")
        {
        }

        public override void TurnUpdate(uint turn, Player player)
        {
            base.TurnUpdate(turn, player);
            if (player == Owner)
            {
                player.EarnMoney(moneyPerTurn);
            }
        }

        public static void LoadFromConfig(Configuration config)
        {
            moneyPerTurn = config.GetProperty<int>("capital.moneyPerTurn");
        }

        public override void PopulateBuildingActions(GuiList buildingActions)
        {
            buildingActions.addElement(ElementBuildButton.CreateBuildButton(buildingActions.Bounds.Location, "Builder (500G)", () => TrySpawnBuilder()));
        }

        private void TrySpawnBuilder()
        {
            // Check if the player can afford this soldier
            int cost = 500;
            if (!Owner.CanAfford(cost) || Owner.Population <= 0)
            {
                return;
            }

            // Set this soldier in the world if possible
            Tile currentTile = ((GameWorld as Grid)[PositionInGrid] as Tile);
            if (!currentTile.Occupied)
            {
                // Nothing here, just place it in this square
                Builder b = new Builder(PositionInGrid.X, PositionInGrid.Y, Owner);
                currentTile.SetUnit(b);
            }
            else
            {
                // We can't place it, just stop this whole function
                return;
            }

            // Buy the soldier, as we placed it.
            Owner.Buy(cost);
            Owner.CalculatePopulation();
        }
    }
}
