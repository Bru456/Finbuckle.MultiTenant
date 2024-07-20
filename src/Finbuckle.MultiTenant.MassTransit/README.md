# Finbuckle.MultiTenant.MassTransit
This package provides a way to seamlessly use the Finbuckle.MultiTenant library with MassTransit.

It was built from issue [853](https://github.com/Finbuckle/Finbuckle.MultiTenant/issues/853) in Finbuckle.MultiTenant library.

> [!WARNING]  
> No official support is provided by Finbuckle. Use at your own risk. Any issues should be reported to the GitHub repository.

## Installation

There is no Nuget for this package currently. There are multiple ways to use this package:
- Clone the repository and build the project locally and add a project Assembly reference to the DLL (recommended).
- Clone the repository locally and add the project to your solution to be built as part of your solution.

### Clone and Build

```bash
git clone https://XXXXX
cd Finbuckle.MultiTenant.MassTransit
dotnet build Finbuckle.MultiTenant.MassTransit
```

Then in your project, add a reference to the DLL. No CLI command is available for this, so you will need to manually add the reference to your project file.

```xml
<ItemGroup>
  <Reference Include="Finbuclke.MultiTenant.MassTransit">
    <HintPath>.\<PathToDll>\Finbuclke.MultiTenant.MassTransit.dll</HintPath>
  </Reference>
</ItemGroup>
```

Or you can add the DLL via Visual Studio by right-clicking on the project and selecting Add Reference. Then browse to the DLL and add it.

### Clone and Add to Solution

```bash
git clone https://XXXXX
```

Then in your solution, right-click on the solution and select Add -> Existing Project. Then browse to the `Finbuckle.MultiTenant.MassTransit` folder and select the `.csproj` file.

## Configuration

> [!IMPORTANT]  
> The Finbuckle.MultiTenant library must be setup and configured before using this package. This package is an extension to the Finbuckle.MultiTenant library and will not work without it.

The package provides MassTransit filters in order to add the Tenant Identifier into the messages header. 
A Finbukcle.MultiTenant Strategy exists to retrieve determine the Tenant Context base on the message header.

The order of the builder configuration is important. Ensure the Finbuckle.MultiTenant library must be configured before the MassTransit filters are added.

A Finbuckle.MultiTenant Strategy is provided to resolve the Tenant Context from the message header. This strategy is required to be used in the MultiTenantBuilder to use the MassTransit filters.

Example of the configuration with both the ASP.Net Header Strategy and the MassTransit Header Strategy:

```csharp
builder.Services.AddMultiTenant<TenantInfo>()
                .WithConfigurationStore()
                .WithHeaderStrategy("tenant")
                .WithMassTransitHeaderStrategy();
```

Five MassTransit filters are provided:

| Filter | Description | Requires Strategy |
| --- | --- | --- |
| TenantConsumeFilter | Resolves the Tenant Context from the Tenant Identifier in the message header.  If not present the tenant is not resolved, but the message is still passed to the consumer. | Yes |
| TenantPublishFilter | Adds the Tenant Identifier to the message header based on the current Tenant Context.  If no currently resolved Tenant, header is not added but message is published.  | No |
| TenantSendFilter | Adds the Tenant Identifier to the message header based on the current Tenant Context.  If no currently resolved Tenant, header is not added but message is sent. | No |
| TenantCompensateFilter | Resolves the Tenant Context from the Tenant Identifier in the message header.  If not present the tenant is not resolved, but the compensate activity is still triggered. | Yes |
| TenantExecuteFilter | Resolves the Tenant Context from the Tenant Identifier in the message header.  If not present the tenant is not resolved, but the execute activity is still triggered. | Yes |




The filters can be added individually to the MassTransit bus configuration or from an Extension method to add them all at once:

Example of adding the filters from the extension method:

```csharp
builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>(); // The MassTransit Consumer that will be used to consume messages.

                x.UsingInMemory((IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg) => //using in memory for simplicity. Please replace with your preferred transport method.
                {
                    cfg.AddTenantFilters(context); // Required if wanting to have a MassTransit Consumer and maintain tenant context. To use this filter, .WithMassTransitHeaderStrategy() must be called in the MultiTenantBuilder.
                    cfg.ConfigureEndpoints(context);
                });
            });
```

Example of adding the filters individually, you only need to use the ones you will use:

```csharp
builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>(); // The MassTransit Consumer that will be used to consume messages.
                cfg.AddActivity<YourExecuteActivity, YourExecuteArguments, YourExecuteLog>();
                cfg.AddActivity<YourExecuteActivityThatFails, YourExecuteArguments, YourExecuteLog>();

                x.UsingInMemory((IBusRegistrationContext context, IInMemoryBusFactoryConfigurator cfg) => //using in memory for simplicity. Please replace with your preferred transport method.
                {
                    cfg.UseConsumeFilter(typeof(TenantConsumeFilter<>), context); 
                    cfg.UsePublishFilter(typeof(TenantPublishFilter<>), context);
                    cfg.UseSendFilter(typeof(TenantPublishFilter<>), context);
                    cfg.UseCompensateActivityFilter(typeof(TenantCompensateFilter<>), context);
                    cfg.UseExecuteActivityFilter(typeof(TenantExecuteFilter<>), context);
                    cfg.ConfigureEndpoints(context);
                });

            });
```

A complete example can be found in: 
- The `tests` folder under the `Finbuckle.MultiTenant.MassTransit.Tests` project in the `MassTransitTests` class.
- The `samples` folder under the `MassTransitApi` project in the `Program` class.

## Usage

A working example can be found in the `samples` folder under the `MassTransitApi` project. This project is a simple API that uses MassTransit to send and receive messages. The API is setup to use the Finbuckle.MultiTenant library and the Finbuckle.MultiTenant.MassTransit package to maintain tenant context.

To use this example, set it as the startup project and run the project. The API will be hosted on `https://localhost:7244` and the Swagger UI will be available at `https://localhost:7244/swagger/index.html`.

Use postman or another tool to send a GET request to `https://localhost:7244/TestBus` with the following header `tenant: tenant1`. This will send a message to the bus with the tenant header. The consumer will receive the message and log the tenant context.

## License

This project is licensed under the [Apache 2.0 license](https://www.apache.org/licenses/LICENSE-2.0) - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Thanks to:
- [@AndrewTriesToCode](https://github.com/AndrewTriesToCode)
- [@fbjerggaard](https://github.com/fbjerggaard)
- [@Bru456](https://github.com/Bru456)

For their help in building this package.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.
