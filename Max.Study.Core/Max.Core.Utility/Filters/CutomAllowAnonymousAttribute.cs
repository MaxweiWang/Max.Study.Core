using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Utility.Filters
{
    public class CutomAllowAnonymousAttribute : Attribute, IAllowAnonymous
    {
    }
}
