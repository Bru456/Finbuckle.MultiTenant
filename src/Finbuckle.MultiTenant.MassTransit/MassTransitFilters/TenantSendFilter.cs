using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.MassTransit.Internal;

using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.MassTransit.MassTransitFilters
{
    /// <summary>
    /// <see href="https://masstransit.io/">MassTransit</see> <see href="https://masstransit.io/documentation/configuration/middleware/scoped">Scoped Filter</see> to set the 
    /// <see cref="ITenantInfo.Identifier"/> to a header within the MassTransit message.
    /// This is scoped to the SendContext.
    /// </summary>
    /// <typeparam name="T">The Type of the Sender</typeparam>
    /// <param name="tenantResolver">Injected via Dependency Injection. <see cref="ITenantResolver"/>.</param>
    /// <param name="mtcSetter">Injected via Dependency Injection. <see cref="IMultiTenantContextSetter"/></param>
    /// <example> 
    /// <code>
    /// <![CDATA[ builder.Services.AddMassTransit(x =>
    ///        {
    ///            x.UsingInMemory((context, cfg) =>
    ///            {
    ///                cfg.UseSendFilter(typeof(TenantPublishFilter<>), context);
    ///                cfg.ConfigureEndpoints(context);
    ///            });
    ///        });]]>
    /// </code>
    /// </example>
    public class TenantSendFilter<T>(
        IMultiTenantContextAccessor<TenantInfo> mtca,
        ITenantHeaderConfiguration thc
        ) 
        : IFilter<SendContext<T>> where T : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenantSendFilter");
        }

        /// <summary>
        /// If configured, called by MassTransit during the message processing pipeline and sets the current tenant to a header in the MassTransit message from the Finbukcle.MultiTenant. Loosely based on Finbuckle.MultiTenant.AspNetCore.Internal.MultiTenantMiddleware.
        /// </summary>
        /// <param name="context">Current MassTransit Send Context.</param>
        /// <param name="next">The next step in the MassTransit pipeline.</param>
        /// <returns></returns>
        /// <remarks>The idea here is that MassTransit calls this as part of its own middleware so we in effect embed Finbuckle Tenant Resolving capabilities into the MassTransit middleware.</remarks>
        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (mtca.MultiTenantContext?.TenantInfo is null) return next.Send(context);

            context.Headers.Set(thc.TenantIdentifierHeaderKey, mtca.MultiTenantContext.TenantInfo.Identifier, false);

            return next.Send(context);
        }
    }
}
