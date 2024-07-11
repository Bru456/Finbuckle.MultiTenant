using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finbuckle.MultiTenant.MassTransit.Internal
{
    /// <summary>
    /// Used in MassTransit filters to configure the header keys used to identify tenants.
    /// Also used in the MassTransit strategy to get the tenant identifier from the headers.
    /// </summary>
    public class TenantHeaderConfiguration : ITenantHeaderConfiguration
    {
        public string TenantIdentifierHeaderKey { get; private set; }

        public TenantHeaderConfiguration( string tenantIdentifierHeaderKey)
        {
            
            TenantIdentifierHeaderKey = tenantIdentifierHeaderKey;
        }
    }
}
