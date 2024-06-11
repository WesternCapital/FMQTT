// fsharplint:disable TypeNames PublicValuesNames MemberNames
namespace ExceptionalCode

open System.Diagnostics

[<AutoOpen>]
module internal AtomicOperators =
    [<DebuggerHidden>]
    let (<|>) (a: 'a option) (b: 'a -> 'b) : unit =
        a
        |> Option_SomeToFNQuiet b

    let inline (^) f a = f a