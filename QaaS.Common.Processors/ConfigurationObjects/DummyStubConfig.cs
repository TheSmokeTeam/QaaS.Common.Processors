using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record DummyStubConfig
{
    [Required, Description("Dummy Json Body Key")]
    public string DummyKey { get; set; } = string.Empty;

    [Required, Description("Dummy Json Body Value")]
    public string DummyValue { get; set; } = string.Empty;
}
