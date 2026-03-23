using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record ConditionalResponseConfiguration
{
    [Required, Description("Rules evaluated in order. The first matching rule wins.")]
    public IList<ConditionalResponseRule> Rules { get; set; } = [];

    [Description("Fallback response body when no rule matches")]
    public string? DefaultBody { get; set; }

    [Required, Description("Fallback response Status Code")]
    public int DefaultStatusCode { get; set; } = 404;

    [Required, Description("Fallback response Content-Type header")]
    public string DefaultContentType { get; set; } = "text/plain; charset=utf-8";

    [Description("Fallback response headers")]
    public IDictionary<string, string>? DefaultResponseHeaders { get; set; }
}

public record ConditionalResponseRule
{
    [Description("Request header name to match")]
    public string? RequestHeaderName { get; set; }

    [Description("Path parameter name to match")]
    public string? PathParameterName { get; set; }

    [Required, Description("Expected value for the selected request header or path parameter")]
    public string ExpectedValue { get; set; } = string.Empty;

    [Description("Response body when the rule matches")]
    public string? ResponseBody { get; set; }

    [Required, Description("Response Status Code when the rule matches")]
    public int StatusCode { get; set; } = 200;

    [Required, Description("Response Content-Type header when the rule matches")]
    public string ContentType { get; set; } = "text/plain; charset=utf-8";

    [Description("Response headers when the rule matches")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
