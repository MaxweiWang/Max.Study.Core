
using Max.EFCore3_1.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Interface
{
    public interface IUserService : IBaseService
    {
        //void Query();
        //void Update();
        //void Delete();
        //void Add();

        void UpdateLastLogin(User user);
    }
}
