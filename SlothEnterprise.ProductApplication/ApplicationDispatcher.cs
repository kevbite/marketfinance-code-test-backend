using System;
using System.Collections.Generic;
using System.Linq;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;

namespace SlothEnterprise.ProductApplication
{
    public class ApplicationDispatcher
    {
        private readonly List<object> _submitters;

        public ApplicationDispatcher(ISelectInvoiceService selectInvoiceService,
            IConfidentialInvoiceService confidentialInvoiceWebService, IBusinessLoansService businessLoansService)
        {
            _submitters = new List<object>
            {
                new SelectiveInvoiceDiscountSubmitter(selectInvoiceService),
                new ConfidentialInvoiceDiscountSubmitter(confidentialInvoiceWebService),
                new BusinessLoansSubmitter(businessLoansService)
            };
        }
        public int Dispatch(ISellerApplication application)
        {
            var submitterType = typeof(IApplicationSubmitter<>)
                .MakeGenericType(application.Product.GetType());
            
            var submitter = _submitters
                                .SingleOrDefault(x => x.GetType().GetInterfaces().Any<Type>(t => t == submitterType))
                            ?? throw new InvalidOperationException();

            return ((dynamic)submitter).Submit(application, (dynamic)application.Product);
        }
    }
}