using System;
using System.Runtime.InteropServices;

namespace Onigwrap.Internal
{
    internal unsafe class OnigInterop
    {
        private const string ONIGWRAP = "onigwrap";
        private const CharSet charSet = CharSet.Unicode;
        private const CallingConvention convention = CallingConvention.Cdecl;

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern IntPtr onigwrap_create(char* pattern, int len, int ignoreCase, int multiline);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern IntPtr onigwrap_region_new();

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern void onigwrap_region_free(IntPtr region);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern void onigwrap_free(IntPtr regex);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern int onigwrap_search(IntPtr regex, char* text, int offset, int length, IntPtr region);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern int onigwrap_num_regs(IntPtr region);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern int onigwrap_pos(IntPtr region, int nth);

        [DllImport(ONIGWRAP, CharSet = charSet, CallingConvention = convention)]
        public static extern int onigwrap_len(IntPtr region, int nth);
    }
}
