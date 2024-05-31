// fsharplint:disable TypeNames PublicValuesNames MemberNames
namespace ExceptionalCode

open System.Diagnostics

[<AutoOpen>]
#if EXCEPTIONALCODE
module AtomicOperators =
#else
module internal AtomicOperators =
#endif
    [<DebuggerHidden>]
    let (<|>) (a: 'a option) (b: 'a -> 'b) : unit =
        a
        |> Option_SomeToFNQuiet b

    let inline (^) f a = f a