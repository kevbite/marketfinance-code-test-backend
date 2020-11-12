using System.Collections.Generic;
using System.Globalization;
using System.Threading;
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
        private readonly Queue<int> _returnApplicationIds = new Queue<int>();
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
            
            SubmitApplication(application);

            _capturedApplications.Should()
                .BeEquivalentTo(new Application(
                    application.CompanyData.Number.ToString(CultureInfo.InvariantCulture),
                    product.InvoiceAmount,
                    product.AdvancePercentage
                ));
        }
        
        [Fact]
        public void ShouldReturnCorrectApplicationId()
        {
            var application = new SellerApplication
            {
                Product = _fixture.Create<SelectiveInvoiceDiscount>(),
                CompanyData = _fixture.Create<SellerCompanyData>()
            };
            var expectedApplicationId = _fixture.Create<int>();
            _returnApplicationIds.Enqueue(expectedApplicationId);
            
            var applicationId = SubmitApplication(application);
            
            applicationId.Should().Be(expectedApplicationId);
        }
        

        private int SubmitApplication(SellerApplication application)
        {
            var productApplicationService = new ProductApplicationService(this, null, null);

            var applicationId = productApplicationService.SubmitApplicationFor(application);
            return applicationId;
        }

        
        [Fact]
        public void ShouldSubmitCompanyNumberFormattedCorrectly()
        {
            var testCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            testCulture.NumberFormat.NegativeSign = "!";
            Thread.CurrentThread.CurrentCulture = testCulture;
            
            var application = new SellerApplication
            {
                Product = _fixture.Create<SelectiveInvoiceDiscount>(),
                CompanyData = _fixture.Build<SellerCompanyData>()
                    .With(x => x.Number, -123456)
                    .Create()
            };
            
            SubmitApplication(application);

            _capturedApplications.Should()
                .BeEquivalentTo(new
                {
                    CompanyNumber = "-123456"
                });
        }

        public int SubmitApplicationFor(string companyNumber, decimal invoiceAmount, decimal advancePercentage)
        {
            _capturedApplications.Add(new Application(companyNumber, invoiceAmount, advancePercentage));

            return _returnApplicationIds.Count > 0
                ? _returnApplicationIds.Dequeue() : _fixture.Create<int>();
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