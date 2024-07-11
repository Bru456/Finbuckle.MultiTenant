using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.Events;

using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.MassTransit.MassTransitFilters
{
    public class TenantConsumeFilter<T> (
        ITenantResolver tenantResolver,
        IMultiTenantContextSetter mtcSetter
        ) 
        : IFilter<ConsumeContext<T>> 
        where T : class
    {
       

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenantConsumeFilter");
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            IMultiTenantContext? multiTenantContext = await tenantResolver.ResolveAsync(context);
            mtcSetter.MultiTenantContext = multiTenantContext;

            await next.Send(context);
        }
    }
}
