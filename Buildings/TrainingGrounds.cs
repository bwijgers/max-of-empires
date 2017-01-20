using Ebilkill.Gui.Elements;
using MaxOfEmpires.GameStates.Overlays;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MaxOfEmpires.Buildings
{
    class TrainingGrounds : Building
    {
        public TrainingGrounds(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.trainingground")
        {
        }

        public override void PopulateBuildingActions(GuiList buildingActions)
        {
            // Get this building's trainees
            IList<string> trainees = BuildingRegistry.GetTrainees("building.trainingGrounds");

            // Add a button for every trainee
            foreach (string trainee in trainees)
            {
                AddRecruitingButton(buildingActions, trainee);
            }

            // Add the basic building actions
            base.PopulateBuildingActions(buildingActions);
        }
    }
}
