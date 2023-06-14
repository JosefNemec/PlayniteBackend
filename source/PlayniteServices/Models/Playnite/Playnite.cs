namespace PlayniteServices;

public class User
{
    public string? Id { get; set; }
    public string? WinVersion { get; set; }
    public string? PlayniteVersion { get; set; }
    public DateTime LastLaunch { get; set; }
    public bool Is64Bit { get; set; }
}

public enum AddonType
{
    GameLibrary,
    MetadataProvider,
    Generic,
    ThemeDesktop,
    ThemeFullscreen
}

public class AddonManifestBase
{
    public class AddonUserAgreement
    {
        public DateTime Updated { get; set; }
        public string? AgreementUrl { get; set; }
    }

    public class AddonScreenshot
    {
        public string? Thumbnail { get; set; }
        public string? Image { get; set; }
    }

    public string? IconUrl { get; set; }
    public List<AddonScreenshot>? Screenshots { get; set; }
    public AddonType Type { get; set; }
    public string? InstallerManifestUrl { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
    public string? AddonId { get; set; }
    public string? Author { get; set; }
    public Dictionary<string, string>? Links { get; set; }
    public List<string>? Tags { get; set; }
    public AddonUserAgreement? UserAgreement { get; set; }
    public string? SourceUrl { get; set; }
}

public class AddonInstallerPackage
{
    public Version? Version { get; set; }
    public string? PackageUrl { get; set; }
    public Version? RequiredApiVersion { get; set; }
    public DateTime ReleaseDate { get; set; }
    public List<string>? Changelog { get; set; }
}

public class AddonInstallerManifestBase
{
    public string? AddonId { get; set; }
    public List<AddonInstallerPackage>? Packages { get; set; }
}

public class ServiceStats
{
    public int UserCount;
    public int LastWeekUserCount;
    public SortedDictionary<string, int> UsersByVersion = new SortedDictionary<string, int>();
    public SortedDictionary<string, int> UsersByWinVersion = new SortedDictionary<string, int>();
    public int X86Count;
    public int X64Count;
}

public class DiagPackage
{
    public string? Id { get; set; }
    public string? Version { get; set; }
    public bool IsCrash { get; set; }
    public DateTime Date { get; set; }
}