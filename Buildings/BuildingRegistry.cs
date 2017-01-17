using MaxOfEmpires.Files;
using System.Collections.Generic;

namespace MaxOfEmpires.Buildings
{
    class BuildingRegistry
    {
        private struct BuildingInformation
        {
            private int cost;
            private List<string> trainees;

            public BuildingInformation(int cost, List<string> trainees)
            {
                this.cost = cost;
                this.trainees = new List<string>(trainees);
            }

            public int Cost => cost;
            public IList<string> Trainees => trainees.AsReadOnly();
        }

        private static Dictionary<string, BuildingInformation> buildingInfo = new Dictionary<string, BuildingInformation>();

        /// <summary>
        /// Gets the cost of the specified Building.
        /// </summary>
        /// <param name="buildingName">The name of the Building.</param>
        /// <returns>The cost of the Building.</returns>
        public static int GetCost(string buildingName)
        {
            if (buildingInfo.ContainsKey(buildingName))
            {
                return buildingInfo[buildingName].Cost;
            }
            throw new KeyNotFoundException("The building called '" + buildingName + "' does not exist.");
        }

        /// <summary>
        /// Gets all Soldiers that can be recruited in the specified Building.
        /// </summary>
        /// <param name="buildingName">The name of the Building.</param>
        /// <returns>All recruitable Soldiers.</returns>
        public static IList<string> GetTrainees(string buildingName)
        {
            if (buildingInfo.ContainsKey(buildingName))
            {
                return buildingInfo[buildingName].Trainees;
            }
            throw new KeyNotFoundException("The building called '" + buildingName + "' does not exist.");
        }

        /// <summary>
        /// Initializes all buildings in the game.
        /// </summary>
        /// <param name="config">The configuration section containing all buildings.</param>
        public static void InitBuildings(Configuration config)
        {
            buildingInfo["building.mine"] = GetBuildingInformation(config.GetPropertySection("mine"));
            buildingInfo["building.trainingGrounds"] = GetBuildingInformation(config.GetPropertySection("trainingGrounds"));
        }

        /// <summary>
        /// Creates a BuildingInformation object corresponding to the building supplied.
        /// </summary>
        /// <param name="config">The config to load the building information from.</param>
        /// <returns>The BuildingInformation corresponding to the specified building.</returns>
        private static BuildingInformation GetBuildingInformation(Configuration config)
        {
            int cost = config.GetProperty<int>("cost");
            List<string> trainees = config.GetProperty<List<string>>("trainees");

            return new BuildingInformation(cost, trainees);
        }
    }
}