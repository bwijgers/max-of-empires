using Ebilkill.Gui.Elements;
using MaxOfEmpires.GameStates.Overlays;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class TrainingGrounds : Building
    {
        public TrainingGrounds(Point positionInGrid, Player owner) : base(positionInGrid, owner)
        {
        }

        public override void PopulateBuildingActions(GuiList buildingActions)
        {
            AddRecruitingButton(buildingActions, "unit.swordsman");
            AddRecruitingButton(buildingActions, "unit.archer");
            base.PopulateBuildingActions(buildingActions);
        }
    }
}
