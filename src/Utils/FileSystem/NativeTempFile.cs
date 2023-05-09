using System.IO;

namespace LibWindPop.Utils.FileSystem
{
    public class NativeTempFile : ITempFile
    {
        private string m_Path;
        private Stream m_Stream;
        private bool disposedValue;

        public NativeTempFile()
        {
            m_Path = Path.GetTempFileName();
            m_Stream = File.Create(m_Path);
        }

        public string NativePath => m_Path;

        public Stream Stream => m_Stream;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                m_Stream.Dispose();
                File.Delete(m_Path);
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~NativeTempFile()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
