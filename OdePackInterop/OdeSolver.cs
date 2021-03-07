using System;
using System.IO;
using System.Linq;
using OdePackInterop.Sets;
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

        public static SolverResult Run(SolverParams solverParams, F f, JAC? jac = null)
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

            Console.WriteLine($"Using MF = {descriptor.MethodFlag}");

            var neq = descriptor.NumberOfEquations;
            var y = solverParams.InitialValues.Select(e => e).ToArray();
            var t = solverParams.StartTime;
            var tout = solverParams.EndTime;

            // ATOL must be dimensioned at least NEQ.
            var itol = 2;

            var rtol = solverParams.RelativeTolerance.Select(e => e).ToArray();
            var atol = solverParams.AbsoluteTolerance.Select(e => e).ToArray();

            // Flag indicating the task DLSODE is to perform.
            // Use ITASK = 1 for normal computation of output values of y at t = TOUT.
            // Nothing else is supported.
            var itask = 1;

            // This is the first call for a problem.
            // Subsequent calls (istate = 2) are not supported.
            var istate = 1;

            // Tell DLSODE that some optional parameters will be passed.
            var iopt = 1;

            var mf = descriptor.MethodFlag;
            var lrw = descriptor.LRW;
            var liw = descriptor.LIW;

            var iwork = new int[liw];
            var rwork = new double[lrw];

            // Make the number of internal steps nearly infinite.
            // You are responsible for terminating the solver if it runs forever.
            iwork[5] = Int32.MaxValue;

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

                var solverResult = new SolverResult
                {
                    ResultState = ResultState.TryCreate(istate) ?? ResultState.GlobalFailure,
                    T = t,
                    X = y,
                    Steps = iwork[10],
                    FuncCalls = iwork[11],
                    JacobianCalls = iwork[12],
                    RequiredLRW = iwork[16],
                    RequiredLIW = iwork[17],
                };

                return solverResult;
            }
        }
    }
}
