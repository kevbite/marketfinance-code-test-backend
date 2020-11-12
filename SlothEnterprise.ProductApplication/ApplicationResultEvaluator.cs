using SlothEnterprise.External;

namespace SlothEnterprise.ProductApplication
{
    public static class ApplicationResultEvaluator
    {
        public static int Evaluate(IApplicationResult result)
        {
            return result.Success ? result.ApplicationId ?? -1 : -1;
        }
    }
}