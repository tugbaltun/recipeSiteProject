using System.Web;
using System.Web.Optimization;

namespace website
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/js/jquery.min.js",
                        "~/Scripts/js/jquery.easing.min.js",
                        "~/Scripts/js/jquery.sumoselect.js",
                        "~/Scripts/js/jquery.sumoselect.min.js",
                        "~/Scripts/js/rating.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/js/bootstrap.bundle.min.js",
                      "~/Scripts/js/sb-admin-2.min.js",
                      "~/Scripts/gridmvc.min.js",
                      "~/Scripts/js/select2.min.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/all.min.css",
                      "~/Content/css/fontawesome.min.css",
                      "~/Content/bootstrap4/bootstrap.min.css",
                      "~/Content/bootstrap4/Site.css",
                      "~/Content/css/sb-admin-2.min.css",
                      "~/Content/Gridmvc.css",
                      "~/Content/css/dropdownliststyle.css",
                      "~/Content/css/select2.min.css"
                      ));
        }
    }
}

