using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.Entity;

namespace XIDNA.Repository
{
    public class SQLLogin
    {
        private ModelDbContext db = new ModelDbContext();

        public void PutOrPostLogin(cXIAppUsers login)
        {
            var logins = db.XIAppUsers.Where(l => l.sUserName == login.sUserName);

            if (logins.Any())
            {
                cXIAppUsers tempLogin = logins.First();
                tempLogin.SecurityStamp = login.SecurityStamp;
                //tempLogin.Date = login.Date;
                db.Entry(tempLogin).State = EntityState.Modified;
            }
            else
            {
                db.XIAppUsers.Add(login);
            }
            db.SaveChanges();

        }

        public bool IsLoggedIn(string user, string session)
        {
            var logins = db.XIAppUsers.Where(l => l.sUserName == user && l.SecurityStamp == session);

            if (logins.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
