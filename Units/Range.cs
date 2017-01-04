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

        /// <summary>
        /// Creates a new Range, with a minimum and a maximum range.
        /// </summary>
        /// <param name="min">The minimum range.</param>
        /// <param name="max">The maximum range.</param>
        public Range(int min, int max)
        {
            this.max = max;
            this.min = min;
        }

        /// <summary>
        /// Creates a deep copy of another Range object.
        /// </summary>
        /// <param name="origin">The original to copy.</param>
        public Range(Range origin) : this(origin.min, origin.max)
        {
        }

        /// <summary>
        /// Checks if a number is in this Range, minimum and maximum inclusive. [min, max]
        /// </summary>
        /// <param name="v">The number to check for.</param>
        /// <returns>True if the number is in this Range, false otherwise.</returns>
        public bool InRange(int v)
        {
            return v >= min && v <= max;
        }

        /// <summary>
        /// Loads a Range object from the specified configuration.
        /// </summary>
        /// Note that a Range object requires the following keys:
        ///   - min (int)
        ///   - max (int)
        /// <param name="config">The configuration to load from.</param>
        /// <returns>The Range object as in the configuration.</returns>
        public static Range LoadFromConfiguration(Configuration config)
        {
            int max = config.GetProperty<int>("max");
            int min = config.GetProperty<int>("min");

            return new Range(min, max);
        }

        /// <summary>
        /// Creates a deep copy of this Range object.
        /// </summary>
        /// <returns>The copy of this object.</returns>
        public Range Copy()
        {
            return new Range(this);
        }
    }
}
