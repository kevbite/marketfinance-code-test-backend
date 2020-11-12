using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class BusinessLoansSubmitter : IApplicationSubmitter<BusinessLoans>
    {
        private readonly IBusinessLoansService _service;

        public BusinessLoansSubmitter(IBusinessLoansService service)
            => _service = service;

        public int Submit(ISellerApplication application, BusinessLoans product)
        {
            var result = _service.SubmitApplicationFor(
                ProductApplicationService.CreateCompanyDataRequest(application.CompanyData), new LoansRequest
                {
                    InterestRatePerAnnum = product.InterestRatePerAnnum,
                    LoanAmount = product.LoanAmount
                });
            return result.Success ? result.ApplicationId ?? -1 : -1;
        }
    }
}