namespace ExceptionalCode
//Op: Auto
open System.Diagnostics
open System.Text.RegularExpressions
//Op: End
[<AutoOpen>]

module internal AtomicX =
    type hidden = DebuggerHiddenAttribute

    let IgnoreQuietly = ignore
    [<DebuggerHidden>]
    let ns (x: string) : string = if isNull x then "" else $"{x}"

    [<DebuggerHidden>]
    let RegexReplace (pattern: string) (replacement: string) (input: string) : string =
        let input = input |> ns
        let pattern = pattern |> ns
        let replacement = replacement |> ns
        Regex.Replace(input, pattern, replacement, (RegexOptions.IgnoreCase ||| RegexOptions.Singleline))

    [<DebuggerHidden>]
    let RegexStrip (pattern: string) (input: string) = input |> RegexReplace pattern ""

    [<DebuggerHidden>]
    let TrimSpecificStart needle (text: string) =
        ns text
        |> RegexStrip $"^{needle |> Regex.Escape}"

    [<DebuggerHidden>]
    let TrimSpecificEnd needle (text: string) =
        ns text
        |> RegexStrip $"{needle |> Regex.Escape}$"

    [<DebuggerHidden>]
    let TrimSpecific needle (text: string) =
        text
        |> TrimSpecificStart needle
        |> TrimSpecificEnd needle

    let [<hidden>]Option_SomeToFN (fn: 'a -> 'b) (maybe: 'a option)      = Option.map fn maybe
    let [<hidden>]Option_SomeToFNQuiet a b                               = Option_SomeToFN a b |> ignore