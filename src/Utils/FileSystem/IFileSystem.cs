using System;
using System.IO;

namespace LibWindPop.Utils.FileSystem
{
    public interface IFileSystem
    {
        Stream OpenReadWrite(string path);

        Stream OpenRead(string path);

        Stream OpenWrite(string path);

        Stream Create(string path);

        bool FileExists(string path);

        bool FolderExists(string path);

        void FileMove(string oldPath, string newPath);

        void FolderMove(string oldPath, string newPath);

        void FileDelete(string path);

        void FolderDelete(string path);

        void CreateFolder(string path);

        string GetNativePath(string path);

        string GetFakePath(string path);

        string Combine(string? path1, string? path2);

        string Combine(string? path1, string? path2, string? path3);

        string Combine(string? path1, string? path2, string? path3, string? path4);

        ITempFile CreateTempFile();

        string[] GetFiles(string path);

        string[] GetFolders(string path);

        string GetFileName(string path);

        string GetExtension(string path);

        string ChangeExtension(string path, string extension);

        string? GetParentPath(string path);

        DateTime GetCreateTimeUtc(string path);

        DateTime GetModifyTimeUtc(string path);

        DateTime GetAccessTimeUtc(string path);

        void SetCreateTimeUtc(string path, DateTime time);

        void SetModifyTimeUtc(string path, DateTime time);

        void SetAccessTimeUtc(string path, DateTime time);
    }
}
