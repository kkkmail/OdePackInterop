namespace OdePackInteropFSharpTest

open System
open System.Diagnostics

#nowarn "9"

open Microsoft.FSharp.NativeInterop
open Softellect.OdePackInterop
open Softellect.OdePackInterop.Sets
open Softellect.OdePackInterop.SolverDescriptors

module OdeTestRunner =

    let defaultNumberOfPairs = 50_000
    let defaultMakeNonNegative = true
    let fwdCoeff = 1.0
    let bkwCoeff = 0.1
    let startTime = 0.0
    let endTime = 1.0e06


    type SolverType =
        | Adams
        | Bdf

        member t.value =
            match t with
            | Adams -> 1
            | Bdf -> 2


    type CorrectorIteratorType =
        | Functional
        | ChordWithDiagonalJacobian

        member t.value =
            match t with
            | Functional -> 0
            | ChordWithDiagonalJacobian -> 3


    let private f (p: bool, neq : int, x : nativeptr<double>, dx : nativeptr<double>) =
        let numberOfPairs = (neq / 2) - 1
        let g i = NativePtr.get x i
        let s i v = NativePtr.set dx i v
        let y = [| for i in 0.. neq - 1 -> if p then max 0.0 (g i) else (g i) |]

        s 0 (2.0 * (-fwdCoeff * y.[0] + bkwCoeff * y.[1] * y.[2]))
        s 1 (fwdCoeff * y.[0] - bkwCoeff * y.[1] * y.[2])

        for i in 1 .. numberOfPairs do
            s (2 * i) (fwdCoeff * y.[2 * i - 2] - bkwCoeff * y.[2 * i - 1] * y.[2 * i] + 2.0 * (-fwdCoeff * y.[2 * i] + bkwCoeff * y.[2 * i + 1] * y.[2 * i + 2]))
            s (2 * i + 1) (fwdCoeff * y.[2 * i] - bkwCoeff * y.[2 * i + 1] * y.[2 * i + 2])

        s (2 * numberOfPairs + 2) (fwdCoeff * y.[2 * numberOfPairs] - bkwCoeff * y.[2 * numberOfPairs + 1] * y.[2 * numberOfPairs + 2])


    let createInterop p = Interop.F(fun n _ y dy -> f(p, n, y, dy))
    let getInitialValues n = [| for i in 0 .. n - 1 -> if i = 0 then 10.0 else 0.0 |]


    let outputResult (r : SolverResult) (e : TimeSpan) =
        let n = if r.NumberOfEquations <= 1001 then r.NumberOfEquations else 100
        printfn $"At t = {r.EndTime:N2}\n    Total = {r.X |> Array.sum}"

        let s = r.X |> Array.take n |> Array.mapi(fun i e -> $"    y[{i}] = {e}") |> String.concat "\n"
        printfn $"{s}"

        let steps = if r.Steps > 0 then $"{r.Steps:N0}" else "<unknown>"
        printfn $"No. steps = {steps}, No. f-s = {r.FuncCalls:N0}, No. J-s = {r.JacobianCalls:N0}"

        printfn $"Elapsed: {e}\n\n"
