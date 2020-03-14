using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Utility.Filters
{
    public class CutomAuthorizeAttribute : AuthorizeAttribute
    {
        public CutomAuthorizeAttribute()
        {
            this.Roles = "Admin";
        }
    }
}
