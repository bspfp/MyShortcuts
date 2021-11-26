using System;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace MyShortcuts {
    public partial class MainWindow : Window {
        public static MainWindow Inst => App.Inst.MainWindow as MainWindow;

        private ExplorerBrowser explorerBrowser;
        private About aboutDlg = null;
        private Timer toastTimer = null;
        private int toastStartTick = 0;

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
            if (App.Inst.Config.UseSingleClick)
                explorerBrowser.DefaultFolderSettings.fFlags |= FOLDERFLAGS.FWF_SINGLECLICKACTIVATE;
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
            if (!App.Inst.Config.FixedPosition && WindowState == WindowState.Normal) {
                App.Inst.Config.Left = Left;
                App.Inst.Config.Top = Top;
                App.Inst.Config.Width = Width;
                App.Inst.Config.Height = Height;
                App.Inst.Config.Maximized = false;
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e) {
            if (!App.Inst.Config.FixedPosition) {
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
        }

        protected override void OnStateChanged(EventArgs e) {
            base.OnStateChanged(e);
            if (WindowState == WindowState.Normal && App.Inst.Config.FixedPosition) {
                Left = App.Inst.Config.Left;
                Top = App.Inst.Config.Top;
                Width = App.Inst.Config.Width;
                Height = App.Inst.Config.Height;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            App.Inst.Config.Maximized = WindowState == WindowState.Maximized;
            aboutDlg?.Close();
            aboutDlg = null;
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

        private IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) {
            switch ((uint)msg) {
                case (uint)WindowMessages.Activate: {
                        BringToTop();
                        handled = true;
                    }
                    break;

                case (int)WindowMessages.WM_APPCOMMAND:
                    if (explorerBrowser != null)
                        handled = explorerBrowser?.OnAppCommand(hwnd, wparam, Win32.GET_APPCOMMAND_LPARAM(lparam)) ?? false;
                    break;

                case (int)WindowMessages.WM_COMMAND:
                    handled = OnWM_Command((ushort)Win32.LOWORD(wparam));
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

                case (ushort)CustomCommands.ChangeDeactiveBehavior:
                    OnChangeDeactiveBehavior();
                    break;

                case (ushort)CustomCommands.ChangePinMethod:
                    OnChangePinMethod();
                    break;

                case (ushort)CustomCommands.ChangeKeepFolder:
                    OnChangeKeepFolder();
                    break;

                case (ushort)CustomCommands.ChangeFixedPosition:
                    OnChangeFixedPosition();
                    break;

                default:
                    break;
            }
            return false;
        }

        private void OnChangeDeactiveBehavior() {
            switch (App.Inst.Config.NextDeactiveBehavior()) {
                case DeactiveBehavior.None:
                    ShowToast("비활성화 동작 설정 변경: 그대로 남겨 두기");
                    break;
                case DeactiveBehavior.Minimize:
                    ShowToast("비활성화 동작 설정 변경: 최소화 하기");
                    break;
                case DeactiveBehavior.MoveToBack:
                    ShowToast("비활성화 동작 설정 변경: 뒤로 보내기");
                    break;
                default:
                    break;
            }
        }

        private void OnChangePinMethod() {
            switch (App.Inst.Config.NextPinMethod()) {
                case PinMethods.None:
                    ShowToast("창 고정 설정 변경: 현재 상태 유지");
                    break;
                case PinMethods.Pin:
                    ShowToast("창 고정 설정 변경: 창을 고정 시키기");
                    _ = VirtualDesktop.PinWindow(this);
                    break;
                case PinMethods.Unpin:
                    ShowToast("창 고정 설정 변경: 창 고정을 해제");
                    _ = VirtualDesktop.UnpinWindow(this);
                    break;
                default:
                    break;
            }
        }

        private void OnChangeKeepFolder() {
            if (App.Inst.Config.NextKeepFolder())
                ShowToast("활성화 되면서 이전 폴더가 보여집니다.");
            else
                ShowToast("활성화 되면서 지정 폴더가 보여집니다.");
        }

        private void OnChangeFixedPosition() {
            if (App.Inst.Config.NextFixedPosition())
                ShowToast("창의 위치와 크기를 고정합니다.");
            else
                ShowToast("창의 위치와 크기의 변경을 기록합니다.");
        }

        private void OnSetHome() {
            if (explorerBrowser != null && !string.IsNullOrWhiteSpace(explorerBrowser.ParsingName)) {
                App.Inst.Config.Folder = explorerBrowser.ParsingName;
                ShowToast(string.Format("{0}을 홈으로 지정하였습니다.", explorerBrowser.EditName));
            }
        }

        private void ShowToast(string msg) {
            toastMsg.Visibility = Visibility.Visible;
            toastMsg.Opacity = 0;
            toastMsg.Text = msg;
            toastStartTick = Environment.TickCount;
            toastTimer?.Dispose();
            toastTimer = new Timer(state => {
                _ = Dispatcher.BeginInvoke(new Action(() => {
                    var id = (int)state;
                    if (id == toastStartTick) {
                        var param1 = 300.0;
                        var param2 = 2000;

                        var elapsed = (uint)(Environment.TickCount - toastStartTick);
                        if (elapsed < param1) {
                            toastMsg.Opacity = elapsed / param1;
                        }
                        else if (elapsed < param2 - param1) {
                            toastMsg.Opacity = 1.0;
                        }
                        else if (elapsed < param2) {
                            toastMsg.Opacity = 1.0 - ((elapsed - (param2 - param1)) / param1);
                        }
                        else {
                            toastMsg.Visibility = Visibility.Hidden;
                            toastMsg.Opacity = 0;
                            toastMsg.Text = "";
                            toastStartTick = 0;
                            toastTimer?.Dispose();
                            toastTimer = null;
                        }
                    }
                }));
            }, toastStartTick, 50, 50);
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
            return aboutDlg != null
                && PresentationSource.FromVisual(aboutDlg) is HwndSource hwndSource
                && Win32.IsDialogMessageW(hwndSource.Handle, ref msg) != 0;
        }
    }
}
