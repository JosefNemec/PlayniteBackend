﻿using System.IO;
using System.Text.RegularExpressions;

namespace Playnite;

public partial class Paths
{
    private const string longPathPrefix = @"\\?\";
    private const string longPathUncPrefix = @"\\?\UNC\";
    public static readonly char[] DirectorySeparators = ['\\', '/'];

    public const int MaxPathLength = 32_767;

    public static bool IsValidFilePath(string path)
    {
        try
        {
            if (path.IsNullOrEmpty())
            {
                return false;
            }

            if (Path.GetExtension(path).IsNullOrEmpty())
            {
                return false;
            }

            var drive = Path.GetPathRoot(path);
            if (!drive.IsNullOrEmpty() && !Directory.Exists(drive))
            {
                return false;
            }

            return true;
        }
        catch
        {
            // Any of Path methods can throw exception in case that path is some weird string
            return false;
        }
    }

    public static string FixSeparators(string path)
    {
        if (path.IsNullOrWhiteSpace())
        {
            return path;
        }

        char prev = default;
        var sb = new StringBuilder(path.Length);
        for (int i = 0; i < path.Length; i++)
        {
            var current = path[i];
            if (current == Path.AltDirectorySeparatorChar)
            {
                current = Path.DirectorySeparatorChar;
            }

            if (prev != current || current != Path.DirectorySeparatorChar ||
                (current == Path.DirectorySeparatorChar && prev != Path.DirectorySeparatorChar))
            {
                prev = current;
                sb.Append(current);
                continue;
            }
        }

        if (path.StartsWith(@"\\", StringComparison.Ordinal))
        {
            sb.Insert(0, @"\");
        }

        return sb.ToString();
    }

    private static string Normalize(string path)
    {
        var formatted = path;
        try
        {
            formatted = new Uri(path).LocalPath;
        }
        catch
        {
            // this shound't happen
        }

        return Path.GetFullPath(formatted).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
    }

    public static bool AreEqual(string? path1, string? path2)
    {
        if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
        {
            return false;
        }

        try
        {
            return Normalize(path1) == Normalize(path2);
        }
        catch
        {
            return false;
        }
    }

    public static string GetSafePathName(string filename)
    {
        var path = string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        return Regex.Replace(path, @"\s+", " ").Trim();
    }

    public static bool IsFullPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        // Don't use Path.IsPathRooted because it fails on paths starting with one backslash.
        return Regex.IsMatch(path, @"^([a-zA-Z]:\\|\\\\)");
    }

    public static string GetCommonDirectory(string[] paths)
    {
        int k = paths[0].Length;
        for (int i = 1; i < paths.Length; i++)
        {
            k = Math.Min(k, paths[i].Length);
            for (int j = 0; j < k; j++)
            {
                if (paths[i][j] != paths[0][j])
                {
                    k = j;
                    break;
                }
            }
        }

        var common = paths[0][..k];
        if (common.Length == 0)
        {
            return string.Empty;
        }

        if (common[^1] == Path.DirectorySeparatorChar)
        {
            return common;
        }

        return common.Substring(0, common.LastIndexOf(Path.DirectorySeparatorChar) + 1);
    }

    public static string FormatAsLongPath(string path, bool forcePrefix = false)
    {
        if (path.IsNullOrWhiteSpace())
        {
            return path;
        }

        // Relative paths don't support long paths
        // https://docs.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=cmd
        if (!Paths.IsFullPath(path))
        {
            return path;
        }

        // While the MAX_PATH value is 260 characters, a lower value is used because
        // methods can append "\" and string terminator characters to paths and
        // make them surpass the limit
        if ((path.Length >= 258 || forcePrefix) && !path.StartsWith(longPathPrefix, StringComparison.Ordinal))
        {
            if (path.StartsWith(@"\\", StringComparison.Ordinal))
            {
                return string.Concat(longPathUncPrefix, path.AsSpan(2));
            }
            else
            {
                return longPathPrefix + path;
            }
        }

        return path;
    }

    public static string GetPathWithoutFileExtension(string path)
    {
        var index = path.LastIndexOf('.');
        if (index > 0)
        {
            return path[..index];
        }
        else
        {
            return path;
        }
    }
}
