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
    /// <summary>
    /// The strategy to identify the Tenant from the MassTransit header. Used only as part of MassTransit Consumer. 
    /// </summary>
    public class MassTransitHeaderStrategy : IMultiTenantStrategy
    {
        private readonly ITenantHeaderConfiguration _config;
        public MassTransitHeaderStrategy(ITenantHeaderConfiguration headerKey) 
        {
            _config = headerKey;
        }

        /// <summary>
        /// Get the Tenant identifier from the MassTransit header.
        /// </summary>
        /// <param name="context">MassTransits <see cref="ConsumeContext"/></param>
        /// <returns>The Tenant Identifier if found otherwise null</returns>
        /// <exception cref="MultiTenantException">Maintaining current process of erroring if Context does not match the expected type.</exception>
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
