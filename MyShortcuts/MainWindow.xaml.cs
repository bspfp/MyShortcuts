using System;
using System.Windows;
using System.Windows.Interop;

namespace MyShortcuts {
    public partial class MainWindow : Window {
        public static MainWindow Inst => App.Inst.MainWindow as MainWindow;

        private ExplorerBrowser explorerBrowser;
        private About aboutDlg = null;
        private ToastWindow toast = null;

        public MainWindow() {
            InitializeComponent();

            if (App.Inst.Config.Valid) {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = App.Inst.Config.Left;
                Top = App.Inst.Config.Top;
                Width = App.Inst.Config.Width;
                Height = App.Inst.Config.Height;
            }
            else {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Left = double.NaN;
                Top = double.NaN;
                Width = 512;
                Height = 300;
            }
        }

        public bool PreTranslateMessage(ref MSG msg) {
            return explorerBrowser != null && explorerBrowser.PreTranslateMessage(ref msg);
        }

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            if (PresentationSource.FromVisual(this) is HwndSource hwndSource) {
                hwndSource.AddHook(WndProcHook);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            explorerBrowser = new ExplorerBrowser();
            explorerBrowserHolder.Child = explorerBrowser;

            explorerBrowser.NavigateToFolder(App.Inst.Config.Folder);

            // 화면의 1/8 또는 버튼 공간은 확보 되도록
            MinWidth = Math.Max(SystemParameters.VirtualScreenWidth / 8, upButton.ActualWidth * 8);
            MinHeight = Math.Max(SystemParameters.VirtualScreenHeight / 8, upButton.ActualHeight * 4);

            explorerBrowser.SetFocusToShellView();

            switch (App.Inst.Config.PinMethods) {
                case PinMethods.Pin:
                    _ = VirtualDesktop.PinWindow(this);
                    break;
                case PinMethods.Unpin:
                    _ = VirtualDesktop.UnpinWindow(this);
                    break;
                case PinMethods.None:
                default:
                    break;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (WindowState == WindowState.Normal) {
                App.Inst.Config.Left = Left;
                App.Inst.Config.Top = Top;
                App.Inst.Config.Width = Width;
                App.Inst.Config.Height = Height;
                App.Inst.Config.Maximized = false;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e) {
            if (PresentationSource.FromVisual(this) is PresentationSource presentationSource) {
                var leftTop = presentationSource.CompositionTarget.TransformToDevice.Transform(new Point(Left, Top));
                if (WindowState == WindowState.Normal && ((int)leftTop.X) != -32000 && ((int)leftTop.Y) != -32000) {
                    App.Inst.Config.Left = Left;
                    App.Inst.Config.Top = Top;
                    App.Inst.Config.Width = Width;
                    App.Inst.Config.Height = Height;
                    App.Inst.Config.Maximized = false;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Inst.Config.Maximized = WindowState == WindowState.Maximized;
            aboutDlg?.Close();
            aboutDlg = null;
            toast?.Close();
            toast = null;
        }

        private void Window_Activated(object sender, EventArgs e) {
            if (!App.Inst.Config.KeepFolder)
                explorerBrowser?.NavigateToFolder(App.Inst.Config.Folder);
        }

        private void Window_Deactivated(object sender, EventArgs e) {
            if (aboutDlg == null) {
                switch (App.Inst.Config.DeactiveBehavior) {
                    case DeactiveBehavior.Minimize:
                        WindowState = WindowState.Minimized;
                        break;

                    case DeactiveBehavior.MoveToBack:
                        BringToBottom();
                        break;

                    case DeactiveBehavior.None:
                    default:
                        break;
                }
            }
        }

        private void BringToTop() {
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                MyShortcutsInterop.BringToTop(hwndSource.Handle);
        }

        private void BringToBottom() {
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                MyShortcutsInterop.BringToBottom(hwndSource.Handle);
        }

        private static IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) {
            var mainWindow = App.Inst.MainWindow as MainWindow;

            switch ((uint)msg) {
                case (uint)WindowMessages.Activate: {
                        if (mainWindow != null)
                            mainWindow.BringToTop();
                        handled = true;
                    }
                    break;

                case (int)WindowMessages.WM_APPCOMMAND:
                    handled = mainWindow?.explorerBrowser.OnAppCommand(hwnd, wparam, Win32.GET_APPCOMMAND_LPARAM(lparam)) ?? false;
                    break;

                case (int)WindowMessages.WM_COMMAND:
                    handled = mainWindow?.OnWM_Command((ushort)Win32.LOWORD(wparam)) ?? false;
                    break;

                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private bool OnWM_Command(ushort idc) {
            switch (idc) {
                case (ushort)CustomCommands.Backward:
                    BackButton_Click(null, null);
                    break;

                case (ushort)CustomCommands.Forward:
                    ForeButton_Click(null, null);
                    break;

                case (ushort)CustomCommands.Up:
                    UpButton_Click(null, null);
                    break;

                case (ushort)CustomCommands.Home:
                    HomeButton_Click(null, null);
                    break;

                case (ushort)CustomCommands.About:
                    AboutButton_Click(null, null);
                    break;

                case (ushort)CustomCommands.SetHome:
                    OnSetHome();
                    break;

                default:
                    break;
            }
            return false;
        }

        private void OnSetHome() {
            if (explorerBrowser != null && !string.IsNullOrWhiteSpace(explorerBrowser.ParsingName)) {
                App.Inst.Config.Folder = explorerBrowser.ParsingName;
                ShowToast(string.Format("{0}을 홈으로 지정하였습니다.", explorerBrowser.EditName));
            }
        }

        private void ShowToast(string msg) {
            toast?.Close();
            toast = new ToastWindow(msg) { Left = Left, Top = Top, Owner = this };
            toast.Show();
        }

        private void ExplorerBrowserHolder_SizeChanged(object sender, SizeChangedEventArgs e) {
            if (explorerBrowser != null && explorerBrowser.Handle != IntPtr.Zero)
                MyShortcutsInterop.FitToParent(explorerBrowser.Handle);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {
            if (explorerBrowser != null)
                explorerBrowser.NavigateTo(MyShortcutsInterop.NavigateTarget.Backward);
        }

        private void UpButton_Click(object sender, RoutedEventArgs e) {
            if (explorerBrowser != null)
                explorerBrowser.NavigateTo(MyShortcutsInterop.NavigateTarget.Up);
        }

        private void ForeButton_Click(object sender, RoutedEventArgs e) {
            if (explorerBrowser != null)
                explorerBrowser.NavigateTo(MyShortcutsInterop.NavigateTarget.Forward);
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            if (explorerBrowser != null)
                explorerBrowser.NavigateTo(MyShortcutsInterop.NavigateTarget.Home);
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e) {
            aboutDlg = new About() { Owner = this };
            _ = aboutDlg.ShowDialog();
            aboutDlg = null;
        }

        public bool IsDialogMessage(ref MSG msg) {
            if (aboutDlg != null && PresentationSource.FromVisual(aboutDlg) is HwndSource hwndSource) {
                return Win32.IsDialogMessageW(hwndSource.Handle, ref msg) != 0;
            }

            return false;
        }
    }
}
