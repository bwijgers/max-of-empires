using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxOfEmpires.Files
{
    interface IConfigurable
    {
        void LoadFromConfiguration(Configuration config);
    }
}
