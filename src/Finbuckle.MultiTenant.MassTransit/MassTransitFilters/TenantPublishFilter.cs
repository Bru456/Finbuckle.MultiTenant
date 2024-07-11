using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.MassTransit.Internal;
using Finbuckle.MultiTenant.MassTransit.Strategies;

using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.MassTransit.MassTransitFilters
{
    public class TenantPublishFilter<T>(
        IMultiTenantContextAccessor<TenantInfo> mtca,
        ITenantHeaderConfiguration thc
        ) 
        : IFilter<PublishContext<T>> 
        where T : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenantPublishFilter");
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            if (mtca.MultiTenantContext?.TenantInfo is null) return next.Send(context);

            //context.Headers.Set("tenantId", mtca.MultiTenantContext.TenantInfo.Id, false);
            context.Headers.Set(thc.TenantIdentifierHeaderKey, mtca.MultiTenantContext.TenantInfo.Identifier, false);

            return next.Send(context);
        }
    }
}
