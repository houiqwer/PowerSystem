using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PowerSystemLibrary.DBContext;
using PowerSystemLibrary.Entity;

namespace PowerSystemLibrary.Util
{
    public class LoginHelper
    {

        public static User CurrentUser(PowerSystemDBContext db)
        {
            string token = HttpContext.Current.Request.Headers.GetValues("Authorization")[0];
            return db.User.FirstOrDefault(t => t.Token == token);
        }

        public static string GenerateToken(int userID, DateTime expire, PowerSystemDBContext db)
        {
            User user = db.User.FirstOrDefault(t => t.ID == userID);
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Token))
                    user.Token = Guid.NewGuid().ToString();
                user.Expire = expire;
                db.SaveChanges();
            }
            return user.Token;
        }
    }
}
