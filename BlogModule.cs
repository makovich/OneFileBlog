#region AssemblyInfo

[assembly: System.Reflection.AssemblyTitle("One File Blog Engine")]
[assembly: System.Reflection.AssemblyDescription("Blog engine in a single file (by NancyFX & Strapdown.js)")]
#if DEBUG
[assembly: System.Reflection.AssemblyConfiguration("Debug")]
#else
[assembly: System.Reflection.AssemblyConfiguration("Release")]
#endif
[assembly: System.Reflection.AssemblyCompany("The Oneman Company")]
[assembly: System.Reflection.AssemblyProduct("OneFileBlog")]
[assembly: System.Reflection.AssemblyCopyright("CC 2013 by makovich")]
[assembly: System.Reflection.AssemblyCulture("")]
[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
[assembly: System.Reflection.AssemblyFileVersion("1.0.0.0")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Runtime.InteropServices.Guid("227690f3-6e4c-4225-b3aa-c59f1eccc98d")]

#endregion

namespace TheOnemanCompany.OneFileBlog
{
    using System.IO;
    using System.Text;
    using Nancy;
    using CfgMgr = System.Configuration.ConfigurationManager;

    #region Blog Module

    public class BlogModule : NancyModule
    {
        public BlogModule() : base("/blog")
        {
            Get["/"] = parameters =>
            {
                return "Hello! World? Are you here?" + Cfg.WhatDoIHave();
            };
        }
    }

    #endregion

    #region Blog Configuration

    internal static class Cfg
    {
        public static readonly string PathToBlogRecords = CfgMgr.AppSettings["PathToBlogRecords"];
        public static readonly string PathToImages =      Path.Combine(PathToBlogRecords, CfgMgr.AppSettings["ImagesDirectory"]);
        public static readonly string PathToIndexFile =   Path.Combine(PathToBlogRecords, "index.md");
        public static readonly string BlogTocTitle =      CfgMgr.AppSettings["BlogTocTitle"];
        public static readonly string BootswatchTheme =   CfgMgr.AppSettings["BootswatchTheme"];
        public static readonly string DisqusShortname =   CfgMgr.AppSettings["DisqusShortname"];

        public static string WhatDoIHave()
        {
            var result = new StringBuilder().AppendLine("<hr /><b>web.config : configuration/appSettings</b><br />");

            foreach (var field in typeof(Cfg).GetFields())
            {
                result.AppendLine("{0}: <i>{1}</i><br />".FormatWith(field.Name, field.GetValue(null)));
            }

            return result.ToString();
        }
    }

    #endregion

    #region Utilites

    internal static class StringExtensions
    {
        public static string FormatWith(this string value, params object[] args)
        {
            return string.Format(value, args);
        }
    }

    #endregion
}
