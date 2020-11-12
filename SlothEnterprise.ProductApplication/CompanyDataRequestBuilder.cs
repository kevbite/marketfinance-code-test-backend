using SlothEnterprise.External;
using SlothEnterprise.ProductApplication.Applications;

namespace SlothEnterprise.ProductApplication
{
    public static class CompanyDataRequestBuilder
    {
        public static CompanyDataRequest Create(ISellerCompanyData applicationCompanyData)
        {
            return new CompanyDataRequest
            {
                CompanyFounded = applicationCompanyData.Founded,
                CompanyNumber = applicationCompanyData.Number,
                CompanyName = applicationCompanyData.Name,
                DirectorName = applicationCompanyData.DirectorName
            };
        }
    }
}