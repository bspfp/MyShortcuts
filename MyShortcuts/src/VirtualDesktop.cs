// Original Author: Markus Scholtes, 2021
// Version 1.9alpha, 2021-07-13
// Version for Windows Insider (10 21H2 and 11)
// https://github.com/MScholtes/VirtualDesktop

using System;
using System.Windows;
using System.Windows.Interop;

namespace MyShortcuts {
    internal class VirtualDesktop {

        public static bool PinWindow(Window wnd) {
            try {
                if (wnd != null) {
                    if (PresentationSource.FromVisual(wnd) is HwndSource hwnd) {
                        if (ApplicationViewCollection.GetViewForHwnd(hwnd.Handle, out var view) == 0) {
                            if (!VirtualDesktopPinnedApps.IsViewPinned(view)) {
                                VirtualDesktopPinnedApps.PinView(view);
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool UnpinWindow(Window wnd) {
            try {
                if (wnd != null) {
                    if (PresentationSource.FromVisual(wnd) is HwndSource hwnd) {
                        if (ApplicationViewCollection.GetViewForHwnd(hwnd.Handle, out var view) == 0) {
                            if (VirtualDesktopPinnedApps.IsViewPinned(view)) {
                                VirtualDesktopPinnedApps.UnpinView(view);
                                return true;
                            }
                        }
                    }
                }
            }
            catch { }
            return false;
        }

        public static bool IsWindowOnCurrentVirtualDesktop(Window wnd, bool returnValueOnException = true) {
            try {
                if (wnd != null) {
                    if (PresentationSource.FromVisual(wnd) is HwndSource hwnd)
                        return VirtualDesktopManager.IsWindowOnCurrentVirtualDesktop(hwnd.Handle);
                }
            }
            catch { }
            return returnValueOnException;
        }

        public static Guid GetWindowDesktopId(Window wnd) {
            try {
                var hwndHandle = IntPtr.Zero;
                if (wnd != null && PresentationSource.FromVisual(wnd) is HwndSource hwnd)
                    return GetWindowDesktopId(hwnd.Handle);
            }
            catch { }
            return Guid.Empty;
        }
        public static Guid GetWindowDesktopId(IntPtr hwnd) {
            try {
                if (hwnd != IntPtr.Zero)
                    return VirtualDesktopManager.GetWindowDesktopId(hwnd);
            }
            catch { }
            return Guid.Empty;
        }

        public static void MoveWindowToDesktop(Window wnd, ref Guid desktopId) {
            try {
                if (wnd != null) {
                    if (PresentationSource.FromVisual(wnd) is HwndSource hwnd)
                        VirtualDesktopManager.MoveWindowToDesktop(hwnd.Handle, desktopId);
                }
            }
            catch { }
        }

        static VirtualDesktop() {
            var shell = (IServiceProvider10)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_ImmersiveShell));
            VirtualDesktopManager = (IVirtualDesktopManager)Activator.CreateInstance(Type.GetTypeFromCLSID(Guids.CLSID_VirtualDesktopManager));
            ApplicationViewCollection = (IApplicationViewCollection)shell.QueryService(typeof(IApplicationViewCollection).GUID, typeof(IApplicationViewCollection).GUID);
            VirtualDesktopPinnedApps = (IVirtualDesktopPinnedApps)shell.QueryService(Guids.CLSID_VirtualDesktopPinnedApps, typeof(IVirtualDesktopPinnedApps).GUID);
        }

        internal static IVirtualDesktopManager VirtualDesktopManager;
        internal static IApplicationViewCollection ApplicationViewCollection;
        internal static IVirtualDesktopPinnedApps VirtualDesktopPinnedApps;
    }
}
