open System

open OdePackInteropFSharpTest.OdeTestRunner

[<EntryPoint>]
let main argv =
    let numberOfPairs =
        match argv.Length with
        | 1 ->
            match Int32.TryParse argv.[0] with
            | true, n -> n
            | false, _ -> defaultNumberOfPairs
        | _ -> defaultNumberOfPairs

    0
