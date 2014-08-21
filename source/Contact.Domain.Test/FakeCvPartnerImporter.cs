using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Test
{
    public class FakeCvPartnerImporter : IImportDataFromCvPartner
    {
        public async Task<List<CvPartnerImportData>> GetImportData()
        {
            return new List<CvPartnerImportData>();
        }
    }
}
