
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
                .WithMassTransitHeaderStrategy("tenantIdentifier");
            //.WithBasePathStrategy();
            

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>();
                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseConsumeFilter(typeof(TenantConsumeFilter<>), context);
                    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context);
                    cfg.UseSendFilter(typeof(TenantPublishFilter<>), context);
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
