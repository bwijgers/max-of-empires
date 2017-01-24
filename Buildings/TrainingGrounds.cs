using Ebilkill.Gui.Elements;
using MaxOfEmpires.GameStates.Overlays;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace MaxOfEmpires.Buildings
{
    class TrainingGrounds : Building
    {
        public TrainingGrounds(Point positionInGrid, Player owner) : base(positionInGrid, owner, "building.trainingGrounds")
        {
        }

        public override void PopulateBuildingActions(GuiList buildingActions)
        {
            // Get this building's trainees
            IList<string> trainees = BuildingRegistry.GetTrainees("building.trainingGrounds");

            // Add a button for every trainee
            foreach (string trainee in trainees)
            {
                AddRecruitingButton(buildingActions, trainee+"."+Owner.UnitTiers[trainee] );
                if(Owner.UnitTiers[trainee] < 3)
                {
                    AddUpgradeButton(buildingActions, trainee, Owner.UnitTiers[trainee], Owner);
                }
            }

            // Add the basic building actions
            base.PopulateBuildingActions(buildingActions);
        }
    }
}
