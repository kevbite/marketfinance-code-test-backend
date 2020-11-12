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
                CompanyDataRequestBuilder.Create(application.CompanyData), new LoansRequest
                {
                    InterestRatePerAnnum = product.InterestRatePerAnnum,
                    LoanAmount = product.LoanAmount
                });
            
            return ApplicationResultEvaluator.Evaluate(result);
        }
    }
}