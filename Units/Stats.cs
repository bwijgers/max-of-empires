using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Stats
    {
        /// <summary>
        /// Loads a new Stats object from a configuration file/subsection.
        /// </summary>
        /// Important to keep in mind: the configuration section needs to contain these keys:
        ///   - hp
        ///   - att
        ///   - def
        ///   - hit
        ///   - dodge
        /// <param name="config">The configuration file/subsection to use.</param>
        /// <returns>The configured Stats object.</returns>
        public static Stats LoadFromConfiguration(Configuration config)
        {
            // Load everything in Stats from the config
            int hp = config.GetProperty<int>("hp");
            int att = config.GetProperty<int>("att");
            int def = config.GetProperty<int>("def");
            int hit = config.GetProperty<int>("hit");
            int dodge = config.GetProperty<int>("dodge");

            // Return stats loaded from config
            return new Stats(hp, att, def, hit, dodge);
        }

        /// <summary>
        /// Returns an empty Stats object.
        /// </summary>
        public static Stats Empty => new Stats(0, 0, 0, 0, 0);

        public int hp;
        public int maxHp;
        public int att; // damage
        public int hit; // chance
        public int dodge; // chance
        public int def; // damage

        /// <summary>
        /// Creates a new Stats object.
        /// </summary>
        /// <param name="hp">The current and max hp.</param>
        /// <param name="att">The attack.</param>
        /// <param name="def">The defence.</param>
        /// <param name="hit">The base hit chance.</param>
        /// <param name="dodge">The base dodge chance.</param>
        private Stats(int hp, int att, int def, int hit, int dodge)
        {
            this.hp = maxHp = hp;
            this.att = att;
            this.hit = hit;
            this.dodge = dodge;
            this.def = def;
        }

        /// <summary>
        /// Creates a deep copy of a Stats object.
        /// </summary>
        /// <param name="origin"></param>
        public Stats(Stats origin) : this(origin.maxHp, origin.att, origin.def, origin.hit, origin.dodge)
        {
            hp = origin.hp;
        }

        /// <summary>
        /// Returns a deep copy of this Stats object.
        /// </summary>
        /// <returns></returns>
        public Stats Copy()
        {
            return new Stats(this);
        }

        //public int crit; // crit chance
        //public int avoid; // crit chance
    }
}
