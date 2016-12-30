using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class UnitRegistry
    {
        private static Dictionary<string, Unit> unitsByName = new Dictionary<string, Unit>();

        public static Unit GetUnit(string name, bool owner)
        {
            return unitsByName[name].Copy(owner);
        }

        public static void Init(Configuration unitConfiguration)
        {
            RegisterUnitFromConfiguration("swordsman", unitConfiguration.GetPropertySection("swordsman"));
            RegisterUnitFromConfiguration("archer", unitConfiguration.GetPropertySection("archer"));
        }

        public static void RegisterUnit(string name, Unit u)
        {
            unitsByName[name] = u;
        }

        public static void RegisterUnitFromConfiguration(string name, Configuration c)
        {
            // Create a Unit
            Unit u = Unit.LoadFromConfiguration(c);

            // Register the Unit
            RegisterUnit(name, u);
        }
    }
}
