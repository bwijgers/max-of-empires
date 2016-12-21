using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Swordsman : Unit
    {
        private static string textureName;
        private static int baseHp, baseMaxHp, baseAtt, baseDef, baseHit, baseDodge;

        public Swordsman(int x, int y, bool owner) : base(x, y, owner, textureName)
        {
            this.moveSpeed = 4;
            Stats = new Stats(baseHp, baseAtt, baseDef, baseHit, baseDodge);
        }

        public static void LoadConfig(Configuration mainConfig)
        {
            // Load the configuration file
            Configuration config = mainConfig.GetPropertySection("swordsman");

            // Load the values from the config file
            baseHp = baseMaxHp = config.GetProperty<int>("hp");
            baseAtt = config.GetProperty<int>("att");
            baseDef = config.GetProperty<int>("def");
            baseHit = config.GetProperty<int>("hit");
            baseDodge = config.GetProperty<int>("dodge");

            // Load texture from config file
            textureName = config.GetProperty<string>("texture.name");
        }
    }
}
