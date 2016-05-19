using BluApi.Chef.ChefAPI;
using BluApi.Common;
using ReturnType = BluApi.Common.Function;

namespace Blu
{
    public static partial class Method
    {
        public static void ConfigureApiClient()
        {
            ChefConfigurator chefConfigurator = new ChefConfigurator();
            chefConfigurator.LoadConfig();
            Logger.log("ok", "API Client is configured.");
        }
    }
}
