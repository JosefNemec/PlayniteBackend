using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Playnite.SDK;
using System.IO;
using System.Net.Http;

namespace PlayniteServices.Controllers;

[Route("github")]
public class GitHubController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly UpdatableAppSettings settings;

    public GitHubController(UpdatableAppSettings settings)
    {
        this.settings = settings;
    }

    //[HttpGet("sponsors")]
    //public async Task<ServicesResponse<List<string>>> Get()
    //{
    //    return new ServicesResponse<List<string>>(new List<string>());
    //}
}