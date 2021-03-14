open System

open Softellect.OdePackInterop
open Softellect.OdePackInterop.Sets
open Softellect.OdePackInterop.SolverDescriptors
open OdePackInteropFSharpTest.OdeTestRunner

[<EntryPoint>]
let main argv =

    let usage i =
        let s =
            "Allowed command line parameters are:\n" +
            $"    Zero command line parameters - default values of the number of pairs: {defaultNumberOfPairs} " +
            $"and make non-negative: {defaultMakeNonNegative} will be used.\n" +
            $"    One command line parameter - specify an integer value for the number of pairs.\n" +
            $"    Two command line parameters - specify an integer value for the number of pairs " +
            $"and an integer: 0 (false) or 1 (true) for make non-negative parameter."

        if i then printfn "Invalid command line parameters.\n"
        printfn $"{s}"


    let n() =
        match Int32.TryParse argv.[0] with
        | true, n -> n
        | false, _ ->
            usage true
            defaultNumberOfPairs

    let p() =
        match Int32.TryParse argv.[1] with
        | true, n when n = 0  -> false
        | true, n when n = 1  -> true
        | _ ->
            usage true
            true

    let np =
        match argv.Length with
        | 0 ->
            usage false
            Some (defaultNumberOfPairs, defaultMakeNonNegative)
        | 1 -> Some (n(), defaultMakeNonNegative)
        | 2 -> Some (n(), p())
        | _ ->
            usage true
            None

    match np with
    | Some (n, p) ->
        printfn $"Using number of pairs: {n}, make non-negative: {p}."
        let f() = createInterop p
        let neq = 2 * n + 1

        // !!! The solver must be run completely in C# !!!
        // If run in F#, then it results in: System.InvalidProgramException: Common Language Runtime detected an invalid program.
        // This is probably due to the fact that it is an unsafe code and in C# the whole execution block is wrapped into unsafe {}.
        // However, there is no unsafe in F# and this is what could be causing the exception above.
        // Method OdeSolver.RunFSharp was written explicitly for F# because of this issue.
        // Using F# fixed can't help here because there is nothing to "fix".
        OdeSolver.RunFSharp((fun() -> f()), SolverType.Bdf.value, CorrectorIteratorType.ChordWithDiagonalJacobian.value, startTime, endTime, getInitialValues neq, (fun r e -> outputResult r e))
        OdeSolver.RunFSharp((fun() -> f()), SolverType.Adams.value, CorrectorIteratorType.ChordWithDiagonalJacobian.value, startTime, endTime, getInitialValues neq, (fun r e -> outputResult r e))
        OdeSolver.RunFSharp((fun() -> f()), SolverType.Bdf.value, CorrectorIteratorType.Functional.value, startTime, endTime, getInitialValues neq, (fun r e -> outputResult r e))
        OdeSolver.RunFSharp((fun() -> f()), SolverType.Adams.value, CorrectorIteratorType.Functional.value, startTime, endTime, getInitialValues neq, (fun r e -> outputResult r e))
    | None -> ()

    0
