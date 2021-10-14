using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;

namespace MyShortcuts {
    public partial class App : Application {
        public static string Name { get; } = "MyShortcuts";

        public static App Inst => Current as App;

        public ConfigFile Config { get; private set; }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var lockFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Name + ".lock");
            if (duplicatedRunChecker.ExistsRunning(lockFileName, (uint)WindowMessages.Activate))
                Shutdown(0);

            Config = ConfigFile.Load();

            var accelTable = new ACCEL[] {
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_LEFT, cmd = CustomCommands.Backward },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_UP, cmd = CustomCommands.Up },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_RIGHT, cmd = CustomCommands.Forward},
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_HOME, cmd = CustomCommands.Home },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_INSERT, cmd = CustomCommands.SetHome },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_1, cmd = CustomCommands.ChangeDeactiveBehavior },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_2, cmd = CustomCommands.ChangePinMethod },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY | AccelFVirt.FALT, key = AccelVirtualKeys.VK_3, cmd = CustomCommands.ChangeKeepFolder },
                new ACCEL(){ fVirt = AccelFVirt.FVIRTKEY, key = AccelVirtualKeys.VK_F1, cmd = CustomCommands.About },
            };
            accelerators = Win32.CreateAcceleratorTableW(accelTable, accelTable.Length);

            ComponentDispatcher.ThreadFilterMessage += MsgFilter;
        }

        protected override void OnExit(ExitEventArgs e) {
            ComponentDispatcher.ThreadFilterMessage -= MsgFilter;

            Config.Save();

            duplicatedRunChecker?.Dispose();
            duplicatedRunChecker = null;

            base.OnExit(e);
        }

        private void MsgFilter(ref MSG msg, ref bool handled) {
            if (MainWindow is MainWindow mainWindow) {
                if (mainWindow.IsDialogMessage(ref msg)) {
                    handled = true;
                    return;
                }

                if (accelerators != IntPtr.Zero) {
                    if (PresentationSource.FromVisual(mainWindow) is HwndSource hwndSource) {
                        if (Win32.TranslateAcceleratorW(hwndSource.Handle, accelerators, ref msg) != 0) {
                            handled = true;
                            return;
                        }
                    }
                }

                if (mainWindow.PreTranslateMessage(ref msg)) {
                    handled = true;
                    return;
                }
            }

            handled = false;
        }

        private DuplicatedRunChecker duplicatedRunChecker = new DuplicatedRunChecker();
        private IntPtr accelerators = IntPtr.Zero;
    }
}
