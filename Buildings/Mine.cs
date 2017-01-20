using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Mine : Building
    {
        public static int moneyPerTurn;

        public Mine(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.mine")
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
            moneyPerTurn = config.GetProperty<int>("mine.moneyPerTurn");
        }
    }
}
