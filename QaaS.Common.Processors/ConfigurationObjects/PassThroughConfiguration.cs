using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record PassThroughConfiguration
{
    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Description("Optional response Content-Type header")]
    public string? ContentType { get; set; }

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }

    [Description("Preserve request metadata and only replace the HTTP response metadata")]
    public bool PreserveMetaData { get; set; } = true;
}
