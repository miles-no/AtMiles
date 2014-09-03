using System;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;

namespace Contact.WebClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://+:80";
            using (WebApp.Start<Startup>(url))
            {

                BundleConfig.RegisterBundles();
                StaticConfiguration.DisableErrorTraces = false;
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
            app.UseStaticFiles("/Content");
            app.UseStaticFiles("/Content/foundation");
          
            app.UseStaticFiles("/scripts");
            
            app.UseStaticFiles("/scripts/foundation");
            app.UseStaticFiles("/scripts/app");

            app.UseNancy(); 
        }
    }
}
