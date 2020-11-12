using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;

namespace SlothEnterprise.ProductApplication
{
    public class ProductApplicationService
    {
        private readonly ApplicationDispatcher _submitter;

        public ProductApplicationService(ISelectInvoiceService selectInvoiceService, IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _submitter = new ApplicationDispatcher(selectInvoiceService, confidentialInvoiceWebService,
                businessLoansService);
        }

        public int SubmitApplicationFor(ISellerApplication application)
        {
            return _submitter.Dispatch(application);
        }

        public static CompanyDataRequest CreateCompanyDataRequest(ISellerCompanyData applicationCompanyData)
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
