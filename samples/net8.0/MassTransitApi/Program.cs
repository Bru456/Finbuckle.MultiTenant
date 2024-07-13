
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.MassTransit.MassTransitFilters;

using MassTransit;
using MassTransit.Configuration;

using MassTransitApi.Consumers;

//using MassTransitApi.Consumers;

namespace MassTransitApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddLogging();

            builder.Services.AddMultiTenant<TenantInfo>()
                .WithConfigurationStore()
                .WithHeaderStrategy("tenant")
                .WithMassTransitHeaderStrategy();
            

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>(); // The MassTransit Consumer that will be used to consume messages.

                // You can use the following code to add filters to the MassTransit pipeline.
                // This is individual filters for each type of MassTransit action.
                // This is not required if you use the AddTenantFilters extension method. See uncommented section below.

                //x.UsingInMemory((IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg) => //using in memory for simplicity. Please replace with your preferred transport method.
                //{
                //    cfg.UseConsumeFilter(typeof(TenantConsumeFilter<>), context); // Required if wanting to have a MassTransit Consumer and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                //    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context); // Required if wanting to have a MassTransit Publisher and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                //    cfg.UseSendFilter(typeof(TenantPublishFilter<>), context); // Required if wanting to have a MassTransit Sender and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                //    cfg.ConfigureEndpoints(context);
                //});

                // This is a single add command that can be used to apply all FinBuckle.MultiTenant filters to the MassTransit pipeline.
                x.UsingInMemory((IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg) => //using in memory for simplicity. Please replace with your preferred transport method.
                {
                    cfg.AddTenantFilters(context); // Required if wanting to have a MassTransit Consumer and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMultiTenant();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
