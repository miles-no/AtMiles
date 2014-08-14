using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner;
using Contact.Import.CvPartner.CvPartner.Models.Cv;
using Contact.Import.CvPartner.CvPartner.Models.Employee;
using Contact.Infrastructure;
using Newtonsoft.Json;

namespace Contact.Import.CvPartner
{
    class Program
    {
        static void Main(string[] args)
        {
            string cvPartnerToken = null;
#if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
#endif
            var import = new ImportMiles(new CommandSenderMock());
            import.ImportMilesComplete(cvPartnerToken,new Person("tull","ball"));
        }
    }

   
}
