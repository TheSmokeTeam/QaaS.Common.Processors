using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record StaticResponseConfiguration
{
    [Description("Response body to return as UTF-8 text")]
    public string? Body { get; set; }

    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Required, Description("Response Content-Type header")]
    public string ContentType { get; set; } = "text/plain; charset=utf-8";

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
