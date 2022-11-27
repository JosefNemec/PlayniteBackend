using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Playnite;
using Playnite.Common;
using PlayniteServices.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlayniteServices.Controllers.PlayniteTools
{
    [Route("playnite/diag")]
    public class DiagnosticsController : Controller
    {
        private readonly string diagsDir;
        private readonly string diagsCrashDir;

        public DiagnosticsController(UpdatableAppSettings settings)
        {
            diagsDir = settings.Settings.DiagsDirectory;
            if (!Path.IsPathRooted(diagsDir))
            {
                diagsDir = Path.Combine(ServicePaths.ExecutingDirectory, diagsDir);
            }

            diagsCrashDir = Path.Combine(diagsDir, "crashes");
        }

        [ServiceFilter(typeof(ServiceKeyFilter))]
        [HttpGet("serverlog")]
        public IActionResult GetServerLog()
        {
            var logPath = Path.Combine(ServicePaths.ExecutingDirectory, "playnite.log");
            var zipLog = Path.Combine(ServicePaths.ExecutingDirectory, "serverlog.zip");
            if (System.IO.File.Exists(zipLog))
            {
                System.IO.File.Delete(zipLog);
            }

            using (var zip = ZipFile.Open(zipLog, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(logPath, "serverlog.log");
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
        public ServicesResponse<List<string>> GetPackages()
        {
            if (!Directory.Exists(diagsDir))
            {
                return new ServicesResponse<List<string>>(new List<string>());
            }

            var diagFiles = Directory.
                GetFiles(diagsDir, "*.zip", SearchOption.AllDirectories).
                Select(a => a.Replace(diagsDir, "", StringComparison.Ordinal).Trim(Path.DirectorySeparatorChar) + $",{new FileInfo(a).CreationTime}").
                ToList();
            return new ServicesResponse<List<string>>(diagFiles);
        }

        [ServiceFilter(typeof(PlayniteVersionFilter))]
        [HttpPost]
        public ServicesResponse<Guid> UploadPackage()
        {
            var packageId = Guid.NewGuid();
            var targetPath = Path.Combine(diagsDir, $"{packageId}.zip");
            if (!Directory.Exists(diagsDir))
            {
                Directory.CreateDirectory(diagsDir);
            }

            if (!Directory.Exists(diagsCrashDir))
            {
                Directory.CreateDirectory(diagsCrashDir);
            }

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
                        var info = DataSerialization.FromJson<DiagnosticPackageInfo>(infoStream);
                        version = info.PlayniteVersion;
                        isCrash = info.IsCrashPackage;
                    }
                }
                else
                {
                    var log = zip.GetEntry("playnite.log");
                    if (log != null)
                    {
                        using (var logStream = log.Open())
                        {
                            using (var tr = new StreamReader(logStream))
                            {
                                while (!tr.EndOfStream)
                                {
                                    var line = tr.ReadLine();
                                    if (line.Contains("Unhandled exception", StringComparison.OrdinalIgnoreCase))
                                    {
                                        isCrash = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    var playniteInfo = zip.GetEntry("playniteInfo.txt");
                    if (playniteInfo != null)
                    {
                        using (var infoStream = playniteInfo.Open())
                        {
                            var info = DataSerialization.FromJson<Dictionary<string, object>>(infoStream);
                            version = info["Version"].ToString();
                        }
                    }
                }
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

            return new ServicesResponse<Guid>(packageId);
        }
    }
}
