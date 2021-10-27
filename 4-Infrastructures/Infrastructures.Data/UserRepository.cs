using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Core.Entities.Data;

namespace Infrastructures.Data
{
    public class UserRepository: BaseDAC, IUserRepository
    {
        public ApplicationUser SelectById(string id)
        {
            return _db.Users.Where(x => x.Id == id).FirstOrDefault();
        }
        public ApplicationUser SelectByUserName(string username)
        {
            return _db.Users.Where(x => x.UserName == username).FirstOrDefault();
        }
    }
}
