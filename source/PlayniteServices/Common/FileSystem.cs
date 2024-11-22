﻿using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace Playnite;

public enum FileSystemItem
{
    File,
    Directory
}

public static partial class FileSystem
{
    private static readonly ILogger logger = LogManager.GetLogger();

    public static void CreateDirectory(string path)
    {
        CreateDirectory(path, false);
    }

    public static void CreateDirectory(string path, bool clean)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        if (Directory.Exists(path))
        {
            if (clean)
            {
                DeleteDirectory(path, true);
            }
            else
            {
                return;
            }
        }

        Directory.CreateDirectory(path);
    }

    public static void PrepareSaveFile(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!dir.IsNullOrEmpty())
        {
            CreateDirectory(dir);
        }

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool IsDirectoryEmpty(string path)
    {
        if (Directory.Exists(path))
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
        else
        {
            return true;
        }
    }

    public static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void CreateFile(string path)
    {
        FileSystem.PrepareSaveFile(path);
        File.Create(path).Dispose();
    }

    public static void CopyFile(string sourcePath, string targetPath, bool overwrite = true)
    {
        PrepareSaveFile(targetPath);
        File.Copy(sourcePath, targetPath, overwrite);
    }

    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    public static void DeleteDirectory(string path, bool includeReadonly)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        if (includeReadonly)
        {
            foreach (var s in Directory.GetDirectories(path))
            {
                DeleteDirectory(s, true);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                var attr = File.GetAttributes(file);
                if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(file, attr ^ FileAttributes.ReadOnly);
                }

                File.Delete(file);
            }

            var dirAttr = File.GetAttributes(path);
            if ((dirAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                File.SetAttributes(path, dirAttr ^ FileAttributes.ReadOnly);
            }

            Directory.Delete(path, false);
        }
        else
        {
            DeleteDirectory(path);
        }
    }

    public static bool CanWriteToFolder(string folder)
    {
        try
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using var stream = File.Create(Path.Combine(folder, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string ReadFileAsStringSafe(string path, int retryAttempts = 5)
    {
        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't read from file, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
        }

        throw new IOException($"Failed to read {path}", ioException);
    }

    public static byte[] ReadFileAsBytesSafe(string path, int retryAttempts = 5)
    {
        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't read from file, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
        }

        throw new IOException($"Failed to read {path}", ioException);
    }

    public static Stream CreateWriteFileStreamSafe(string path, int retryAttempts = 5)
    {
        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                return new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't open write file stream, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
        }

        throw new IOException($"Failed to read {path}", ioException);
    }

    public static Stream OpenReadFileStreamSafe(string path, int retryAttempts = 5)
    {
        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                return new FileStream(path, FileMode.Open, FileAccess.Read);
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't open read file stream, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
        }

        throw new IOException($"Failed to read {path}", ioException);
    }

    public static void WriteStringToFile(string path, string content)
    {
        PrepareSaveFile(path);
        File.WriteAllText(path, content);
    }

    public static string ReadStringFromFile(string path, Encoding? encoding = null)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, encoding ?? Encoding.Default);
        return sr.ReadToEnd();
    }

    public static void WriteStringToFileSafe(string path, string content, int retryAttempts = 5)
    {
        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                PrepareSaveFile(path);
                File.WriteAllText(path, content);
                return;
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't write to a file, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
        }

        throw new IOException($"Failed to write to {path}", ioException);
    }

    public static void DeleteFileSafe(string path, int retryAttempts = 5)
    {
        if (!File.Exists(path))
        {
            return;
        }

        IOException? ioException = null;
        for (int i = 0; i < retryAttempts; i++)
        {
            try
            {
                File.Delete(path);
                return;
            }
            catch (IOException exc)
            {
                logger.Debug($"Can't detele file, trying again. {path}");
                ioException = exc;
                Task.Delay(500).Wait();
            }
            catch (UnauthorizedAccessException exc)
            {
                logger.Error(exc, $"Can't detele file, UnauthorizedAccessException. {path}");
                return;
            }
        }

        throw new IOException($"Failed to delete {path}", ioException);
    }

    public static long GetFreeSpace(string drivePath)
    {
        var root = Path.GetPathRoot(drivePath);
        var drive = DriveInfo.GetDrives().FirstOrDefault(a => a.RootDirectory.FullName.Equals(root, StringComparison.OrdinalIgnoreCase)); ;
        if (drive != null)
        {
            return drive.AvailableFreeSpace;
        }
        else
        {
            return 0;
        }
    }

    public static long GetFileSize(string path)
    {
        return GetFileSize(new FileInfo(path));
    }

    public static long GetFileSize(FileInfo fi)
    {
        return fi.Length;
    }

    public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = true, bool overwrite = true)
    {
        var dir = new DirectoryInfo(sourceDirName);
        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException(
                "Source directory does not exist or could not be found: "
                + sourceDirName);
        }

        var dirs = dir.GetDirectories();
        if (!Directory.Exists(destDirName))
        {
            Directory.CreateDirectory(destDirName);
        }

        var files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, overwrite);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, temppath, copySubDirs);
            }
        }
    }

    public static bool FileExistsOnAnyDrive(string filePath, [NotNullWhen(true)] out string? existringPath)
    {
        return PathExistsOnAnyDrive(filePath, path => File.Exists(path), out existringPath);
    }

    public static bool DirectoryExistsOnAnyDrive(string directoryPath, [NotNullWhen(true)] out string? existringPath)
    {
        return PathExistsOnAnyDrive(directoryPath, path => Directory.Exists(path), out existringPath);
    }

    private static bool PathExistsOnAnyDrive(string originalPath, Predicate<string> predicate, [NotNullWhen(true)] out string? existringPath)
    {
        existringPath = null;
        try
        {
            if (predicate(originalPath))
            {
                existringPath = originalPath;
                return true;
            }

            if (!Paths.IsFullPath(originalPath))
            {
                return false;
            }

            var availableDrives = DriveInfo.GetDrives().Where(d => d.IsReady);
            foreach (var drive in availableDrives)
            {
                var pathWithoutDrive = originalPath.Substring(drive.Name.Length);
                var newPath = Path.Combine(drive.Name, pathWithoutDrive);
                if (predicate(newPath))
                {
                    existringPath = newPath;
                    return true;
                }
            }
        }
        catch (Exception ex) when (!Debugger.IsAttached)
        {
            logger.Error(ex, $"Error checking if path exists on different drive \"{originalPath}\"");
        }

        return false;
    }

    public static void ReplaceStringInFile(string path, string oldValue, string newValue, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        var fileContent = File.ReadAllText(path, encoding);
        if (fileContent.IsNullOrEmpty())
        {
            return;
        }

        File.WriteAllText(path, fileContent.Replace(oldValue, newValue, StringComparison.Ordinal), encoding);
    }

    public static string GetMD5(Stream stream)
    {
        using var md5 = MD5.Create();
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "", StringComparison.Ordinal);
    }

    public static string GetMD5(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return GetMD5(stream);
    }

    public static bool AreFileContentsEqual(string path1, string path2)
    {
        var info1 = new FileInfo(path1);
        var info2 = new FileInfo(path2);
        if (info1.Length != info2.Length)
        {
            return false;
        }
        else
        {
            return GetMD5(path1) == GetMD5(path2);
        }
    }
}
