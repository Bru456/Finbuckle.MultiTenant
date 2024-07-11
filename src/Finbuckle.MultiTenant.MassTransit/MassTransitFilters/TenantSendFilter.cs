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
    public class TenantSendFilter<T>(
        IMultiTenantContextAccessor<TenantInfo> mtca,
        ITenantHeaderConfiguration thc
        ) 
        : IFilter<SendContext<T>> where T : class
    {
        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("tenantPublishFilter");
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (mtca.MultiTenantContext?.TenantInfo is null) return next.Send(context);

            context.Headers.Set(thc.TenantIdentifierHeaderKey, mtca.MultiTenantContext.TenantInfo.Identifier, false);

            return next.Send(context);
        }
    }
}
