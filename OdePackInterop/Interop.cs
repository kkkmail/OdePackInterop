using System.Runtime.InteropServices;

namespace OdePackInterop
{
    /// <summary>
    /// FORTRAN source code: https://www.netlib.org/odepack/
    ///
    /// https://www.codeproject.com/Articles/1099942/FORTRAN-Interoperability-with-NET-Exchanging-Compl
    /// http://xtechnotes.blogspot.com/2008/07/callback-to-c-from-unmanaged-fortran.html
    /// https://stackoverflow.com/questions/17549123/c-sharp-performance-using-unsafe-pointers-instead-of-intptr-and-marshal
    /// </summary>
    public static class Interop
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void F(
            ref int neq,
            ref double t,
            double* y,
            double* yDot);

        /// <summary>
        /// !!! DO NOT USE AS JACOBIAN IS NOT SUPPORTED YET !!!
        /// </summary>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void JAC(
            ref int neq,
            ref double t,
            double* y,
            ref int ml,
            ref int mu,
            double** pd,
            ref int nrowpd);

        [DllImport("OdePack.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "DLSODEINTEROP")]
        public static extern unsafe void DLSODE(
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
