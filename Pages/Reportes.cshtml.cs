using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chef_Board_Desafio3.Pages;

[Authorize]
public class ReportesModel : PageModel
{
    private readonly SsrsOptions _ssrs;

    public ReportesModel(IOptions<SsrsOptions> ssrsOptions)
    {
        _ssrs = ssrsOptions.Value;
    }

    [BindProperty(SupportsGet = true)]
    public string SelectedReportKey { get; set; } = "RecipesIngredients";

    [BindProperty(SupportsGet = true)]
    public int? TiempoMaximo { get; set; }

    public string? IframeUrl { get; private set; }
    public string? ManagerUrl { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        if (!_ssrs.IsValid())
        {
            ErrorMessage = "Configuración de SSRS inválida en la aplicación.";
            return;
        }

        if (!_ssrs.Reports.TryGetValue(SelectedReportKey, out var reportPath))
        {
            ErrorMessage = "Reporte solicitado no está disponible.";
            return;
        }

        var baseUrl = _ssrs.BaseUrl?.TrimEnd('/');
        if (string.IsNullOrEmpty(baseUrl))
        {
            ErrorMessage = "BaseUrl de SSRS no configurada.";
            return;
        }

        // Construir URL de ReportServer (URL Access)
        var url = $"{baseUrl}?{reportPath}&rs:Command=Render&rc:Toolbar=true&rc:Parameters=true";

        if (SelectedReportKey == "RecipesIngredients" && TiempoMaximo.HasValue)
        {
            if (TiempoMaximo <= 0 || TiempoMaximo > 1440)
            {
                ErrorMessage = "Tiempo máximo debe ser entre 1 y 1440 minutos.";
                return;
            }
            url += $"&TiempoMaximo={TiempoMaximo.Value}";
        }

        IframeUrl = url;

        // Construir URL de fallback al portal (Reports). Reemplazamos ReportServer por Reports
        try
        {
            var portalBase = baseUrl.Replace("/ReportServer", "/Reports");
            ManagerUrl = portalBase + "/report" + reportPath;
        }
        catch
        {
            ManagerUrl = null;
        }
    }
}
