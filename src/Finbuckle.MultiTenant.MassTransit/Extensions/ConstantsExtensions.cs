// Copyright Finbuckle LLC, Andrew White, and Contributors.
// Refer to the solution LICENSE file for more information.

namespace Finbuckle.MultiTenant.Internal;

internal static class ConstantsExtensions
{
    /// <summary>
    /// Default MassTransit header key used to identify tenants. Defaults to: 'X-Tenant-Identifier'
    /// </summary>
    public static readonly string MassTransitTenantHeader = "X-Tenant-Identifier";
}