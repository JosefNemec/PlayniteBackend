using Microsoft.AspNetCore.Mvc;

namespace Playnite.Backend.IGDB;

[ServiceFilter(typeof(ServiceKeyFilter))]
[Route("igdb")]
public class IgdbController : Controller
{
    private readonly IgdbManager igdb;

    public IgdbController(IgdbManager igdb)
    {
        this.igdb = igdb;
    }

    [HttpGet("webhooks")]
    public async Task<ResponseBase> GetAllWebhooks()
    {
        return new DataResponse<List<Webhook>>(await igdb.GetWebhooks());
    }

    [HttpDelete("webhooks")]
    public async Task<ActionResult> DeleteAllWebhooks()
    {
        foreach (var webhook in await igdb.GetWebhooks())
        {
            await igdb.DeleteWebhook(webhook.id);
        }

        return Ok();
    }

    [HttpDelete("webhooks/{webhookId:int}")]
    public async Task<ActionResult> DeleteWebhooks(int webhookId)
    {
        if (webhookId <= 0)
        {
            return BadRequest();
        }

        await igdb.DeleteWebhook(webhookId);
        return Ok();
    }

    [HttpPost("clone_collections")]
    public ActionResult Clone()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        igdb.CloneAllCollections();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        return Ok();
    }
}