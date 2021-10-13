using System;
using System.Runtime.InteropServices;

namespace MyShortcuts {
    #region Virtual Desktop
    internal enum APPLICATION_VIEW_COMPATIBILITY_POLICY : int {
        AVCP_NONE = 0,
        AVCP_SMALL_SCREEN = 1,
        AVCP_TABLET_SMALL_SCREEN = 2,
        AVCP_VERY_SMALL_SCREEN = 3,
        AVCP_HIGH_SCALE_FACTOR = 4
    }

    internal enum APPLICATION_VIEW_CLOAK_TYPE : int {
        AVCT_NONE = 0,
        AVCT_DEFAULT = 1,
        AVCT_VIRTUAL_DESKTOP = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Size {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9")]
    internal interface IObjectArray {
        void GetCount(out int count);
        void GetAt(int index, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object obj);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIInspectable)]
    [Guid("372E1D3B-38D3-42E4-A15B-8AB2B178F513")]
    internal interface IApplicationView {
        int SetFocus();
        int SwitchTo();
        int TryInvokeBack(IntPtr /* IAsyncCallback* */ callback);
        int GetThumbnailWindow(out IntPtr hwnd);
        int GetMonitor(out IntPtr /* IImmersiveMonitor */ immersiveMonitor);
        int GetVisibility(out int visibility);
        int SetCloak(APPLICATION_VIEW_CLOAK_TYPE cloakType, int unknown);
        int GetPosition(ref Guid guid /* GUID for IApplicationViewPosition */, out IntPtr /* IApplicationViewPosition** */ position);
        int SetPosition(ref IntPtr /* IApplicationViewPosition* */ position);
        int InsertAfterWindow(IntPtr hwnd);
        int GetExtendedFramePosition(out Rect rect);
        int GetAppUserModelId([MarshalAs(UnmanagedType.LPWStr)] out string id);
        int SetAppUserModelId(string id);
        int IsEqualByAppUserModelId(string id, out int result);
        int GetViewState(out uint state);
        int SetViewState(uint state);
        int GetNeediness(out int neediness);
        int GetLastActivationTimestamp(out ulong timestamp);
        int SetLastActivationTimestamp(ulong timestamp);
        int GetVirtualDesktopId(out Guid guid);
        int SetVirtualDesktopId(ref Guid guid);
        int GetShowInSwitchers(out int flag);
        int SetShowInSwitchers(int flag);
        int GetScaleFactor(out int factor);
        int CanReceiveInput(out bool canReceiveInput);
        int GetCompatibilityPolicyType(out APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
        int SetCompatibilityPolicyType(APPLICATION_VIEW_COMPATIBILITY_POLICY flags);
        int GetSizeConstraints(IntPtr /* IImmersiveMonitor* */ monitor, out Size size1, out Size size2);
        int GetSizeConstraintsForDpi(uint uint1, out Size size1, out Size size2);
        int SetSizeConstraintsForDpi(ref uint uint1, ref Size size1, ref Size size2);
        int OnMinSizePreferencesUpdated(IntPtr hwnd);
        int ApplyOperation(IntPtr /* IApplicationViewOperation* */ operation);
        int IsTray(out bool isTray);
        int IsInHighZOrderBand(out bool isInHighZOrderBand);
        int IsSplashScreenPresented(out bool isSplashScreenPresented);
        int Flash();
        int GetRootSwitchableOwner(out IApplicationView rootSwitchableOwner);
        int EnumerateOwnershipTree(out IObjectArray ownershipTree);
        int GetEnterpriseId([MarshalAs(UnmanagedType.LPWStr)] out string enterpriseId);
        int IsMirrored(out bool isMirrored);
        int Unknown1(out int unknown);
        int Unknown2(out int unknown);
        int Unknown3(out int unknown);
        int Unknown4(out int unknown);
        int Unknown5(out int unknown);
        int Unknown6(int unknown);
        int Unknown7();
        int Unknown8(out int unknown);
        int Unknown9(int unknown);
        int Unknown10(int unknownX, int unknownY);
        int Unknown11(int unknown);
        int Unknown12(out Size size1);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("1841C6D7-4F9D-42C0-AF41-8747538F10E5")]
    internal interface IApplicationViewCollection {
        int GetViews(out IObjectArray array);
        int GetViewsByZOrder(out IObjectArray array);
        int GetViewsByAppUserModelId(string id, out IObjectArray array);
        int GetViewForHwnd(IntPtr hwnd, out IApplicationView view);
        int GetViewForApplication(object application, out IApplicationView view);
        int GetViewForAppUserModelId(string id, out IApplicationView view);
        int GetViewInFocus(out IntPtr view);
        int Unknown1(out IntPtr view);
        void RefreshCollection();
        int RegisterForApplicationViewChanges(object listener, out int cookie);
        int UnregisterForApplicationViewChanges(int cookie);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
    internal interface IServiceProvider10 {
        [return: MarshalAs(UnmanagedType.IUnknown)]
        object QueryService(ref Guid service, ref Guid riid);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("A5CD92FF-29BE-454C-8D04-D82879FB3F1B")]
    internal interface IVirtualDesktopManager {
        bool IsWindowOnCurrentVirtualDesktop(IntPtr topLevelWindow);
        Guid GetWindowDesktopId(IntPtr topLevelWindow);
        void MoveWindowToDesktop(IntPtr topLevelWindow, ref Guid desktopId);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("4CE81583-1E4C-4632-A621-07A53543148F")]
    internal interface IVirtualDesktopPinnedApps {
        bool IsAppIdPinned(string appId);
        void PinAppID(string appId);
        void UnpinAppID(string appId);
        bool IsViewPinned(IApplicationView applicationView);
        void PinView(IApplicationView applicationView);
        void UnpinView(IApplicationView applicationView);
    }
    #endregion

