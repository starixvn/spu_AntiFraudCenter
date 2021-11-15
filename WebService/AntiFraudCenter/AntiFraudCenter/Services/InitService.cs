namespace AntiFraudCenter
{
    using AntiFraudCenterService.Features.FraudCustomer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

    public static class InitService
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterInitService(services);
            RegisterFraudCustomerService(services);
        }

        private static void RegisterInitService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private static void RegisterFraudCustomerService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IFraudCustomerDataAccess, FraudCustomerDataAccess>();
            services.AddSingleton<IFraudCustomerDBService, FraudCustomerDBService>();
            services.AddSingleton<IFraudCustomerService, FraudCustomerService>();
        }
    }
}