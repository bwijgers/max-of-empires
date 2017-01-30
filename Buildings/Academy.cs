using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ebilkill.Gui.Elements;
using Microsoft.Xna.Framework;

namespace MaxOfEmpires.Buildings
{
    class Academy : Building
    {
        public const string BUILDING_ID = "building.academy";

        public Academy(Point positionInGrid, Player owner) : base(positionInGrid, owner, BUILDING_ID)
        {
        }

        public override void PopulateBuildingActions(GuiList buildingActions)
        {
            // Get this building's trainees
            IList<string> trainees = BuildingRegistry.GetTrainees(BUILDING_ID);

            // Add a button for every trainee
            foreach (string trainee in trainees)
            {
                AddRecruitingButton(buildingActions, trainee+"."+Owner.soldierTiers[trainee]);
                if (Owner.soldierTiers[trainee] < 3)
                {
                    AddUpgradeButton(buildingActions, trainee, Owner.soldierTiers[trainee], Owner);
                }
            }

            // Add the basic building actions
            base.PopulateBuildingActions(buildingActions);
        }
    }
}
