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
    using Nancy;

    public class BlogModule : NancyModule
    {
        public BlogModule() : base("/blog")
        {
            Get["/"] = parameters =>
            {
                return "Hello! World? Are you here?";
            };
        }
    }
}
