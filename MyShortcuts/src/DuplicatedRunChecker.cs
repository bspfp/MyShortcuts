using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyShortcuts {
    internal class DuplicatedRunChecker : IDisposable {
        public const int TryLockCount = 3;

        public bool ExistsRunning(string inLockFilePath, uint notifyMessage) {
            lockFilePath = inLockFilePath;

            var tryCount = TryLockCount;
            while (tryCount-- > 0) {
                try {
                    lockFile = File.Open(lockFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                    var pid = Process.GetCurrentProcess().Id;
                    var bt = BitConverter.GetBytes(pid);
                    lockFile.Write(bt, 0, bt.Length);
                    lockFile.Flush(true);
                    return false;
                }
                catch { }
                try {
                    using (var f = File.Open(lockFilePath, FileMode.Open, FileAccess.Read, FileShare.Write)) {
                        var buf = new byte[4];
                        f.Read(buf, 0, 4);
                        var pid = BitConverter.ToInt32(buf, 0);
                        var running = Process.GetProcessById(pid);
                        var mainWindow = running.MainWindowHandle;
                        _ = Win32.PostMessage(mainWindow, notifyMessage, 0, 0);
                        return true;
                    }
                }
                catch (Exception ex) {
                    Debug.WriteLine(ex.Message);
                }
                Thread.Sleep(1000);
            }

            return true;
        }

        private string lockFilePath = "";
        private FileStream lockFile = null;

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (lockFile != null) {
                        lockFile.Dispose();
                        lockFile = null;
                        File.Delete(lockFilePath);
                    }
                }
                disposedValue = true;
            }
        }
        ~DuplicatedRunChecker() {
            Dispose(disposing: false);
        }
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
