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
            buildingActions.addElement(ElementBuildButton.CreateBuildButton(buildingActions.Bounds.Location, "Swordsman (10G): ", () => TrySpawnUnit("unit.swordsman")));
            buildingActions.addElement(ElementBuildButton.CreateBuildButton(buildingActions.Bounds.Location, "Archer (15G): ", () => TrySpawnUnit("unit.archer")));
            base.PopulateBuildingActions(buildingActions);
        }
    }
}
