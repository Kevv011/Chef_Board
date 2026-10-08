using System.Collections.Generic;

namespace Chef_Board_Desafio3;

public class SsrsOptions
{
    public string? BaseUrl { get; set; }
    public string? PortalBaseUrl { get; set; }
    public Dictionary<string, string> Reports { get; set; } = new Dictionary<string, string>();

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(BaseUrl) && Reports != null && Reports.Count > 0;
    }
}
