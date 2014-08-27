using Nancy;

namespace JustForFun2.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {

            Get["/"] = paramters => { return View["Index.cshtml"]; };

        }

    }
}