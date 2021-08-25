using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;
using PowerSystemLibrary.Util;

namespace PowerSystemLibrary.DAO
{
    public class LogDAO
    {
        public void AddLog(LogCode logCode, string content, PowerSystemDBContext db, User user = null)
        {
            Log log = new Log();
            log.CreateDate = DateTime.Now;
            log.Content = content;
            log.LogCode = logCode;

            if (user != null)
            {
                log.UserID = user.ID;

            }
            else
            {
                log.UserID = null;
            }

            db.Log.Add(log);
            db.SaveChanges();
        }
    }
}
