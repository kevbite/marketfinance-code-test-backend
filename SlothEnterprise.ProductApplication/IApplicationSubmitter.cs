using SlothEnterprise.ProductApplication.Applications;
using SlothEnterprise.ProductApplication.Products;

namespace SlothEnterprise.ProductApplication
{
    public interface IApplicationSubmitter<in T> where T: IProduct
    {
        int Submit(ISellerApplication application, T product);
    }
}