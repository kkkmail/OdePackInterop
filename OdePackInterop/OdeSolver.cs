using System;
using System.IO;
using System.Linq;
using static OdePackInterop.Interop;

namespace OdePackInterop
{
    public static class OdeSolver
    {
        private static unsafe void JacImpl(
            ref int neq,
            ref double t,
            double* y,
            ref int ml,
            ref int mu,
            double** pd,
            ref int nrowpd) =>
            throw new NotSupportedException("Using Jacobian is not supported yet.");

        public static void Run(SolverParams solverParams, F f, JAC? jac = null)
        {
            var descriptor = solverParams.SolverDescriptor;

            if (solverParams.InitialValues.Length != descriptor.NumberOfEquations)
            {
                throw new InvalidDataException(
                    $"Expected length of initial values = {descriptor.NumberOfEquations} but got {solverParams.InitialValues.Length}.");
            }

            if (solverParams.RelativeTolerance.Length != descriptor.NumberOfEquations)
            {
                throw new InvalidDataException(
                    $"Expected length of relative tolerance = {descriptor.NumberOfEquations} but got {solverParams.RelativeTolerance.Length}.");
            }

            if (solverParams.AbsoluteTolerance.Length != descriptor.NumberOfEquations)
            {
                throw new InvalidDataException(
                    $"Expected length of absolute tolerance = {descriptor.NumberOfEquations} but got {solverParams.AbsoluteTolerance.Length}.");
            }

            var neq = descriptor.NumberOfEquations;
            var y = solverParams.InitialValues.Select(e => e).ToArray();
            var t = solverParams.StartTime;
            var tout = solverParams.EndTime;
            var itol = 2;
            var rtol = solverParams.RelativeTolerance.Select(e => e).ToArray();
            var atol = solverParams.AbsoluteTolerance.Select(e => e).ToArray();
            var itask = 1;
            var istate = 1;
            var iopt = 0;
            var mf = descriptor.MethodFlag;
            var lrw = descriptor.LRW;
            var liw = descriptor.LIW;

            var iwork = new int[liw];
            var rwork = new double[lrw];

            unsafe
            {
                jac ??= JacImpl;

                // Pin everything so that GC won't move the things around...
                fixed (double* _ = &y[0], __ = &rtol[0], ___ = &atol[0], ____ = &rwork[0])
                {
                    fixed (int* _____ = &iwork[0])
                    {
                        DLSODE(
                            f,
                            ref neq,
                            y,
                            ref t,
                            ref tout,
                            ref itol,
                            rtol,
                            atol,
                            ref itask,
                            ref istate,
                            ref iopt,
                            rwork,
                            ref lrw,
                            iwork,
                            ref liw,
                            jac,
                            ref mf);
                    }
                }

                Console.WriteLine($"At t = {t}, y[0] = {y[0]}, y[1] = {y[1]}, y[2] = {y[2]}");
                Console.WriteLine($"No. steps = {iwork[10]}, No. f-s = {iwork[11]}, No. J-s = {iwork[12]}");
                Console.ReadLine();
            }
        }
    }
}
