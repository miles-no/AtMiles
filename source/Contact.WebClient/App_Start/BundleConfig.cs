using SquishIt.Framework;
using SquishIt.Framework.Minifiers.JavaScript;
using YuiMinifier = SquishIt.Framework.Minifiers.CSS.YuiMinifier;

namespace Contact.WebClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles()
        {
            
            Bundle.JavaScript()
                .Add("~/Scripts/App/homeController.js")
                .WithMinifier<MsMinifier>()
                .AsNamed("home_scripts","~/scripts/home_#.js");

            Bundle.JavaScript()
             .Add("~/Scripts/jquery-2.1.1.js")
             .Add("~/Scripts/angular.js")
             .WithMinifier<MsMinifier>()
             .AsNamed("start_scripts", "~/scripts/lib_#.js");
            

            Bundle.JavaScript()
             //.AddDirectory("~/Scripts/foundation")
             .Add("~/Scripts/foundation/foundation.js")
             .Add("~/Scripts/foundation/foundation.tab.js")
     
             .WithMinifier<MsMinifier>()
             .AsNamed("foundation", "~/scripts/lib_#.js");
            

            Bundle.Css()
                .AddDirectory("~/Content/foundation")
                .WithMinifier<YuiMinifier>()
                .AsNamed("styles","~/content/styles_#.css");


            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            
        }
    }
}