using System;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace StarTrekNut.Utils
{
    public static class ProcessHelpers
    {
        /// <summary>
        /// The max character length to test main window title.
        /// </summary>
        private static readonly int _applicationTitleCharMax = 512;

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
        /// Send voice command keystrokes to our target application
        /// </summary>
        /// <param name="keys">Key strokes to send</param>
        /// <param name="targetApplication">our target application</param>
        /// <param name="keyStrokesDelayInMilliSeconds">a delay in milliseconds between keystrokes</param>
        /// <returns>True if successfull, False otherwise</returns>
        public static bool SendKeysToApplication(List<System.Windows.Input.Key> keys, Process targetApplication, int keyStrokesDelayInMilliSeconds)
        {
            if (keys == null || !keys.Any())
                throw new NullReferenceException("keys param is NULL");
            if (targetApplication == null)
                throw new NullReferenceException("runningProcess param is NULL");
            if (targetApplication.HasExited)
                throw new Exception("The process has existed");

            targetApplication.WaitForInputIdle();

            var inputSimulator = new WindowsInput.InputSimulator();

            StringBuilder sb = new StringBuilder(_applicationTitleCharMax);
            if (GetWindowText(GetForegroundWindow(), sb, _applicationTitleCharMax) > 0)
            {
                // Get the target window title and truncate if too long.
                var targetAppMainWindowTitle = targetApplication.MainWindowTitle;
                if (targetAppMainWindowTitle.Length > _applicationTitleCharMax) targetAppMainWindowTitle = targetAppMainWindowTitle.Substring(0, _applicationTitleCharMax);

                // If our target process doesn't have focus, we want to set focus to it
                // and wait for 3 seconds before sending any keystrokes to the application
                if (!sb.ToString().Equals(targetApplication.MainWindowTitle))
                {
                    // If we fail to set focus to our running process, then return
                    // and don't try sending keystrokes.
                    if (!SetForegroundWindow(targetApplication.MainWindowHandle))
                    {
                        return false;
                    }

                    // Wait for 3 seconds before sending any keystrokes to the application
                    inputSimulator.Keyboard.Sleep(3000);
                }
            }
            else
            {
                return false;
            }

            // A list of modifier keys
            var keyUp = new List<System.Windows.Input.Key>();
            
            // Process keystrokes to send to the target application
            keys.ForEach(key =>
            {
                // Process leading modifier keys
                if (key == System.Windows.Input.Key.LeftShift ||
                    key == System.Windows.Input.Key.RightShift ||
                    key == System.Windows.Input.Key.LeftCtrl ||
                    key == System.Windows.Input.Key.RightCtrl ||
                    key == System.Windows.Input.Key.LeftAlt ||
                    key == System.Windows.Input.Key.RightAlt)
                {
                    // Convert our application key to 3rd party enum
                    var convertedModifierKey = System.Windows.Input.KeyInterop.VirtualKeyFromKey(key);
                    var castedModifierKey = (WindowsInput.Native.VirtualKeyCode)convertedModifierKey;

                    // If more then one key in the list, then add a short delay
                    if (keys.Count > 1) inputSimulator.Keyboard.Sleep(keyStrokesDelayInMilliSeconds);
                    
                    // Send our KeyDown modifier key to the target application
                    inputSimulator.Keyboard.KeyDown(castedModifierKey);

                    // Add our KeyDown keystroke to our modifier list so it can be processed later.
                    keyUp.Add(key);
                }
                else
                {
                    // Convert our application key to 3rd party enum
                    var convertedKey = System.Windows.Input.KeyInterop.VirtualKeyFromKey(key);
                    var castedKey = (WindowsInput.Native.VirtualKeyCode)convertedKey;

                    // If more then one key in the list, then add a short delay
                    if (keys.Count > 1) inputSimulator.Keyboard.Sleep(keyStrokesDelayInMilliSeconds);

                    // Send our key press to the target application
                    inputSimulator.Keyboard.KeyPress(castedKey);

                    // If any key modifers were included, then we need to process them now.
                    if (keyUp.Any())
                    {
                        keyUp.ForEach(keyUpKey => 
                            {
                                // Convert our application key to 3rd party enum
                                var convertedModifierKey = System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyUpKey);
                                var castedModifierKey = (WindowsInput.Native.VirtualKeyCode)convertedModifierKey;

                                // If more then one key in the list, then add a short delay
                                if (keys.Count > 1) inputSimulator.Keyboard.Sleep(keyStrokesDelayInMilliSeconds);

                                // Send our KeyUp modifier key to the target application
                                inputSimulator.Keyboard.KeyUp(castedModifierKey);
                            });
                        keyUp.Clear();
                    }
                }
            });

            // We have this hear in case someone has one or more modifier keys at the end of their keystrokes.
            if(keyUp.Any())
            {
                keyUp.ForEach(keyUpKey =>
                {
                    // Convert our application key to 3rd party enum
                    var convertedModifierKey = System.Windows.Input.KeyInterop.VirtualKeyFromKey(keyUpKey);
                    var castedModifierKey = (WindowsInput.Native.VirtualKeyCode)convertedModifierKey;

                    // If more then one key in the list, then add a short delay
                    if (keys.Count > 1) inputSimulator.Keyboard.Sleep(keyStrokesDelayInMilliSeconds);

                    // Send our KeyUp modifier key to the target application
                    inputSimulator.Keyboard.KeyUp(castedModifierKey);
                });
                keyUp.Clear();
            }

            return true;
        }

        #endregion

        #region Methods

        // import function used to set focus on our target application.
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr point);

        // import function used to get the currently application with focus
        [DllImport("User32.dll")]
        static extern int GetForegroundWindow();

        // import function to get the title of the current application window with focus
        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);
        #endregion
    }
}