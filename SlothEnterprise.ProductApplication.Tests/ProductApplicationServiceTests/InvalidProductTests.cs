using System;
using FluentAssertions;
using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;
using Xunit;

namespace SlothEnterprise.ProductApplication.Tests.ProductApplicationServiceTests
{
    public class InvalidProductTests
    {
        [Fact]
        public void ShouldThrowInvalidOperationExceptionOnUnsupportedProduct()
        {
            var service = new ProductApplicationService(null,null,null);

            Action action = () =>  service.SubmitApplicationFor(new SellerApplication
            {
                Product = new UnsupportedTestProduct()
            });

            action.Should().Throw<InvalidOperationException>();
        }
        
        private class UnsupportedTestProduct : IProduct
        {
            public int Id { get; }
        }
    }
}