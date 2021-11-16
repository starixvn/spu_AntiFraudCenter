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
            RegisterParticipantService(services);
        }

        private static void RegisterInitService(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        private static void RegisterFraudCustomerService(this IServiceCollection services)
        {
            services.AddSingleton<IFraudCustomerDataAccess, FraudCustomerDataAccess>();
            services.AddSingleton<IFraudCustomerDBService, FraudCustomerDBService>();
            services.AddSingleton<IFraudCustomerService, FraudCustomerService>();
        }

        private static void RegisterParticipantService(this IServiceCollection services)
        {
            services.AddSingleton<IParticipantInfoDataAccess, ParticipantInfoDataAccess>();
            services.AddSingleton<IParticipantInfoDBService, ParticipantInfoDBService>();
            services.AddSingleton<IParticipantService, ParticipantService>();
        }
    }
}