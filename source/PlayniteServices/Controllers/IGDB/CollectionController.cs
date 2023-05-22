using Microsoft.AspNetCore.Mvc;

namespace PlayniteServices.Controllers.IGDB;

[ServiceFilter(typeof(ServiceKeyFilter))]
public class CollectionController<T> : Controller where T : class, IIgdbItem
{
    private readonly UpdatableAppSettings settings;
    private readonly IgdbApi igdb;
    private readonly IgdbCollection<T> collection;
    public readonly string EndpointPath;

    public CollectionController(string endpointPath, IgdbApi igdb, UpdatableAppSettings settings)
    {
        EndpointPath = endpointPath;
        this.igdb = igdb;
        this.settings = settings;
        collection = (IgdbCollection<T>)igdb.DataCollections.First(a => a.EndpointPath == endpointPath);
    }

    [HttpGet("{itemId}")]
    public async Task<ResponseBase> GetItem(ulong itemId)
    {
        return new DataResponse<T>(await collection.GetItem(itemId, true));
    }

    [HttpDelete("{itemId}")]
    public async Task<ActionResult> DeleteItem(ulong itemId)
    {
        await collection.Delete(itemId);
        return Ok();
    }

    [HttpGet("webhooks")]
    public async Task<ResponseBase> GetWebhooks()
    {
        var hooks = await igdb.GetWebhooks();
        if (hooks == null)
        {
            return new DataResponse<List<Webhook>>(default);
        }

        return new DataResponse<List<Webhook>>(hooks.
            Where(a => 
                a.url?.Replace(settings.Settings.IGDB!.WebHookRootAddress!, string.Empty, StringComparison.OrdinalIgnoreCase).
                TrimStart('/').
                StartsWith(EndpointPath, StringComparison.OrdinalIgnoreCase) == true).
            ToList());
    }

    [HttpPost("clone")]
    public ActionResult Clone()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        collection.CloneCollection();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        return Ok();
    }
}
