using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public interface IImportDataFromCvPartner
    {
        Task<List<CvPartnerImportData>> GetImportData();
    }
}
