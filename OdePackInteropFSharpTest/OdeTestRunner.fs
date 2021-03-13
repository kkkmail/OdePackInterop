namespace OdePackInteropFSharpTest

#nowarn "9"

open Microsoft.FSharp.NativeInterop
open Softellect.OdePackInterop

module OdeTestRunner =

    let defaultNumberOfPairs = 50_000
    let fwdCoeff = 1.0
    let bkwCoeff = 0.1

    let makeNonNegativeByRef (neq : int) (x : nativeptr<double>) : double[] =
        [| for i in 0.. neq - 1 -> max 0.0 (NativePtr.get x i) |]


    let private f (n : byref<int>, x : nativeptr<double>, dx : nativeptr<double>) =
        let neq = n
        let numberOfPairs = (neq / 2) - 1

        let y = [| for i in 0.. neq - 1 -> max 0.0 (NativePtr.get x i) |]
        let s i v = NativePtr.set dx i v

        s 0 (2.0 * (-fwdCoeff * y.[0] + bkwCoeff * y.[1] * y.[2]))
        s 1 (fwdCoeff * y.[0] - bkwCoeff * y.[1] * y.[2])

        for i in 0 .. numberOfPairs do
            s (2 * i) (fwdCoeff * y.[2 * i - 2] - bkwCoeff * y.[2 * i - 1] * y.[2 * i] + 2.0 * (-fwdCoeff * y.[2 * i] + bkwCoeff * y.[2 * i + 1] * y.[2 * i + 2]))
            s (2 * i + 1) (fwdCoeff * y.[2 * i] - bkwCoeff * y.[2 * i + 1] * y.[2 * i + 2])

        s (2 * numberOfPairs + 2) (fwdCoeff * y.[2 * numberOfPairs] - bkwCoeff * y.[2 * numberOfPairs + 1] * y.[2 * numberOfPairs + 2])


    let createInterop() = Interop.F(fun n _ y dy -> f(&n, y, dy))

