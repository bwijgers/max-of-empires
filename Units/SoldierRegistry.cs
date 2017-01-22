using MaxOfEmpires.Files;
using System.Collections.Generic;

namespace MaxOfEmpires.Units
{
    class SoldierRegistry
    {
        private static Dictionary<string, Soldier> unitsByName = new Dictionary<string, Soldier>();
        private static Dictionary<string, int> unitCostsByName = new Dictionary<string, int>();
        private static List<Soldier> allSoldiers = new List<Soldier>();
        private static string[] soldierNames = new string[] { "spearman", "swordsman", "archer", "axeman", "horse", "heavy", "mage", "assassin" };

        /// <summary>
        /// Gets a Unit by its name, with a specified owner. Position will be (0,0) until set in the Grid at a specified position.
        /// </summary>
        /// <param name="name">The unlocalized name of the Unit to load.</param>
        /// <param name="owner">The owner of the Unit.</param>
        /// <returns>The requested Unit with specified owner.</returns>
        public static Soldier GetSoldier(string name, Player owner)
        {
            return unitsByName[name].Copy(owner);
        }

        public static int GetSoldierCost(string name)
        {
            return unitCostsByName[name];
        }

        /// <summary>
        /// Initializes the UnitRegistry. Loads all units from configuration.
        /// </summary>
        /// <param name="unitConfiguration">The configuration to load the Units from.</param>
        public static void Init(Configuration unitConfiguration)
        {
            foreach (string s in soldierNames)
            {
                RegisterSoldierFromConfiguration("unit." + s, unitConfiguration.GetPropertySection(s));
            }
        }

        /// <summary>
        /// Registers a Unit by its name into the Unit registry.
        /// </summary>
        /// <param name="name">The name to register the Unit by.</param>
        /// <param name="soldier">The Unit to register.</param>
        private static void RegisterSoldier(string name, Soldier soldier, int cost)
        {
            unitsByName[name] = soldier;
            unitCostsByName[name] = cost;
            allSoldiers.Add(soldier);
        }

        /// <summary>
        /// Registers a Unit from a configuration subsection. The subsection must be unit.type area.
        /// </summary>
        /// <param name="name">The unlocalized name of the Unit to load.</param>
        /// <param name="c">The configuration to load the Unit from.</param>
        public static void RegisterSoldierFromConfiguration(string name, Configuration c)
        {
            // Create a Unit
            Soldier u = Soldier.LoadFromConfiguration(c);

            // Get its cost
            int cost = c.GetProperty<int>("cost");

            // Register the Unit
            RegisterSoldier(name, u, cost);
        }

        public IList<Soldier> AllSoldiers => allSoldiers.AsReadOnly();
    }
}
