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

        private static int NumberOfPairs { get; set; } = 50_000;
        private static int NumberOfEquations => 2 * NumberOfPairs + 1;

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
        /// kk:20210307
        /// The tests below were run for NumberOfPairs = 50,000 (number of equations = 100,001).
        /// If non-negativity is used (all negative values of y are treated as zeros when calculating
        /// the derivative) then the results are as follows:
        ///     1. MF = 23 (SolutionMethod.Bdf, CorrectorIteratorMethod.ChordWithDiagonalJacobian).
        ///        Integral of motion: 10.0 -> 10.301689191032535 or OVER 3% discrepancy.
        ///        No. steps = 40,104, No. f-s = 132,533, No. J-s = 37,380
        ///        Elapsed: 00:02:37.3532643
        ///     2. MF = 13 (SolutionMethod.Adams, CorrectorIteratorMethod.ChordWithDiagonalJacobian).
        ///        Integral of motion: 10.0 -> 10.380914193130206 or OVER 3% discrepancy.
        ///        No. steps = 39,955, No. f-s = 100,769, No. J-s = 20,207
        ///        Elapsed: 00:01:49.2829071
        ///     3. MF = 20 (SolutionMethod.Bdf, CorrectorIteratorMethod.Functional).
        ///        Integral of motion: 10.0 -> 9.999999999999996.
        ///        No. steps = 49,067, No. f-s = 89,820, No. J-s = 0
        ///        Elapsed: 00:01:42.9414014
        ///     4. MF = 10 (SolutionMethod.Adams, CorrectorIteratorMethod.Functional)
        ///        Integral of motion: 10.0 -> 9.999999999999936.
        ///        No. steps = 48,266, No. f-s = 87,707, No. J-s = 0
        ///        Elapsed: 00:01:39.7107217
        ///     5. AlgLib Cash-Carp method.
        ///        The solver did not come back.
        /// If non-negativity (replacement of yInpt by y) is turned off then the following happens:
        ///     1. MF = 23.
        ///        Integral of motion is nearly conserved: 10.0 -> 9.994361679959828
        ///        No. steps = 18,176, No. f-s = 64,101, No. J-s = 20,098
        ///        Elapsed: 00:00:46.7831109
        ///     2. MF = 13.
        ///        Integral of motion: 10.0 -> 9.98345003326132
        ///        No. steps = 185,378, No. f-s = 649,958, No. J-s = 184,053
        ///        Elapsed: 00:08:43.6774383
        ///     3. MF = 20.
        ///        The solver did not come back.
        ///     4. MF = 10.
        ///        The solver did not come back.
        ///     5. AlgLib Cash-Carp method.
        ///        The solver did not come back.
        ///
        /// neq = 2 * n + 1
        /// y[0] ⇌ y[1] + y[2]
        /// y[2] ⇌ y[3] + y[4]
        /// y[4] ⇌ y[5] + y[6]
        /// ...
        /// y[2 * n - 2] ⇌ y[2 * n - 1] + y[2 * n]
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

        static void FImpl(double[] y, double x, double[] dy, object _)
        {
            CallCount++;

            dy[0] = -fwdCoeff * y[0] + bkwCoeff * y[1] * y[2];
            dy[1] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
            dy[2] = fwdCoeff * y[0] - bkwCoeff * y[1] * y[2];
        }

        private static DateTime Start { get; set; }

        static void FImpl2(double[] yInpt, double x, double[] dy, object obj)
        {
            CallCount++;
            var neq = yInpt.Length;

            if (CallCount % 1_000_000 == 0) Console.WriteLine($"    t = {x}, CallCount = {CallCount}, run time = {DateTime.Now - Start}.");

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

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (int.TryParse(args[0], out var np))
                {
                    NumberOfPairs = np;
                }
            }

            Console.WriteLine($"Using number of pairs: {NumberOfPairs}. Specify another number via command line parameter to override.");

            var numberOfEquations = NumberOfEquations;
            Console.WriteLine(
                $"Number of equations: {numberOfEquations}, DefaultAbsoluteTolerance: {SolverParams.DefaultAbsoluteTolerance}.\n");
            RunDLSODE(numberOfEquations, SolutionMethod.Bdf, CorrectorIteratorMethod.ChordWithDiagonalJacobian);
            RunDLSODE(numberOfEquations, SolutionMethod.Adams, CorrectorIteratorMethod.ChordWithDiagonalJacobian);
            RunDLSODE(numberOfEquations, SolutionMethod.Bdf, CorrectorIteratorMethod.Functional);
            RunDLSODE(numberOfEquations, SolutionMethod.Adams, CorrectorIteratorMethod.Functional);
            RunAlgLib(numberOfEquations);
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

            SolverResult solverResult;

            var sw = new Stopwatch();
            sw.Start();

            unsafe
            {
                solverResult = OdeSolver.Run(solverParam, FImpl2);
            }

            var elapsed = sw.Elapsed;
            OutputResults(solverResult, elapsed);
        }

        private static void OutputResults(SolverResult solverResult, TimeSpan elapsed)
        {
            var n = solverResult.NumberOfEquations <= 1001 ? solverResult.NumberOfEquations : 100;
            Console.WriteLine($"At t = {solverResult.EndTime:N2}\n    Total = {solverResult.X.Sum()}");
            Console.WriteLine($"{string.Join("\n", solverResult.X.Take(n).Select((e, i) => $"    y[{i}] = {e}"))}");

            Console.WriteLine(
                $"No. steps = {(solverResult.Steps > 0 ? $"{solverResult.Steps:N0}" : "<unknown>")}, " +
                $"No. f-s = {solverResult.FuncCalls:N0}, No. J-s = {solverResult.JacobianCalls:N0}");

            Console.WriteLine($"Elapsed: {elapsed}\n\n");
        }

        static void RunAlgLib(int numberOfEquations)
        {
            if (numberOfEquations > 51)
            {
                Console.WriteLine($"NOT calling OdeSolverSolve because the number of equations is too large: {numberOfEquations}.");
                return;
            }

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

            var elapsed = sw.Elapsed;

            var solverResult = new SolverResult
            {
                ResultState = ResultState.Success,
                StartTime = StartTime,
                EndTime = xTbl[1],
                X = Enumerable.Range(0, numberOfEquations).Select(i => yTbl[1, i]).ToArray(),
                Steps = -1,
                FuncCalls = CallCount,
                JacobianCalls = 0,
                RequiredLRW = 0,
                RequiredLIW = 0,
            };

            OutputResults(solverResult, elapsed);
        }
    }
}
