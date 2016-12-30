using MaxOfEmpires.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Units
{
    class Range
    {
        private int max;
        private int min;

        public Range(int min, int max)
        {
            this.max = max;
            this.min = min;
        }

        public Range(Range origin) : this(origin.min, origin.max)
        {
        }

        public bool InRange(int v)
        {
            return v >= min && v <= max;
        }

        public static Range LoadFromConfiguration(Configuration config)
        {
            int max = config.GetProperty<int>("max");
            int min = config.GetProperty<int>("min");

            return new Range(min, max);
        }

        public Range Copy()
        {
            return new Range(this);
        }
    }
}
