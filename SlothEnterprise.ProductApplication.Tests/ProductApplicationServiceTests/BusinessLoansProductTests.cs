using System.Collections.Generic;
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
    public class BusinessLoansProductTests : IBusinessLoansService
    {
        private readonly List<Application> _capturedApplications = new List<Application>();
        private readonly Queue<IApplicationResult> _returnResults = new Queue<IApplicationResult>();
        private readonly Fixture _fixture = new Fixture();

        [Fact]
        public void ShouldSubmitApplicationData()
        {
            var product = _fixture.Create<BusinessLoans>();
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
                    LoansRequest = new
                    {
                        product.InterestRatePerAnnum,
                        product.LoanAmount
                    }
                });
        }


        [Fact]
        public void ShouldReturnApplicationIdWhenApplicationIsSuccessful()
        {
            var expectedApplicationId = _fixture.Create<int>();

            _returnResults.Enqueue(new TestApplicationResult
            {
                ApplicationId = expectedApplicationId,
                Success = true
            });

            var applicationId = SubmitApplication();

            applicationId.Should().Be(expectedApplicationId);
        }

        [Fact]
        public void ShouldReturnMinusOneWhenApplicationIsSuccessfulButNoApplicationIdIsReturned()
        {
            _returnResults.Enqueue(new TestApplicationResult
            {
                ApplicationId = null,
                Success = true
            });

            var applicationId = SubmitApplication();

            applicationId.Should().Be(-1);
        }

        [Fact]
        public void ShouldReturnMinusOneWhenApplicationIsUnsuccessful()
        {
            _returnResults.Enqueue(new TestApplicationResult
            {
                ApplicationId = _fixture.Create<int>(),
                Success = false
            });

            var applicationId = SubmitApplication();

            applicationId.Should().Be(-1);
        }

        private int SubmitApplication(ISellerApplication application)
        {
            var productApplicationService = new ProductApplicationService(null, null, this);

            return productApplicationService.SubmitApplicationFor(application);
        }

        private int SubmitApplication()
        {
            var application = new SellerApplication
            {
                Product = _fixture.Create<BusinessLoans>(),
                CompanyData = _fixture.Create<SellerCompanyData>()
            };

            return SubmitApplication(application);
        }


        private class Application
        {
            public CompanyDataRequest CompanyDataRequest { get; }
            public LoansRequest LoansRequest { get; }

            public Application(CompanyDataRequest companyDataRequest, LoansRequest loansRequest)
                => (CompanyDataRequest, LoansRequest)
                    = (companyDataRequest, loansRequest);
        }

        public IApplicationResult SubmitApplicationFor(CompanyDataRequest applicantData, LoansRequest businessLoans)
        {
            _capturedApplications.Add(new Application(applicantData, businessLoans));

            return _returnResults.Count > 0
                ? _returnResults.Dequeue()
                : _fixture.Create<TestApplicationResult>();
        }
    }
}