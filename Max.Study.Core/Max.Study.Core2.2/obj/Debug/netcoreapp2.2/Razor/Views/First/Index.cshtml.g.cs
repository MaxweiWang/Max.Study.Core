#pragma checksum "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "1808ffc51a24e3428d9db194b2205b52f6b41033"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_First_Index), @"mvc.1.0.view", @"/Views/First/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/First/Index.cshtml", typeof(AspNetCore.Views_First_Index))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\_ViewImports.cshtml"
using Max.Study.Core2;

#line default
#line hidden
#line 2 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\_ViewImports.cshtml"
using Max.Study.Core2.Models;

#line default
#line hidden
#line 2 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
using Max.Core.Utility;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1808ffc51a24e3428d9db194b2205b52f6b41033", @"/Views/First/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"de719a8811d779f2ec1816f5b9eab1c0bc151b84", @"/Views/_ViewImports.cshtml")]
    public class Views_First_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Max.Core.Utility.CurrentUser>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(64, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 4 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
  
    ViewBag.Title = "Index";
    CurrentUser userViewData = ViewData["User1"] as CurrentUser;//ViewData需要类型转换
    CurrentUser userViewBag = ViewBag.User;//ViewBag直接用
    CurrentUser userOther = ViewBag.User1;

#line default
#line hidden
            BeginContext(286, 285, true);
            WriteLiteral(@"<div class=""jumbotron"">
    <h1>ASP.NET</h1>
    <p class=""lead"">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS and JavaScript.</p>
    <p><a href=""http://asp.net"" class=""btn btn-primary btn-lg"">Learn more &raquo;</a></p>
</div>
");
            EndContext();
#line 15 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
 if (1 > 2)
{ }
else
{ }

#line default
#line hidden
            BeginContext(600, 2, true);
            WriteLiteral("\r\n");
            EndContext();
#line 20 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
 for (int i = 0; i < 5; i++)
{

#line default
#line hidden
            BeginContext(635, 7, true);
            WriteLiteral("    <p>");
            EndContext();
            BeginContext(643, 1, false);
#line 22 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
  Write(i);

#line default
#line hidden
            EndContext();
            BeginContext(644, 10, true);
            WriteLiteral("</p>\r\n    ");
            EndContext();
            BeginContext(656, 20, true);
            WriteLiteral("  123 &nbsp;234567\r\n");
            EndContext();
            BeginContext(686, 23, true);
            WriteLiteral("\r\n        1234356\r\n    ");
            EndContext();
#line 26 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
           
}

#line default
#line hidden
            BeginContext(721, 89, true);
            WriteLiteral("<div class=\"row\">\r\n    <div class=\"col-md-4\">\r\n        <h2>Getting started</h2>\r\n        ");
            EndContext();
            BeginContext(811, 15, false);
#line 31 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
   Write(base.Model.Name);

#line default
#line hidden
            EndContext();
            BeginContext(826, 13, true);
            WriteLiteral("\r\n        <p>");
            EndContext();
            BeginContext(841, 37, false);
#line 32 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
       Write(((CurrentUser)ViewData["User1"]).Name);

#line default
#line hidden
            EndContext();
            BeginContext(879, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(897, 17, false);
#line 33 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
      Write(userViewData.Name);

#line default
#line hidden
            EndContext();
            BeginContext(914, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(932, 19, false);
#line 34 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
      Write(userViewBag.Account);

#line default
#line hidden
            EndContext();
            BeginContext(951, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(969, 14, false);
#line 35 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
      Write(userOther.Name);

#line default
#line hidden
            EndContext();
            BeginContext(983, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(1002, 32, false);
#line 36 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
       Write(((CurrentUser)ViewBag.User).Name);

#line default
#line hidden
            EndContext();
            BeginContext(1035, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(1054, 36, false);
#line 37 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
       Write(((CurrentUser)TempData["User"]).Name);

#line default
#line hidden
            EndContext();
            BeginContext(1091, 17, true);
            WriteLiteral("</p>\r\n        <p>");
            EndContext();
            BeginContext(1109, 15, false);
#line 38 "D:\BMax\Study\Max.Study.Core\Max.Study.Core2.2\Views\First\Index.cshtml"
      Write(base.Model.Name);

#line default
#line hidden
            EndContext();
            BeginContext(1124, 693, true);
            WriteLiteral(@"</p>
    </div>
    <div class=""col-md-4"">
        <h2>Get more libraries</h2>
        <p>NuGet is a free Visual Studio extension that makes it easy to add, remove, and update libraries and tools in Visual Studio projects.</p>
        <p><a class=""btn btn-default"" href=""http://go.microsoft.com/fwlink/?LinkId=301866"">Learn more &raquo;</a></p>
    </div>
    <div class=""col-md-4"">
        <h2>Web Hosting</h2>
        <p>You can easily find a web hosting company that offers the right mix of features and price for your applications.</p>
        <p><a class=""btn btn-default"" href=""http://go.microsoft.com/fwlink/?LinkId=301867"">Learn more &raquo;</a></p>
    </div>
</div>


");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Max.Core.Utility.CurrentUser> Html { get; private set; }
    }
}
#pragma warning restore 1591
