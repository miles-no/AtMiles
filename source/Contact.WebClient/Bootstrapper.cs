using System.IO;
using Nancy;

namespace Contact.WebClient
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {

#if DEBUG
        protected override IRootPathProvider RootPathProvider
        {
            get { return new DebugRootPathProvider(); }
        }
#endif
#if !DEBUG
        protected override IRootPathProvider RootPathProvider
        {
            get { return new ReleaseRootPathProvider(); }
        }
#endif

        public class DebugRootPathProvider : IRootPathProvider
        {
            public string GetRootPath()
            {
                return Directory.GetCurrentDirectory() + "\\..\\..";
            }
        }

        public class ReleaseRootPathProvider : IRootPathProvider
        {
            public string GetRootPath()
            {
                return Directory.GetCurrentDirectory();
            }
        }
    }
}