﻿
using Microsoft.EntityFrameworkCore;
using Max.Core.Interface;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Core.Service
{
    public class CompanyService : BaseService, ICompanyService
    {
        public CompanyService(DbContext context) : base(context)
        {
        }

        public void TestError()
        {
            throw new NotImplementedException();
        }


        //public void Add()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Delete()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Query()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Update()
        //{
        //    throw new NotImplementedException();
        //}
    }
}