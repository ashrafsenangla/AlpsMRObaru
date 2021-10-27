using Core.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Repository
{
    public interface IUserRepository
    {
        ApplicationUser SelectById(string id);
        ApplicationUser SelectByUserName(string username);
    }
}
