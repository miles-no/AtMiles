using System.Collections.Generic;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Test
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
            return await Task.Run(() => _importData);
        }
    }
}
