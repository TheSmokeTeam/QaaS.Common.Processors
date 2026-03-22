using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using QaaS.Framework.SDK.ContextObjects;

namespace QaaS.Common.Processors.Tests;

public static class Globals
{
    public static readonly Context Context = new()
    {
        Logger = NullLogger.Instance,
        RootConfiguration = new ConfigurationBuilder().Build()
    };
}
