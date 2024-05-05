using System;
using System.IO;

namespace LibWindPop.Utils.FileSystem
{
    public struct NativeFileSystem : IFileSystem
    {
        private readonly bool m_IsWindows;

        private readonly void NormalizePath(ref string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('/', '\\').Replace(" \\", "\\");
            }
        }

        private readonly void DenormalizePath(ref string path)
        {
            if (m_IsWindows)
            {
                path = path.Replace('\\', '/');
            }
        }

        public NativeFileSystem()
        {
            m_IsWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
        }

        public Stream OpenReadWrite(string path)
        {
            NormalizePath(ref path);
            return File.Open(path, FileMode.Open, FileAccess.ReadWrite);
        }

        public Stream OpenRead(string path)
        {
            NormalizePath(ref path);
            return File.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            NormalizePath(ref path);
            return File.OpenWrite(path);
        }

        public Stream Create(string path)
        {
            NormalizePath(ref path);
            string? folder = Path.GetDirectoryName(path);
            if (folder != null)
            {
                Directory.CreateDirectory(folder);
            }
            return File.Create(path);
        }

        public bool FileExists(string path)
        {
            NormalizePath(ref path);
            return File.Exists(path);
        }

        public bool FolderExists(string path)
        {
            NormalizePath(ref path);
            return Directory.Exists(path);
        }

        public void FileMove(string oldPath, string newPath)
        {
            NormalizePath(ref oldPath);
            NormalizePath(ref newPath);
            File.Move(oldPath, newPath);
        }

        public void FolderMove(string oldPath, string newPath)
        {
            NormalizePath(ref oldPath);
            NormalizePath(ref newPath);
            Directory.Move(oldPath, newPath);
        }

        public void FileDelete(string path)
        {
            NormalizePath(ref path);
            File.Delete(path);
        }

        public void FolderDelete(string path)
        {
            NormalizePath(ref path);
            Directory.Delete(path);
        }

        public void CreateFolder(string path)
        {
            NormalizePath(ref path);
            Directory.CreateDirectory(path);
        }

        public string GetNativePath(string path)
        {
            NormalizePath(ref path);
            return path;
        }

        public string GetFakePath(string path)
        {
            DenormalizePath(ref path);
            return path;
        }

        public string Combine(string path1, string path2)
        {
            NormalizePath(ref path1);
            NormalizePath(ref path2);
            string ans = Path.Combine(path1, path2);
            DenormalizePath(ref ans);
            return ans;
        }

        public string Combine(string path1, string path2, string path3)
        {
            NormalizePath(ref path1);
            NormalizePath(ref path2);
            NormalizePath(ref path3);
            string ans = Path.Combine(path1, path2, path3);
            DenormalizePath(ref ans);
            return ans;
        }

        public string Combine(string path1, string path2, string path3, string path4)
        {
            NormalizePath(ref path1);
            NormalizePath(ref path2);
            NormalizePath(ref path3);
            NormalizePath(ref path4);
            string ans = Path.Combine(path1, path2, path3, path4);
            DenormalizePath(ref ans);
            return ans;
        }

        public ITempFile CreateTempFile()
        {
            return new NativeTempFile();
        }

        public string[] GetFiles(string path)
        {
            NormalizePath(ref path);
            string[] ans = Directory.GetFiles(path);
            for (int i = 0; i < ans.Length; i++)
            {
                DenormalizePath(ref ans[i]);
            }
            return ans;
        }

        public string[] GetFolders(string path)
        {
            NormalizePath(ref path);
            string[] ans = Directory.GetDirectories(path);
            for (int i = 0; i < ans.Length; i++)
            {
                DenormalizePath(ref ans[i]);
            }
            return ans;
        }

        public string GetFileName(string path)
        {
            NormalizePath(ref path);
            string ans = Path.GetFileName(path);
            DenormalizePath(ref ans);
            return ans;
        }

        public string GetExtension(string path)
        {
            NormalizePath(ref path);
            string ans = Path.GetExtension(path);
            DenormalizePath(ref ans);
            return ans;
        }

        public string ChangeExtension(string path, string extension)
        {
            NormalizePath(ref path);
            string ans = Path.ChangeExtension(path, extension);
            DenormalizePath(ref ans);
            return ans;
        }

        public string? GetParentPath(string path)
        {
            NormalizePath(ref path);
            string? ans = Path.GetDirectoryName(path);
            if (ans != null)
            {
                DenormalizePath(ref ans);
            }
            return ans;
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
