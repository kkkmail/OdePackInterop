using System;
using System.Text;
using System.Runtime.InteropServices;

namespace OdePackInterop
{
    /// <summary>
    /// https://www.codeproject.com/Articles/1099942/FORTRAN-Interoperability-with-NET-Exchanging-Compl
    /// http://xtechnotes.blogspot.com/2008/07/callback-to-c-from-unmanaged-fortran.html
    /// https://stackoverflow.com/questions/17549123/c-sharp-performance-using-unsafe-pointers-instead-of-intptr-and-marshal
    /// </summary>
    public static class Interop
    {
        [DllImport("OdePack.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DUMSUM")]
        public static unsafe extern void DUMSUM(ref double a, ref double b, ref double c);

        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate void F(
        //    ref int neq,
        //    ref double t,
        //    [In, Out] double[] y,
        //    [In, Out] double[] ydot);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void F(
            ref int neq,
            ref double t,
            double* y,
            double* ydot);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void JAC(
            ref int neq,
            ref double t,
            [In, Out] double[] y,
            ref int ml,
            ref int mu,
            [In, Out] double[][] pd,
            ref int nrowpd);

        [DllImport("OdePack.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DLSODE")]
        public static unsafe extern void DLSODE(
            F f,
            ref int neq,
            [In, Out] double[] y,
            ref double t,
            ref double tout,
            ref int itol,
            [In, Out] double[] rtol,
            [In, Out] double[] atol,
            ref int itask,
            ref int istate,
            ref int iopt,
            [In, Out] double[] rwork,
            ref int lrw,
            [In, Out] int[] iwork,
            ref int liw,
            JAC jac,
            ref int mf);
    }
}
