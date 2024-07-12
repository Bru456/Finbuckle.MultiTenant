
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.MassTransit.MassTransitFilters;

using MassTransit;

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
                .WithRouteStrategy()
                .WithHeaderStrategy("tenant")
                .WithMassTransitHeaderStrategy();
                //.WithMassTransitHeaderStrategy("tenantIdentifier"); // Only required if wanting to use MassTransit.
            //.WithBasePathStrategy();
            

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>(); // The MassTransit Consumer that will be used to consume messages.
                x.UsingInMemory((context, cfg) => //using in memory for simplicity. Please replace with your preferred transport method.
                {
                    cfg.UseConsumeFilter(typeof(TenantConsumeFilter<>), context); // Required if wanting to have a MassTransit Consumer and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context); // Required if wanting to have a MassTransit Publisher and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                    cfg.UseSendFilter(typeof(TenantPublishFilter<>), context); // Required if wanting to have a MassTransit Sender and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
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
