using MaxOfEmpires.Files;
using System.Collections.Generic;
using System;

namespace MaxOfEmpires.Buildings
{
    class BuildingRegistry
    {
        private struct BuildingInformation
        {
            public readonly int cost;
            public readonly string textureName;
            private List<string> trainees;
            public readonly int turnsBeforeRazeOnSeize;

            public BuildingInformation(int cost, int turnsBeforeRazeOnSeize, string textureName, List<string> trainees)
            {
                this.cost = cost;
                this.textureName = textureName;
                this.trainees = new List<string>(trainees);
                this.turnsBeforeRazeOnSeize = turnsBeforeRazeOnSeize;
            }

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
                return buildingInfo[buildingName].cost;
            }
            throw new KeyNotFoundException("The building called '" + buildingName + "' does not exist.");
        }

        public static int GetRazeTime(string buildingName)
        {
            if (buildingInfo.ContainsKey(buildingName))
            {
                return buildingInfo[buildingName].turnsBeforeRazeOnSeize;
            }
            throw new KeyNotFoundException("The building called '" + buildingName + "' does not exist.");
        }

        public static string GetTextureName(string buildingName)
        {
            if (buildingInfo.ContainsKey(buildingName))
            {
                return buildingInfo[buildingName].textureName;
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
            buildingInfo["building.town"] = GetBuildingInformation(config.GetPropertySection("town"));
            buildingInfo["building.mine"] = GetBuildingInformation(config.GetPropertySection("mine"));
            buildingInfo["building.trainingGrounds"] = GetBuildingInformation(config.GetPropertySection("trainingGrounds"));
            buildingInfo["building.academy"] = GetBuildingInformation(config.GetPropertySection("academy"));
            buildingInfo["building.capital"] = GetBuildingInformation(config.GetPropertySection("capital"));
        }

        /// <summary>
        /// Creates a BuildingInformation object corresponding to the building supplied.
        /// </summary>
        /// <param name="config">The config to load the building information from.</param>
        /// <returns>The BuildingInformation corresponding to the specified building.</returns>
        private static BuildingInformation GetBuildingInformation(Configuration config)
        {
            int cost = config.GetProperty<int>("cost");
            int turnsBeforeRazeOnSeize = config.GetProperty<int>("razeTime");
            string textureName = config.GetProperty<string>("texture.name");
            List<string> trainees = config.GetProperty<List<string>>("trainees");

            return new BuildingInformation(cost, turnsBeforeRazeOnSeize, textureName, trainees);
        }
    }
}