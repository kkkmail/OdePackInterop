using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OdePackInterop;
using OdePackInterop.Sets;
using OdePackInterop.SolverDescriptors;
using static OdePackInterop.Interop;

namespace OdePackInteropTest
{
    class Program
    {
        const double fwdCoeff = 0.1;
        const double bkwCoeff = 0.1;

        static unsafe void FImpl(
            ref int neq,
            ref double t,
            double* y,
            double* ydot)
        {
            ydot[0] = -fwdCoeff * y[0] + bkwCoeff * y[1] * y[2];
            ydot[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
            ydot[2] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
        }

        static unsafe void JacImpl(
            ref int neq,
            ref double t,
            double* y,
            ref int ml,
            ref int mu,
            double** pd,
            ref int nrowpd)
        {
            throw new NotSupportedException("Using Jacobian is not supoprted yet.");
        }

        // static void Main(string[] args)
        // {
        //     Console.WriteLine("Calling DLSODE...");
        //
        //     unsafe
        //     {
        //         F f = FImpl;
        //         JAC jac = JacImpl;
        //
        //         var neq = 3;
        //
        //         var y = new double[3];
        //         y[0] = 1.0;
        //         y[1] = 0.2;
        //         y[2] = 0.1;
        //
        //         var t = 0.0;
        //         var tout = 10.0;
        //         var itol = 2;
        //
        //         var rtol = new double[3];
        //         rtol[0] = 1.0e-4;
        //         rtol[1] = 1.0e-4;
        //         rtol[2] = 1.0e-4;
        //
        //         var atol = new double[3];
        //         atol[0] = 1.0e-6;
        //         atol[1] = 1.0e-6;
        //         atol[2] = 1.0e-6;
        //
        //         var itask = 1;
        //         var istate = 1;
        //         var iopt = 0;
        //         var mf = 23;
        //         var lrw = 100;
        //         var liw = 100;
        //
        //         var iwork = new int[liw];
        //         var rwork = new double[lrw];
        //
        //         // Pin everything so that GC won't move the things around...
        //         fixed (double* _ = &y[0], __ = &rtol[0], ___ = &atol[0], ____ = &rwork[0])
        //         {
        //             fixed (int* _____ = &iwork[0])
        //             {
        //                 DLSODE(
        //                     f,
        //                     ref neq,
        //                     y,
        //                     ref t,
        //                     ref tout,
        //                     ref itol,
        //                     rtol,
        //                     atol,
        //                     ref itask,
        //                     ref istate,
        //                     ref iopt,
        //                     rwork,
        //                     ref lrw,
        //                     iwork,
        //                     ref liw,
        //                     jac,
        //                     ref mf);
        //             }
        //         }
        //
        //         Console.WriteLine($"At t = {t}, y[0] = {y[0]}, y[1] = {y[1]}, y[2] = {y[2]}");
        //         Console.WriteLine($"No. steps = {iwork[10]}, No. f-s = {iwork[11]}, No. J-s = {iwork[12]}");
        //         Console.ReadLine();
        //     }
        // }

        static void Main(string[] args)
        {
            Console.WriteLine("Calling DLSODE...");

            var solverParam = new SolverParams(
                // MF = 23, No. steps = 53, No. f-s = 75, No. J-s = 13, Elapsed: 00:00:00.0239851
                //new ChordWithDiagonalJacobianSolver(3, SolutionMethod.Bdf),

                //
                new ChordWithDiagonalJacobianSolver(3, SolutionMethod.Adams),

                // MF = 10, No. steps = 331076, No. f-s = 606038, No. J-s = 0, Elapsed: 00:00:00.0875339
                //new FunctionalSolver(3, SolutionMethod.Adams),

                // MF = 20, No. steps = 336412, No. f-s = 616700, No. J-s = 0, Elapsed: 00:00:00.0884548
                //new FunctionalSolver(3, SolutionMethod.Bdf),

                new[]
                {
                    1.0,
                    0.2,
                    0.1
                })
            {
                StartTime = 0.0,
                EndTime = 1.0e06,
            };

            var sw = new Stopwatch();
            sw.Start();

            unsafe
            {
                OdeSolver.Run(solverParam, FImpl);
            }

            var elapsed = sw.Elapsed;
            Console.WriteLine($"Elapsed: {elapsed}");
            Console.ReadLine();
        }
    }
}