    #region Explorer Browser
    internal enum FOLDERVIEWMODE : int {
        FVM_AUTO = -1,
        FVM_FIRST = 1,
        FVM_ICON = 1,
        FVM_SMALLICON = 2,
        FVM_LIST = 3,
        FVM_DETAILS = 4,
        FVM_THUMBNAIL = 5,
        FVM_TILE = 6,
        FVM_THUMBSTRIP = 7,
        FVM_CONTENT = 8,
        FVM_LAST = 8
    }

    [Flags]
    internal enum FOLDERFLAGS : uint {
        FWF_NONE = 0,
        FWF_AUTOARRANGE = 0x1,
        FWF_ABBREVIATEDNAMES = 0x2,
        FWF_SNAPTOGRID = 0x4,
        FWF_OWNERDATA = 0x8,
        FWF_BESTFITWINDOW = 0x10,
        FWF_DESKTOP = 0x20,
        FWF_SINGLESEL = 0x40,
        FWF_NOSUBFOLDERS = 0x80,
        FWF_TRANSPARENT = 0x100,
        FWF_NOCLIENTEDGE = 0x200,
        FWF_NOSCROLL = 0x400,
        FWF_ALIGNLEFT = 0x800,
        FWF_NOICONS = 0x1000,
        FWF_SHOWSELALWAYS = 0x2000,
        FWF_NOVISIBLE = 0x4000,
        FWF_SINGLECLICKACTIVATE = 0x8000,
        FWF_NOWEBVIEW = 0x10000,
        FWF_HIDEFILENAMES = 0x20000,
        FWF_CHECKSELECT = 0x40000,
        FWF_NOENUMREFRESH = 0x80000,
        FWF_NOGROUPING = 0x100000,
        FWF_FULLROWSELECT = 0x200000,
        FWF_NOFILTERS = 0x400000,
        FWF_NOCOLUMNHEADER = 0x800000,
        FWF_NOHEADERINALLVIEWS = 0x1000000,
        FWF_EXTENDEDTILES = 0x2000000,
        FWF_TRICHECKSELECT = 0x4000000,
        FWF_AUTOCHECKSELECT = 0x8000000,
        FWF_NOBROWSERVIEWSTATE = 0x10000000,
        FWF_SUBSETGROUPS = 0x20000000,
        FWF_USESEARCHFOLDER = 0x40000000,
        FWF_ALLOWRTLREADING = 0x80000000
    }

    [Flags]
    internal enum EXPLORER_BROWSER_OPTIONS : int {
        EBO_NONE = 0,
        EBO_NAVIGATEONCE = 0x1,
        EBO_SHOWFRAMES = 0x2,
        EBO_ALWAYSNAVIGATE = 0x4,
        EBO_NOTRAVELLOG = 0x8,
        EBO_NOWRAPPERWINDOW = 0x10,
        EBO_HTMLSHAREPOINTVIEW = 0x20,
        EBO_NOBORDER = 0x40,
        EBO_NOPERSISTVIEWSTATE = 0x80
    }

    internal enum SORTDIRECTION : int {
        SORT_DESCENDING = -1,
        SORT_ASCENDING = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FOLDERSETTINGS {
        public FOLDERVIEWMODE ViewMode;
        public FOLDERFLAGS fFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PROPERTYKEY {
        public Guid fmtid;
        public uint pid;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SORTCOLUMN {
        public PROPERTYKEY propkey;
        public SORTDIRECTION direction;
    }
    #endregion

    internal static class Guids {
        public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        public static readonly Guid CLSID_VirtualDesktopManager = new Guid("AA509086-5CA9-4C25-8F95-589D3C07B48A");
        public static readonly Guid CLSID_VirtualDesktopPinnedApps = new Guid("B5A399E7-1C87-46B8-88E9-FC5747B171BD");
        public static readonly Guid PKEY_ItemNameDisplay = new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC");
    }
}
