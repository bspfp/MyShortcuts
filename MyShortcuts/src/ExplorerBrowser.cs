using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MyShortcuts {
    internal class ExplorerBrowser : HwndHost {
        public string ParsingName { get; private set; } = "";
        public string EditName { get; private set; } = "";
        public string NormalName { get; private set; } = "";
        public bool IsFileSystemFolder { get; private set; } = false;

        private FOLDERSETTINGS DefaultFolderSettings = new FOLDERSETTINGS() {
            ViewMode = FOLDERVIEWMODE.FVM_ICON,
            fFlags = FOLDERFLAGS.FWF_AUTOARRANGE | FOLDERFLAGS.FWF_SHOWSELALWAYS | FOLDERFLAGS.FWF_NOCOLUMNHEADER
        };
        private readonly EXPLORER_BROWSER_OPTIONS DefaultOptions = EXPLORER_BROWSER_OPTIONS.EBO_ALWAYSNAVIGATE;
        private SORTCOLUMN DefaultSortColumn = new SORTCOLUMN() {
            propkey = new PROPERTYKEY() { fmtid = Guids.PKEY_ItemNameDisplay, pid = 10 },
            direction = SORTDIRECTION.SORT_ASCENDING
        };

        private MyShortcutsInterop.NavigationCompleteCallback completeCallback;
        private MyShortcutsInterop.NavigationFailedCallback failedCallback;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
            completeCallback = new MyShortcutsInterop.NavigationCompleteCallback(OnNavigationComplete);
            failedCallback = new MyShortcutsInterop.NavigationFailedCallback(OnNavigationFailed);
            var hwndControl = MyShortcutsInterop.CreateExplorerBrowser(hwndParent.Handle, ref DefaultFolderSettings, DefaultOptions, ref DefaultSortColumn,
                App.Inst.Config.Folder, completeCallback, failedCallback);
            return new HandleRef(this, hwndControl);
        }

        protected override void DestroyWindowCore(HandleRef hwnd) {
            MyShortcutsInterop.DestroyExplorerBrowser(hwnd.Handle);
        }

        public void SetFocusToShellView() {
            MyShortcutsInterop.SetFocusToShellView(Handle);
        }

        public bool PreTranslateMessage(ref MSG msg) {
            return MyShortcutsInterop.PreTranslateMessage(Handle, ref msg);
        }

        public bool OnAppCommand(IntPtr hwnd, IntPtr child, uint cmd) {
            return MyShortcutsInterop.OnAppCommand(hwnd, child, cmd);
        }

        public void NavigateToFolder(string path) {
            MyShortcutsInterop.NavigateToFolder(Handle, path);
        }

        public void NavigateTo(MyShortcutsInterop.NavigateTarget navigateTarge) {
            MyShortcutsInterop.NavigateTo(Handle, navigateTarge);
        }

        public void OnNavigationComplete(string parsingName, string editName, string normalName, bool isFileSystemFolder) {
            ParsingName = parsingName;
            EditName = editName;
            NormalName = normalName;
            IsFileSystemFolder = isFileSystemFolder;
        }

        public void OnNavigationFailed(string parsingName, string editName, string normalName, bool isFileSystemFolder) { }
    }
}
