using System.Collections.Generic;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain
{
    public interface IImportDataFromCvPartner
    {
        Task<List<CvPartnerImportData>> GetImportData();
    }
}
