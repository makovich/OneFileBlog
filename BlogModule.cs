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
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using Nancy;
    using Nancy.Responses;
    using CfgMgr = System.Configuration.ConfigurationManager;

    #region Blog Module

    public class BlogModule : NancyModule
    {
        public BlogModule() : base("/blog")
        {
            Before += context =>
            {
                var requestPath = context.Request.Path.Split('/').Last();

                if (requestPath.EndsWithAny(".png", ".gif", ".jpg"))
                {
                    var imageFilePath = Path.Combine(Cfg.PathToImages, requestPath);

                    return new GenericFileResponse(imageFilePath);
                }

                return null;
            };

            Get["/"] = parameters =>
            {
                return new BlogRecordResponse(Cfg.PathToIndexFile, "All posts goes here", false);
            };

            Get["/{record}"] = parameters =>
            {
                var blogRecordFilePath = Path.Combine(Cfg.PathToBlogRecords, parameters.record + ".md");

                return new BlogRecordResponse(blogRecordFilePath, parameters.record);
            };
        }

        #region BlogRecordResponse

        private class BlogRecordResponse : GenericFileResponse
        {
            public BlogRecordResponse(string recordFilePath, string title, bool allowDisqus = true)
                : base(recordFilePath)
            {
                if (StatusCode == HttpStatusCode.OK)
                {
                    EnrichContentWithTitle(title);
                    EnrichContentWithJavaScript(allowDisqus);
                }
            }

            private void EnrichContentWithTitle(string title)
            {
                var titleText = title.Contains("-")
                    ? title.FromHyphensToSentense()
                    : title.FromPascalCaseToSentence();

                var wrappedTitle = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><title>{0} - {1}</title>"
                    .FormatWith(Cfg.BlogTocTitle, titleText);

                RewriteContent(wrappedTitle);
            }

            private void EnrichContentWithJavaScript(bool allowDisqus)
            {
                var scriptText = allowDisqus && Cfg.DisqusShortname.NotEmpty()
                    ? ReadJavaScriptFromResources("strapdown-loader-disqus.js").Replace("{{DISQUS_SHORTNAME_MARKER}}", Cfg.DisqusShortname)
                    : ReadJavaScriptFromResources("strapdown-loader.js");

                if (scriptText.IsNullOrWhiteSpace())
                    return;

                var wrappedScript = "<script type=\"text/javascript\" theme=\"{1}\">{0}</script>"
                    .FormatWith(scriptText, Cfg.BootswatchTheme);

                RewriteContent(wrappedScript);
            }

            private string ReadJavaScriptFromResources(string filename)
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                var resourceUri = currentAssembly.GetManifestResourceNames().First(x => x.EndsWith(filename));

                using (var stream = currentAssembly.GetManifestResourceStream(resourceUri))
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            private void RewriteContent(string addend)
            {
                using (var buffer = new MemoryStream())
                using (var reader = new StreamReader(buffer))
                {
                    Contents.Invoke(buffer);
                    buffer.Position = 0;

                    var responseContent = reader.ReadToEnd().Insert(0, addend);

                    ContentType = "text/html";
                    Contents = stream =>
                    {
                        stream.Write(Encoding.Default.GetBytes(responseContent), 0, responseContent.Length);
                    };
                }
            }
        }

        #endregion
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

        public static string FromPascalCaseToSentence(this string value)
        {
            return Regex.Replace(value, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }

        public static string FromHyphensToSentense(this string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Replace('-', ' ').Substring(1);
        }

        public static bool EndsWithAny(this string value, params string[] patterns)
        {
            foreach (var item in patterns)
            {
                if (value.EndsWith(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static bool NotEmpty(this string value)
        {
            return !value.IsNullOrWhiteSpace();
        }
    }

    #endregion
}
