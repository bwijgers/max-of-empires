using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Town : Building
    {
        public static int moneyPerTurn;

        public Town(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.town")
        {
        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            base.TurnUpdate(turn, player, t);
            if (player == Owner)
            {
                player.EarnMoney(moneyPerTurn);
            }
        }

        public static void LoadFromConfig(Configuration config)
        {
            moneyPerTurn = config.GetProperty<int>("town.moneyPerTurn");
        }
    }
}
