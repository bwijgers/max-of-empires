using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Capital : Building
    {
        public Capital(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.capital")
        {
        }
    }
}
