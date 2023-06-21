using System;
using System.IO;

namespace LibWindPop.Utils.FileSystem
{
    public struct NativeFileSystem : IFileSystem
    {
        private readonly bool m_IsWindows;
        private static StreamWriter? m_outStream;

        public NativeFileSystem()
        {
            m_IsWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
        }

        public Stream OpenReadWrite(string path)
        {
            return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public Stream Create(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
            string? folder = Path.GetDirectoryName(path);
            if (folder != null)
            {
                Directory.CreateDirectory(folder);
            }
            return File.Create(path);
        }

        public bool FileExists(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
            return File.Exists(path);
        }

        public bool FolderExists(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
            return Directory.Exists(path);
        }

        public void FileMove(string oldPath, string newPath)
        {
            File.Move(oldPath, newPath);
        }

        public void FolderMove(string oldPath, string newPath)
        {
            Directory.Move(oldPath, newPath);
        }

        public void FileDelete(string path)
        {
            File.Delete(path);
        }

        public void FolderDelete(string path)
        {
            Directory.Delete(path);
        }

        public void CreateFolder(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
            Directory.CreateDirectory(path);
        }

        public string GetNativePath(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
            return path;
        }

        public string GetFakePath(string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('\\', '/');
            }
            return path;
        }

        public string Combine(string? path1, string? path2)
        {
            return Path.Combine(path1!, path2!);
        }

        public string Combine(string? path1, string? path2, string? path3)
        {
            return Path.Combine(path1!, path2!, path3!);
        }

        public string Combine(string? path1, string? path2, string? path3, string? path4)
        {
            return Path.Combine(path1!, path2!, path3!, path4!);
        }

        public ITempFile CreateTempFile()
        {
            return new NativeTempFile();
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetFolders(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string ChangeExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        public string? GetParentPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public DateTime GetCreateTimeUtc(string path)
        {
            return File.GetCreationTimeUtc(path);
        }

        public DateTime GetModifyTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public DateTime GetAccessTimeUtc(string path)
        {
            return File.GetLastAccessTimeUtc(path);
        }

        public void SetCreateTimeUtc(string path, DateTime time)
        {
            File.SetCreationTimeUtc(path, time);
        }

        public void SetModifyTimeUtc(string path, DateTime time)
        {
            File.SetLastWriteTimeUtc(path, time);
        }

        public void SetAccessTimeUtc(string path, DateTime time)
        {
            File.SetLastAccessTimeUtc(path, time);
        }
    }
}
