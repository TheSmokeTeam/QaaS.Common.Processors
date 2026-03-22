using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record TextTransformConfiguration
{
    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Required, Description("Response Content-Type header")]
    public string ContentType { get; set; } = "text/plain; charset=utf-8";

    [Description("Text to prepend to the transformed payload")]
    public string Prefix { get; set; } = string.Empty;

    [Description("Text to append to the transformed payload")]
    public string Suffix { get; set; } = string.Empty;

    [Description("Optional text to search for in the incoming payload")]
    public string? SearchText { get; set; }

    [Description("Replacement text for SearchText. Null removes the matched text.")]
    public string? ReplacementText { get; set; }

    [Description("Trim leading and trailing whitespace before applying prefix, suffix, and replacement")]
    public bool TrimWhitespace { get; set; }

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}
