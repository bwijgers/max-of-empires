﻿using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Stats : IConfigurable
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

        public void LoadFromConfiguration(Configuration config)
        {
            // Load everything in Stats from the config
            hp = maxHp = config.GetProperty<int>("hp");
            att = config.GetProperty<int>("att");
            def = config.GetProperty<int>("def");
            hit = config.GetProperty<int>("hit");
            dodge = config.GetProperty<int>("dodge");
        }

        public Stats Copy()
        {
            return new Stats(hp, att, def, hit, dodge);
        }

        //public int crit; // crit chance
        //public int avoid; // crit chance

        public static Stats Empty => new Stats(0, 0, 0, 0, 0);
    }
}
