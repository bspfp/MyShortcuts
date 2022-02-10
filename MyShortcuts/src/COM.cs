using System;
using System.Runtime.InteropServices;

namespace MyShortcuts {
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

    internal static class Guids {
        public static readonly Guid CLSID_ImmersiveShell = new Guid("C2F03A33-21F5-47FA-B4BB-156362A2F239");
        public static readonly Guid PKEY_ItemNameDisplay = new Guid("B725F130-47EF-101A-A5F1-02608C9EEBAC");
    }
}
