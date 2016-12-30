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
        public int hp;
        public int maxHp;
        public int att; // damage
        public int hit; // chance
        public int dodge; // chance
        public int def; // damage

        public Stats(int hp, int att, int def, int hit, int dodge)
        {
            this.hp = maxHp = hp;
            this.att = att;
            this.hit = hit;
            this.dodge = dodge;
            this.def = def;
        }

        public Stats(Stats origin) : this(origin.maxHp, origin.att, origin.def, origin.hit, origin.dodge)
        {
            hp = origin.hp;
        }

        public static Stats LoadFromConfiguration(Configuration config)
        {
            // Load everything in Stats from the config
            int hp = config.GetProperty<int>("hp");
            int att = config.GetProperty<int>("att");
            int def = config.GetProperty<int>("def");
            int hit = config.GetProperty<int>("hit");
            int dodge = config.GetProperty<int>("dodge");

            return new Stats(hp, att, def, hit, dodge);
        }

        public Stats Copy()
        {
            return new Stats(this);
        }

        //public int crit; // crit chance
        //public int avoid; // crit chance

        public static Stats Empty => new Stats(0, 0, 0, 0, 0);
    }
}
