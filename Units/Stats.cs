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

        public Stats(int hp, int att, int hit, int dodge, int def)
        {
            this.hp = this.maxHp = hp;
            this.att = att;
            this.hit = hit;
            this.dodge = dodge;
            this.def = def;
        }

        //public int crit; // crit chance
        //public int avoid; // crit chance
    }
}
