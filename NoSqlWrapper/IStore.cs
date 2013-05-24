using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoSqlWrapper.Repositories;

namespace NoSqlWrapper
{

    public interface IStore : INoSqlRepository
    {
        void SaveChanges();
    }

}
