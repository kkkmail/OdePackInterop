using System;
using System.Diagnostics;
using System.Linq;
using OdePackInterop;
using OdePackInterop.Sets;
using OdePackInterop.SolverDescriptors;

// ReSharper disable ArgumentsStyleNamedExpression
namespace OdePackInteropTest
{
    class Program
    {
        const double fwdCoeff = 1.0;
        const double bkwCoeff = 0.1;

        private const int NumberOfPairs = 50000;
        private const int NumberOfEquations = 2 * NumberOfPairs + 1;

        static unsafe void FImpl(
            ref int neq,
            ref double t,
            double* y,
            double* dy)
        {
            dy[0] = 2.0 * (-fwdCoeff * y[0] + bkwCoeff * y[1] * y[2]);
            dy[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
            dy[2] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
        }

        /// <summary>
        /// neq = 2 * n + 1
        /// y[0] --- y[1] + y[2]
        /// y[2] --- y[3] + y[4]
        /// y[4] --- y[5] + y[6]
        /// ...
        /// y[2 * n - 2] --- y[2 * n - 1] + y[2 * n]
        /// </summary>
        static unsafe void FImpl2(
            ref int neq,
            ref double t,
            double* yInpt,
            double* dy)
        {
            var y = new double[neq];
            for (var i = 0; i < neq; i++) y[i] = yInpt[i] > 0.0 ? yInpt[i] : 0.0;

            dy[0] = 2.0 * (-fwdCoeff * y[0] + bkwCoeff * y[1] * y[2]);
            dy[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];

            var numberOfPairs = (neq / 2) - 1;

            for (var i = 1; i <= numberOfPairs; i++)
            {
                dy[2 * i] = fwdCoeff * y[2 * i - 2] - bkwCoeff * y[2 * i - 1] * y[2 * i] +
                            2.0 * (-fwdCoeff * y[2 * i] + bkwCoeff * y[2 * i + 1] * y[2 * i + 2]);

                dy[2 * i + 1] = fwdCoeff * y[2 * i] - bkwCoeff * y[2 * i + 1] * y[2 * i + 2];
            }

            dy[2 * numberOfPairs + 2] = fwdCoeff * y[2 * numberOfPairs] - bkwCoeff * y[2 * numberOfPairs + 1] * y[2 * numberOfPairs + 2];
        }

        private static long CallCount { get; set; }

        static void FImpl(double[] y, double x, double[] dy, object obj)
        {
            CallCount++;

            dy[0] = -fwdCoeff * y[0] + bkwCoeff * y[1] * y[2];
            dy[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
            dy[2] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
        }

        private static DateTime Start { get; set; }

        static void FImpl2(double[] y, double x, double[] dy, object obj)
        {
            CallCount++;
            var neq = y.Length;

            if (CallCount % 1_000_000 == 0) Console.WriteLine($"    t = {x}, CallCount = {CallCount}, run time = {DateTime.Now - Start}.");

            // var y = new double[neq];
            // for (var i = 0; i < neq; i++) y[i] = yInpt[i] > 0.0 ? yInpt[i] : 0.0;

            dy[0] = 2.0 * (-fwdCoeff * y[0] + bkwCoeff * y[1] * y[2]);
            dy[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];

            var numberOfPairs = (neq / 2) - 1;

            for (var i = 1; i <= numberOfPairs; i++)
            {
                dy[2 * i] = fwdCoeff * y[2 * i - 2] - bkwCoeff * y[2 * i - 1] * y[2 * i] +
                            2.0 * (-fwdCoeff * y[2 * i] + bkwCoeff * y[2 * i + 1] * y[2 * i + 2]);

                dy[2 * i + 1] = fwdCoeff * y[2 * i] - bkwCoeff * y[2 * i + 1] * y[2 * i + 2];
            }

            dy[2 * numberOfPairs + 2] = fwdCoeff * y[2 * numberOfPairs] - bkwCoeff * y[2 * numberOfPairs + 1] * y[2 * numberOfPairs + 2];
        }

        static void Main(string[] args)
        {
            var numberOfEquations = NumberOfEquations;
            Console.WriteLine(
                $"Number of equations: {numberOfEquations}, DefaultAbsoluteTolerance: {SolverParams.DefaultAbsoluteTolerance}.\n");
            RunDLSODE(numberOfEquations, SolutionMethod.Bdf, CorrectorIteratorMethod.ChordWithDiagonalJacobian);
            RunDLSODE(numberOfEquations, SolutionMethod.Adams, CorrectorIteratorMethod.ChordWithDiagonalJacobian);
            RunDLSODE(numberOfEquations, SolutionMethod.Bdf, CorrectorIteratorMethod.Functional);
            RunDLSODE(numberOfEquations, SolutionMethod.Adams, CorrectorIteratorMethod.Functional);

            if (numberOfEquations <= 50)
            {
                RunAlgLib(numberOfEquations);
            }
        }

        static double[] GetInitialValues(int numberOfEquations) =>
            Enumerable.Range(0, numberOfEquations)
                .Select(e => e == 0 ? 10.0 : 0.0)
                .ToArray();

        private static double StartTime = 0.0;
        private static double EndTime = 1.0e06;

        static void RunDLSODE(int numberOfEquations, SolutionMethod solutionMethod, CorrectorIteratorMethod iteratorMethod)
        {
            Console.WriteLine("Calling DLSODE...");

            SolverParams getChordWithDiagonalJacobianSolver() =>
                new(new ChordWithDiagonalJacobianSolver(numberOfEquations, solutionMethod),
                    GetInitialValues(numberOfEquations))
                {
                    StartTime = StartTime,
                    EndTime = EndTime,
                };

            SolverParams getFunctionalSolver() =>
                new(new FunctionalSolver(numberOfEquations, solutionMethod),
                    GetInitialValues(numberOfEquations))
                {
                    StartTime = StartTime,
                    EndTime = EndTime,
                };

            SolverParams throwNotSupported() =>
                throw new NotSupportedException($"Iterator method: {iteratorMethod} is not supported.");

            var solverParam = iteratorMethod.Switch(
                onFunctional: getFunctionalSolver,
                onChordWithUserJacobian: throwNotSupported,
                onChordWithGeneratedJacobian: throwNotSupported,
                onChordWithDiagonalJacobian: getChordWithDiagonalJacobianSolver,
                onChordWithBandedUserJacobian: throwNotSupported,
                onChordWithBandedGeneratedJacobian: throwNotSupported);

            var sw = new Stopwatch();
            sw.Start();

            unsafe
            {
                OdeSolver.Run(solverParam, FImpl2);
            }

            var elapsed = sw.Elapsed;
            Console.WriteLine($"Elapsed: {elapsed}\n\n");
        }

        static void RunAlgLib(int numberOfEquations)
        {
            Console.WriteLine("Calling OdeSolverSolve.");
            Start = DateTime.Now;
            var stepSize = 0.0;

            var x = new[]
            {
                StartTime,
                EndTime,
            };

            alglib.odesolverrkck(GetInitialValues(numberOfEquations), x, SolverParams.DefaultAbsoluteTolerance, stepSize, out var s);

            var sw = new Stopwatch();
            sw.Start();
            alglib.odesolversolve(s, FImpl2, null);
            alglib.odesolverresults(s, out var m, out var xTbl, out var yTbl, out var rep);

            var y = Enumerable.Range(0, numberOfEquations).Select(i => yTbl[1, i]).ToArray();
            var n = numberOfEquations <= 1001 ? numberOfEquations : 100;
            Console.WriteLine($"At t = {xTbl[1]:N2}\n    Total = {y.Sum()}");
            Console.WriteLine($"{string.Join("\n", y.Take(n).Select((e, i) => $"    y[{i}] = {e}"))}");
            Console.WriteLine($"No. steps = <unknown>, No. f-s = {CallCount:N0}, No. J-s = {0}");

            var elapsed = sw.Elapsed;
            Console.WriteLine($"Elapsed: {elapsed}\n\n");
        }
    }
}
