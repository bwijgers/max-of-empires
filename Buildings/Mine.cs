using MaxOfEmpires.Files;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Mine : Building
    {
        public static int moneyPerTurn;

        public Mine(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.mine")
        {
            owner.CalculateMoneyPerTurn();
        }

        public override void TurnUpdate(uint turn, Player player, GameTime t)
        {
            base.TurnUpdate(turn, player, t);
            if (player == Owner)
            {
            }
        }

        public static void LoadFromConfig(Configuration config)
        {
            moneyPerTurn = config.GetProperty<int>("mine.moneyPerTurn");
        }
    }
}
