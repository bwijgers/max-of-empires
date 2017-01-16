using MaxOfEmpires.Files;
using System.Collections.Generic;

namespace MaxOfEmpires.Buildings
{
    class BuildingRegistry
    {
        private static Dictionary<string, int> buildingCosts = new Dictionary<string, int>();

        public static int GetCost(string buildingName)
        {
            if (buildingCosts.ContainsKey(buildingName))
            {
                return buildingCosts[buildingName];
            }
            throw new KeyNotFoundException("The building called '" + buildingName + "' does not exist.");
        }

        public static void InitBuildings(Configuration config)
        {
            buildingCosts["mine"] = config.GetProperty<int>("mine.cost");
            buildingCosts["trainingGrounds"] = config.GetProperty<int>("trainingGrounds.cost");
        }
    }
}