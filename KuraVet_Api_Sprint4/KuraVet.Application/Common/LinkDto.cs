namespace KuraVet.Application.Common;

/// Link HATEOAS: relação, URL absoluta e método HTTP.
public sealed class LinkDto
{
    public LinkDto()
    {
    }

    public LinkDto(string rel, string href, string method)
    {
        Rel = rel;
        Href = href;
        Method = method;
    }

    public string Rel { get; set; } = string.Empty;

    public string Href { get; set; } = string.Empty;

    public string Method { get; set; } = "GET";
}
