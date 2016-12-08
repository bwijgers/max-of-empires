using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Swordsman : Unit
    {
        public Swordsman(int x, int y, bool owner) : base(x, y, owner, "swordsman")
        {
            this.moveSpeed = 4;
            Stats = new Stats(10, 5, 100, 10, 2);
        }
    }
}
