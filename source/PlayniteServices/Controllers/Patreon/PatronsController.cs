using Microsoft.AspNetCore.Mvc;

namespace Playnite.Backend.Patreon;

[Route("patreon")]
public class PatronsController : Controller
{
    private readonly PatreonManager patreon;

    public PatronsController(PatreonManager patreon)
    {
        this.patreon = patreon;
    }

    [HttpGet("patrons")]
    public DataResponse<List<string>> GetPatrons()
    {
        return new DataResponse<List<string>>(patreon.PatronsList);
    }
}
