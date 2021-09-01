using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Enum
{
   
    public enum AHState
    {
        正常 = 0,
        停电 = 1,
        掉线 = 99
    }

    public enum VoltageType
    {
        [Description("DY")]
        低压 = 1,
        [Description("GY")]
        高压 = 2
    }

}
