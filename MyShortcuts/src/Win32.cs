using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MyShortcuts {
    public enum WindowMessages : uint {
        WM_APPCOMMAND = 0x0319,
        WM_COMMAND = 0x0111,
        WM_APP = 0x8000,
        Activate = WM_APP + 100,
    }

    public enum AppCommands : uint {

        APPCOMMAND_BROWSER_BACKWARD = 1,
        APPCOMMAND_BROWSER_FORWARD = 2,
        APPCOMMAND_BROWSER_REFRESH = 3,
        APPCOMMAND_BROWSER_HOME = 7
    }

    [Flags]
    public enum AccelFVirt : byte {
        FVIRTKEY = 0x01,
        FNOINVERT = 0x02,
        FSHIFT = 0x04,
        FCONTROL = 0x08,
        FALT = 0x10,
    }

    public enum AccelVirtualKeys : ushort {
        VK_HOME = 0x24,
        VK_LEFT = 0x25,
        VK_UP = 0x26,
        VK_RIGHT = 0x27,
        VK_INSERT = 0x2D,
        VK_F1 = 0x70,
    }

    public enum CustomCommands : ushort {
        Home = 100,
        Backward,
        Up,
        Forward,
        About,
        SetHome,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ACCEL {
        public AccelFVirt fVirt;
        public AccelVirtualKeys key;
        public CustomCommands cmd;
    }

    internal class Win32 {
        public const uint FAPPCOMMAND_MASK = 0xF000;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hwnd, uint msg, uint wparam, uint lparam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateAcceleratorTableW([In, Out] ACCEL[] paccel, int cAccel);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int TranslateAcceleratorW(IntPtr hWnd, IntPtr hAccTable, ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int IsDialogMessageW(IntPtr hDlg, ref MSG lpMsg);

        public static uint HIWORD(IntPtr l) {
            return (ushort)((((ulong)l.ToInt64()) >> 16) & 0xffff);
        }

        public static uint LOWORD(IntPtr l) {
            return (ushort)(((ulong)l) & 0xffff);
        }

        public static uint GET_APPCOMMAND_LPARAM(IntPtr lparam) {
            return HIWORD(lparam) & ~FAPPCOMMAND_MASK;
        }
    }
}
