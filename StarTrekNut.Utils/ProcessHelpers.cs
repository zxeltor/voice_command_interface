using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StarTrekNut.Utils
{
    public static class ProcessHelpers
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Check for a running process, and return a handle for it.
        /// </summary>
        /// <param name="applicationName">
        ///     The name of the application.
        ///     <remarks>Typically the name of the executable without the file name extenstion</remarks>
        /// </param>
        /// <returns>A process handle to the found process</returns>
        public static Process FindRunningProcess(string applicationName)
        {
            return Process.GetProcesses().FirstOrDefault(proc => proc.ProcessName.Equals(applicationName));
        }

        /// <summary>
        ///     Check to see if a process is running
        /// </summary>
        /// <param name="applicationName">
        ///     The name of the application.
        ///     <remarks>Typically the name of the executable without the file name extenstion</remarks>
        /// </param>
        /// <returns>True if the process is running, False otherwise.</returns>
        public static bool IsProcessRunning(string applicationName)
        {
            return Process.GetProcesses().Any(proc => proc.ProcessName.Equals(applicationName));
        }

        /// <summary>
        ///     Send a sequence of keys to a running application.
        /// </summary>
        /// <param name="keys">Keystrokes to send to the running application</param>
        /// <param name="runningProcess">A handle to the running application</param>
        public static void SendKeysToApplication(string keys, Process runningProcess)
        {
            if (keys == null)
                throw new NullReferenceException("keys param is NULL");
            if (runningProcess == null)
                throw new NullReferenceException("runningProcess param is NULL");
            if (runningProcess.HasExited)
                throw new Exception("The process has existed");

            runningProcess.WaitForInputIdle();
            var h = runningProcess.MainWindowHandle;
            SetForegroundWindow(h);
            SendKeys.SendWait(keys);
        }

        #endregion

        #region Methods

        // import function which sets wow as the foreground window.
        [DllImport("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr point);

        #endregion
    }
}