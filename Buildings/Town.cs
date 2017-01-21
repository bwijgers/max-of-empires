using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Town : Building
    {
        public static int upkeep;

        public Town(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.town")
        {
        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            base.TurnUpdate(turn, player, t);
            if (player == Owner)
            {
                if (!player.CanAfford(upkeep))
                {
                    RazeBuilding();
                    return;
                }
                player.Buy(upkeep);
            }
        }

        public static void LoadFromConfig(Configuration config)
        {
            upkeep = config.GetProperty<int>("town.upkeep");
        }
    }
}
