﻿using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class SoldierRegistry
    {
        private static Dictionary<string, Soldier> unitsByName = new Dictionary<string, Soldier>();
        private static List<Soldier> allSoldiers = new List<Soldier>();

        /// <summary>
        /// Gets a Unit by its name, with a specified owner. Position will be (0,0) until set in the Grid at a specified position.
        /// </summary>
        /// <param name="name">The unlocalized name of the Unit to load.</param>
        /// <param name="owner">The owner of the Unit.</param>
        /// <returns>The requested Unit with specified owner.</returns>
        public static Soldier GetSoldier(string name, bool owner)
        {
            return unitsByName[name].Copy(owner);
        }

        /// <summary>
        /// Initializes the UnitRegistry. Loads all units from configuration.
        /// </summary>
        /// <param name="unitConfiguration">The configuration to load the Units from.</param>
        public static void Init(Configuration unitConfiguration)
        {
            RegisterSoldierFromConfiguration("unit.swordsman", unitConfiguration.GetPropertySection("swordsman"));
            RegisterSoldierFromConfiguration("unit.archer", unitConfiguration.GetPropertySection("archer"));
        }

        /// <summary>
        /// Registers a Unit by its name into the Unit registry.
        /// </summary>
        /// <param name="name">The name to register the Unit by.</param>
        /// <param name="u">The Unit to register.</param>
        private static void RegisterSoldier(string name, Soldier u)
        {
            unitsByName[name] = u;
            allSoldiers.Add(u);
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

            // Register the Unit
            RegisterSoldier(name, u);
        }

        public IList<Soldier> AllSoldiers => allSoldiers.AsReadOnly();
    }
}
