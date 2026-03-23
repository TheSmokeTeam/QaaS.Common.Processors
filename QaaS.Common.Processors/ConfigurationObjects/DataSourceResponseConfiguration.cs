using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QaaS.Common.Processors.ConfigurationObjects;

public record DataSourceResponseConfiguration
{
    [Description("Optional data source name. When omitted, the first data source is used.")]
    public string? DataSourceName { get; set; }

    [Required, Description("Selection mode used to pick a generated item from the resolved data source")]
    public DataSourceSelectionMode SelectionMode { get; set; } = DataSourceSelectionMode.First;

    [Description("Zero-based item index used when SelectionMode is ByIndex")]
    public int Index { get; set; }

    [Required, Description("Response Status Code")]
    public int StatusCode { get; set; } = 200;

    [Description("Optional response Content-Type header")]
    public string? ContentType { get; set; }

    [Description("Fallback body to return when the data source cannot be resolved or selected")]
    public string? FallbackBody { get; set; }

    [Description("Additional response headers")]
    public IDictionary<string, string>? ResponseHeaders { get; set; }
}

public enum DataSourceSelectionMode
{
    First,
    Last,
    ByIndex
}
