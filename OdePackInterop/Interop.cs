using System;
using System.Text;
using System.Runtime.InteropServices;

namespace OdePackInterop
{
    public static class Interop
    {
        [DllImport("OdePack.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DUMSUM")]
        public static extern void DUMSUM(ref double a, ref double b, ref double c);
    }
}
