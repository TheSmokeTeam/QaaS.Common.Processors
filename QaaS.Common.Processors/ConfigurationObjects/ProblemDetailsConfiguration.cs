using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record ProblemDetailsConfiguration
{
    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 500;

    [Required, Description("Problem details title")]
    public string Title { get; set; } = "An error occurred";

    [Description("Problem details detail")]
    public string? Detail { get; set; }

    [Required, Description("Problem details type")]
    public string Type { get; set; } = "about:blank";

    [Description("Explicit problem details instance")]
    public string? Instance { get; set; }

    [Description("Use the request URI as the problem details instance when no explicit instance is configured")]
    public bool UseRequestUriAsInstance { get; set; } = true;

    [Required, Description("Response Content-Type header")]
    public string ContentType { get; set; } = "application/problem+json";

    [Description("Additional problem details extension fields")]
    public IDictionary<string, string>? Extensions { get; set; }

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
