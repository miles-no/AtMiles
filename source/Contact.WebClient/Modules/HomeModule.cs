using Nancy;

namespace Contact.WebClient.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {

            Get["/"] = paramters => { return View["Index.cshtml"]; };

        }

    }
}