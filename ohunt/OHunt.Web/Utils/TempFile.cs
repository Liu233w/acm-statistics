using System;
using System.IO;

namespace OHunt.Web.Utils
{
    /// <summary>
    /// Disposable temporary file
    ///
    /// from https://stackoverflow.com/a/400391
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        private string? _path;

        public TempFile() : this(System.IO.Path.GetTempFileName())
        {
        }

        public TempFile(string path)
        {
            _path = path;
        }

        public string Path
        {
            get
            {
                if (_path == null) throw new ObjectDisposedException(GetType().Name);
                return _path;
            }
        }

        ~TempFile()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }

            if (_path != null)
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
                } // best effort

                _path = null;
            }
        }
    }
}
