using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MyShortcuts {
    internal class MyShortcutsInterop {
        internal delegate void NavigationCompleteCallback(string parsingName, string editName, string normalName, bool isFileSystemFolder);
        internal delegate void NavigationFailedCallback(string parsingName, string editName, string normalName, bool isFileSystemFolder);

        internal enum NavigateTarget : int {
            Backward,
            Forward,
            Up,
            Home
        }

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void BringToTop(IntPtr hwndMain);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr CreateExplorerBrowser(IntPtr hwndParent, ref FOLDERSETTINGS folderSettings, EXPLORER_BROWSER_OPTIONS browserOptions, ref SORTCOLUMN sortColumn,
            string homeFolder, NavigationCompleteCallback completeCallback, NavigationFailedCallback failedCallback);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void DestroyExplorerBrowser(IntPtr hwnd);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void FitToParent(IntPtr hwnd);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void SetFocusToShellView(IntPtr hwnd);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern bool PreTranslateMessage(IntPtr hwnd, ref MSG msg);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern bool OnAppCommand(IntPtr hwnd, IntPtr child, uint cmd);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void NavigateToFolder(IntPtr hwnd, string path);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void NavigateTo(IntPtr hwnd, NavigateTarget target);

        [DllImport("MyShortcutsInterop.dll", CharSet = CharSet.Unicode)]
        internal static extern void BringToBottom(IntPtr hwndMain);
    }
}
