using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.MassTransit.Internal;

using MassTransit;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.MassTransit.Strategies
{
    public class MassTransitHeaderStrategy : IMultiTenantStrategy
    {
        private readonly ITenantHeaderConfiguration _config;
        public MassTransitHeaderStrategy(ITenantHeaderConfiguration headerKey) 
        {
            _config = headerKey;
        }

        public Task<string?> GetIdentifierAsync(object context)
        {
            string? header = null;

            if (!(context is ConsumeContext consumeContext))
                throw new MultiTenantException(null,
                    new ArgumentException($"\"{nameof(context)}\" type must be of type ConsumeContext", nameof(context)));

            if (consumeContext.Headers.TryGetHeader(_config.TenantIdentifierHeaderKey, out var tenantId))
                {
                    header = tenantId as string;
                }

            return Task.FromResult(header);
        }
    }
}
