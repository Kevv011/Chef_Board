using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Chef_Board_Desafio3.Models;

namespace Chef_Board_Desafio3.Pages;

[Authorize(AuthenticationSchemes = "Identity.Application")]
public class ReportesModel : PageModel
{
    private readonly SsrsOptions _ssrs;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ReportesModel(IOptions<SsrsOptions> ssrsOptions, SignInManager<ApplicationUser> signInManager)
    {
        _ssrs = ssrsOptions.Value;
        _signInManager = signInManager;
    }

    [BindProperty(SupportsGet = true)]
    public string SelectedReportKey { get; set; } = "RecipesIngredients";

    [BindProperty(SupportsGet = true)]
    public int? TiempoMaximo { get; set; }

    public string? IframeUrl { get; private set; }
    public string? ReportUrl { get; private set; }
    public string? ManagerUrl { get; private set; }
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Login");
    }

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

        ReportUrl = url;
        IframeUrl = url + "&rs:Embed=true";

        // El portal puede tener una URL distinta de la del servicio de informes.
        var portalBase = _ssrs.PortalBaseUrl?.TrimEnd('/');
        if (string.IsNullOrEmpty(portalBase))
        {
            portalBase = baseUrl.Replace("/ReportServer", "/Reports", StringComparison.OrdinalIgnoreCase);
        }
        ManagerUrl = portalBase + "/report" + reportPath;
    }
}
