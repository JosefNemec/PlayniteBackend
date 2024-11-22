using Microsoft.AspNetCore.Mvc;
using Playnite;
using System.IO;
using System.IO.Compression;

namespace Playnite.Backend.Playnite;

[Route("playnite/diag")]
public class DiagnosticsController : Controller
{
    private static readonly ILogger logger = LogManager.GetLogger();
    private readonly string diagsDir;
    private readonly string diagsCrashDir;
    private readonly UpdatableAppSettings settings;

    public DiagnosticsController(UpdatableAppSettings settings)
    {
        this.settings = settings;
        if (settings.Settings.DiagsDirectory.IsNullOrEmpty())
        {
            throw new Exception("Diags dir is not configured.");
        }

        diagsDir = settings.Settings.DiagsDirectory;
        if (!Path.IsPathRooted(diagsDir))
        {
            diagsDir = Path.Combine(PlaynitePaths.RuntimeDataDir, diagsDir);
        }

        diagsCrashDir = Path.Combine(diagsDir, "crashes");
        FileSystem.CreateDirectory(diagsCrashDir);
    }

    [HttpGet("test")]
    public DataResponse<string> GetTest()
    {
        return new DataResponse<string>($"ok working {Program.Version?.ToString(2)}");
    }

    [ServiceFilter(typeof(ServiceKeyFilter))]
    [HttpGet("serverlog")]
    public IActionResult GetServerLog()
    {
        var logPath = Path.Combine(PlaynitePaths.LogFile);
        var zipLog = Path.Combine(PlaynitePaths.RuntimeDataDir, "serverlog.zip");
        if (System.IO.File.Exists(zipLog))
        {
            System.IO.File.Delete(zipLog);
        }

        using (var zip = ZipFile.Open(zipLog, ZipArchiveMode.Create))
        {
            var logEntry = zip.CreateEntry("serverlog.log");
            using (var cefS = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var writer = new StreamWriter(logEntry.Open()))
            {
                cefS.CopyTo(writer.BaseStream);
            }
        }

        return PhysicalFile(zipLog, System.Net.Mime.MediaTypeNames.Application.Zip, "serverlog.zip");
    }

    [ServiceFilter(typeof(ServiceKeyFilter))]
    [HttpGet("{packageId}")]
    public IActionResult GetPackage(Guid packageId)
    {
        var diagFiles = Directory.GetFiles(diagsDir, $"{packageId}.zip", SearchOption.AllDirectories);
        if (diagFiles.Length == 0)
        {
            return NotFound();
        }

        var diagFile = new FileInfo(diagFiles[0]);
        return PhysicalFile(diagFile.FullName, System.Net.Mime.MediaTypeNames.Application.Zip, diagFile.Name);
    }

    [ServiceFilter(typeof(ServiceKeyFilter))]
    [HttpDelete("{packageId}")]
    public IActionResult DeletePackage(Guid packageId)
    {
        var diagFiles = Directory.GetFiles(diagsDir, $"{packageId}.zip", SearchOption.AllDirectories);
        if (diagFiles.Length == 0)
        {
            return NotFound();
        }
        else
        {
            foreach (var file in diagFiles)
            {
                System.IO.File.Delete(file);
            }
        }

        return Ok();
    }

    [ServiceFilter(typeof(ServiceKeyFilter))]
    [HttpGet]
    public DataResponse<List<DiagPackage>> GetPackages()
    {
        if (!Directory.Exists(diagsDir))
        {
            return new DataResponse<List<DiagPackage>>(new List<DiagPackage>());
        }

        var packages = new List<DiagPackage>();
        foreach (var file in Directory.GetFiles(diagsDir, "*.zip", SearchOption.AllDirectories))
        {
            var isCrash = false;
            string? version = null;
            if (file.StartsWith(diagsCrashDir, StringComparison.OrdinalIgnoreCase))
            {
                isCrash = true;
                version = file.Split(Path.DirectorySeparatorChar)[^2];
            }

            packages.Add(new()
            {
                Id = Path.GetFileNameWithoutExtension(file),
                IsCrash = isCrash,
                Date = new FileInfo(file).CreationTime,
                Version = version
            });
        }

        return new DataResponse<List<DiagPackage>>(packages);
    }

    [ServiceFilter(typeof(PlayniteVersionFilter))]
    [HttpPost]
    public DataResponse<Guid> UploadPackage()
    {
        var packageId = Guid.NewGuid();
        var targetPath = Path.Combine(diagsDir, $"{packageId}.zip");
        using (var fs = new FileStream(targetPath, FileMode.OpenOrCreate))
        {
            Request.Body.CopyTo(fs);
        }

        var isCrash = false;
        var version = string.Empty;

        using (var zip = ZipFile.OpenRead(targetPath))
        {
            var diagInfo = zip.GetEntry(DiagnosticPackageInfo.PackageInfoFileName);
            if (diagInfo != null)
            {
                using (var infoStream = diagInfo.Open())
                {
                    var info = Serialization.FromJsonStream<DiagnosticPackageInfo>(infoStream);
                    if (info == null)
                    {
                        logger.Warn("Received diag. package without package info file, ignoring");
                        return new DataResponse<Guid>(Guid.Empty);
                    }

                    version = info.PlayniteVersion;
                    isCrash = info.IsCrashPackage;
                }
            }
            else
            {
                logger.Warn("Received diag. package without package info file, ignoring");
                return new DataResponse<Guid>(Guid.Empty);
            }
        }

        var ignorePackage = false;
        if (Version.TryParse(version, out var playniteVer))
        {
            if (playniteVer < settings.Settings.MinimumDiagVersion)
            {
                ignorePackage = true;
            }
        }
        else
        {
            ignorePackage = true;
        }

        if (ignorePackage)
        {
            logger.Info($"Ignoring diag package from version {version}");
            System.IO.File.Delete(targetPath);
            return new DataResponse<Guid>(Guid.Empty);
        }

        if (isCrash)
        {
            var dir = diagsCrashDir;
            if (!string.IsNullOrEmpty(version))
            {
                dir = Path.Combine(dir, version);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            var newPath = Path.Combine(dir, Path.GetFileName(targetPath));
            System.IO.File.Move(targetPath, newPath);
        }

        return new DataResponse<Guid>(packageId);
    }
}
