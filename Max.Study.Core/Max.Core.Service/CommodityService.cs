
using Microsoft.EntityFrameworkCore;
using Max.Core.Interface;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Service
{
    public class CommodityService : BaseService, ICommodityService
    {
        public CommodityService(DbContext context) : base(context)
        {
        }
    }
}
