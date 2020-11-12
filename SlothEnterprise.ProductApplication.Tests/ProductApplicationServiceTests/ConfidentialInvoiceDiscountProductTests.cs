using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using AutoFixture;
using FluentAssertions;
using SlothEnterprise.External;
using SlothEnterprise.External.V1;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests.ProductApplicationServiceTests
{
    public class ConfidentialInvoiceDiscountProductTests : IConfidentialInvoiceService
    {
        private readonly List<Application> _capturedApplications = new List<Application>();
        private readonly Queue<IApplicationResult> _returnResults = new Queue<IApplicationResult>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ShouldSubmitApplicationData()
        {
            var product = _fixture.Create<ConfidentialInvoiceDiscount>();
            var application = new SellerApplication
            {
                Product = product,
                CompanyData = _fixture.Create<SellerCompanyData>()
            };

            SubmitApplication(application);

            _capturedApplications.Should()
                .BeEquivalentTo(new
                {
                    CompanyDataRequest = new
                    {
                        CompanyName = application.CompanyData.Name,
                        CompanyNumber = application.CompanyData.Number,
                        application.CompanyData.DirectorName,
                        CompanyFounded = application.CompanyData.Founded
                    },
                    product.TotalLedgerNetworth,
                    product.AdvancePercentage,
                    product.VatRate
                });
        }

        [Fact]
        public void ShouldReturnApplicationIdWhenApplicationIsSuccessful()
        {
            var application = new SellerApplication
            {
                Product = _fixture.Create<ConfidentialInvoiceDiscount>(),
                CompanyData = _fixture.Create<SellerCompanyData>()
            };
            var expectedApplicationId = _fixture.Create<int>();
            
            _returnResults.Enqueue(new TestApplicationResult
            {
                ApplicationId = expectedApplicationId,
                Success = true
            });

            var applicationId = SubmitApplication(application);

            applicationId.Should().Be(expectedApplicationId);
        }

        private int SubmitApplication(SellerApplication application)
        {
            var productApplicationService = new ProductApplicationService(null, this, null);

            return productApplicationService.SubmitApplicationFor(application);
        }

        public IApplicationResult SubmitApplicationFor(CompanyDataRequest applicantData,
            decimal invoiceLedgerTotalValue,
            decimal advantagePercentage, decimal vatRate)
        {
            _capturedApplications.Add(new Application(applicantData, invoiceLedgerTotalValue, advantagePercentage,
                vatRate));

            return _returnResults.Count > 0
                ? _returnResults.Dequeue() : _fixture.Create<TestApplicationResult>();
        }

        private class Application
        {
            public CompanyDataRequest CompanyDataRequest { get; }
            public decimal TotalLedgerNetworth { get; }
            public decimal AdvancePercentage { get; }
            public decimal VatRate { get; }

            public Application(CompanyDataRequest companyDataRequest, decimal totalLedgerNetworth,
                decimal advancePercentage, decimal vatRate)
                => (CompanyDataRequest, TotalLedgerNetworth, AdvancePercentage, VatRate)
                    = (companyDataRequest, totalLedgerNetworth, advancePercentage, vatRate);
        }

        public class TestApplicationResult : IApplicationResult
        {
            public int? ApplicationId { get; set; }
            public bool Success { get; set; }
            public IList<string> Errors { get; set; }
        }
    }
}