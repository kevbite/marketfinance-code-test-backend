using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public class ConfidentialInvoiceDiscountSubmitter : IApplicationSubmitter<ConfidentialInvoiceDiscount>
    {
        private readonly IConfidentialInvoiceService _confidentialInvoiceWebService;

        public ConfidentialInvoiceDiscountSubmitter(IConfidentialInvoiceService confidentialInvoiceWebService)
            => _confidentialInvoiceWebService = confidentialInvoiceWebService;

        public int Submit(ISellerApplication application, ConfidentialInvoiceDiscount product)
        {
            var result = _confidentialInvoiceWebService.SubmitApplicationFor(
                CompanyDataRequestBuilder.Create(application.CompanyData),
                product.TotalLedgerNetworth,
                product.AdvancePercentage,
                product.VatRate);

            return ApplicationResultEvaluator.Evaluate(result);
        }
    }
}