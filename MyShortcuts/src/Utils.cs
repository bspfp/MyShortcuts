using System.Runtime.CompilerServices;

namespace MyShortcuts {
    internal static class Utils {
        public static string GetMethodName([CallerMemberName] string caller = null) { return caller; }
        public static string GetMethodName(this object type, [CallerMemberName] string caller = null) { return type.GetType().Name + "." + caller; }
    }
}
