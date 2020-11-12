using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests.ProductApplicationServiceTests
{
    public class SelectiveInvoiceDiscountProductTests : ISelectInvoiceService
    {
        private readonly List<Application> _capturedApplications = new List<Application>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ShouldSubmitApplicationData()
        {
            var product = _fixture.Create<SelectiveInvoiceDiscount>();
            var application = new SellerApplication
            {
                Product = product,
                CompanyData = _fixture.Create<SellerCompanyData>()
            };
            var productApplicationService = new ProductApplicationService(this, null, null);

            productApplicationService.SubmitApplicationFor(application);

            _capturedApplications.Should()
                .BeEquivalentTo(new Application(
                    application.CompanyData.Number.ToString(CultureInfo.InvariantCulture),
                    product.InvoiceAmount,
                    product.AdvancePercentage
                ));
        }

        public int SubmitApplicationFor(string companyNumber, decimal invoiceAmount, decimal advancePercentage)
        {
            _capturedApplications.Add(new Application(companyNumber, invoiceAmount, advancePercentage));

            return _fixture.Create<int>();
        }

        private class Application
        {
            public string CompanyNumber { get; }
            public decimal InvoiceAmount { get; }
            public decimal AdvancePercentage { get; }

            public Application(string companyNumber, decimal invoiceAmount, decimal advancePercentage)
                => (CompanyNumber, InvoiceAmount, AdvancePercentage) =
                    (companyNumber, invoiceAmount, advancePercentage);
        }
    }
}