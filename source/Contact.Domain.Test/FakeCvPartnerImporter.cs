using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Test
{
    public class FakeCvPartnerImporter : IImportDataFromCvPartner
    {
        private readonly List<CvPartnerImportData> _importData;
        public FakeCvPartnerImporter(List<CvPartnerImportData> importData)
        {
            _importData = importData;
        }
        public async Task<List<CvPartnerImportData>> GetImportData()
        {
            return _importData;
        }
    }
}
