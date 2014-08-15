using System.Collections.Generic;
using System.IO;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner;

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
            var import = new ImportMiles();
            import.ImportMilesComplete(cvPartnerToken,new Person("tull","ball"),null,null, null, new List<string>());
        }
    }

   
}
