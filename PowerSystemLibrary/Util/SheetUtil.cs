using PowerSystemLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemLibrary.Util
{
    public class SheetUtil
    {
        public static string BuildNO(VoltageType voltageType, SheetType sheetType, int number)
        {
            return BaseUtil.ToDescription(voltageType) + BaseUtil.ToDescription(sheetType) + DateTime.Now.Date.ToString("yyyyMMdd") + number.ToString().PadLeft(2, '0');
        }
    }
}
