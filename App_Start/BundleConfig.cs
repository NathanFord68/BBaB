using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BBaB.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/metro-all.min.css")
                .IncludeDirectory("~/Content", ".css"));

            bundles.Add(new ScriptBundle("~/Scripts").Include(
                "~/Scripts/metro.min.js"));
        }
    }
}