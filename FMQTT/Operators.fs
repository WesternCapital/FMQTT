// THIS IS A SECONDARY COPY OF THE OPERATORS FROM SHAREDFSHARPVANILLA
namespace ExceptionalCode
//Op: Auto
open System
open System.Collections.Generic
open System.Diagnostics
open System.Text.RegularExpressions
//Op: End
[<AutoOpen>]
module internal Operators =
    type Hide0Attribute = DebuggerHiddenAttribute
    type Hide1Attribute = DebuggerHiddenAttribute
    type Hide2Attribute = DebuggerHiddenAttribute
    type Hide3Attribute = DebuggerHiddenAttribute
    [<DebuggerHidden>]
    let Tee fnct obj =
        fnct obj
        obj
    type MyDebuggerHidden() =
        inherit Attribute()
    let private composeWithTwoParameters (fnLeft: 'arg1 -> 'arg2 -> 'middleType) (fnRight: 'middleType -> 'c) (arg1: 'arg1) (arg2: 'arg2) =
        fnLeft arg1 arg2 |> fnRight
    let (>>>) a b = composeWithTwoParameters a b
    let private composeWithTwoParametersSwap (fnLeft: 'arg1 -> 'arg2 -> 'middleType) (fnRight: 'middleType -> 'c) (arg2: 'arg2)
        (arg1: 'arg1) = fnLeft arg1 arg2 |> fnRight
    let (><>) a b = composeWithTwoParametersSwap a b
    let private composeWithThreeParameters (fnLeft: 'arg1 -> 'arg2 -> 'arg3 -> 'middleType) (fnRight: 'middleType -> 'c) (arg1: 'arg1)
        (arg2: 'arg2) (arg3: 'arg3) = fnLeft arg1 arg2 arg3 |> fnRight
    let (>>>>) a b = composeWithThreeParameters a b

    let (<||||) fn args =
        let (a, b, c, d) = args
        fn a b c d

    [<DebuggerHidden>]
    let inline ToList (x: 'a seq) =
        if isNull x then List.empty<'a>
        else Seq.toList x

    [<DebuggerHidden>]
    let ToStack (items: 'a seq) = new Collections.Generic.Stack<'a>(Seq.rev items)

    [<DebuggerHidden>]
    let ToObj x = x :> obj

    [<DebuggerHidden>]
    let ToArray = Seq.toArray

    [<DebuggerHidden>]
    let ToSeq x = List.toSeq x

    [<DebuggerHidden>]
    let ToArrayList x = System.Collections.ArrayList(x |> ToArray)

    [<DebuggerHidden>]
    let Concat = String.concat

    [<DebuggerHidden>]
    let TeeIter obj fnct =
        obj |> Seq.iter fnct
        obj

    let (+/+) x y = System.IO.Path.Combine(x, y |> ExceptionalCode.AtomicX.TrimSpecific "")
    let (@@) (item: 'a) (items: 'a seq) = [ item ] @ (items |> ToList)
    let (@>>) (item: 'a) (items: 'a seq) = [ item ] @ (items |> ToList)
    let (<<@) (items: 'a seq) (item: 'a) = (items |> ToList) @ [ item ]
    let (@@>>) (itemsStart: 'a seq) (itemsToAppend: 'a seq) = itemsToAppend |> Seq.append itemsStart
    let (<<@@) = (@@>>)
    let (&->) (item: 'a) (items: 'a seq) = (items |> ToList) @ [ item ]
    let (<-&) (items: 'a seq) (item: 'a) = [ item ] @ (items |> ToList)
    let (<&) = (<<@)
    let (&>) = (@>>)

    [<DebuggerHidden>]
    let (|?~) (items: string seq) (regex: string) =
        let r = Regex(regex, RegexOptions.IgnoreCase)
        items
        |> Seq.filter (fun x -> r.IsMatch(x))

    [<DebuggerHidden>]
    let composeWhere (itemsFn: 'a -> #seq<'b>) (pred: 'b -> bool): 'a -> 'b seq =
        itemsFn
        >> Seq.where pred

    [<DebuggerHidden>]
    let composeWhereList (itemsFn: 'a -> 'b list) (pred: 'b -> bool): 'a -> 'b list =
        itemsFn
        >> List.where pred

    //Oxp: IndentOnRegex (?<=.*?let.*?\(.*?\)
    //Op: CollapseWhitespaceKeepIndent
    //Op: IndentOnWrapSpaces =
    //Op: Sort
    let [<Hide0>](|?) (items: #seq<'a>) predicate                   = items |> Seq.filter predicate
    let [<Hide1>](|+) a b                                           = Seq.map b a
    let [<Hide1>](|+?) a b                                          = Seq.choose b a |> ToList
    let [<Hide1>](|+~) a b                                          = Seq.collect b a
    let [<Hide1>](|+~|) a b                                         = Seq.collect b a |> ToList
    let [<Hide1>](|.?) (items: ('a * 'b) seq) (test: 'b -> bool)    = items |? (fun (_, prop) -> test prop) |+ fst
    let [<Hide2>](|.?!) (items: ('a * 'b) seq) (test: 'b -> bool)   = items |.? (test >> not)
    let [<Hide2>](|?|) items predicate                              = items |? predicate |> Seq.toList
    let [<Hide3>](.*/) (input: string) (pat: string)                = Regex.Split(input, pat, RegexOptions.IgnoreCase)
    let [<Hide3>](.*=) (input: string) (pat: string)                = Regex.Matches(input, pat, RegexOptions.IgnoreCase)
    let [<Hide3>](.*>) (input: string) (pat: string) (rep: string)  = Regex.Replace(input, pat, rep, RegexOptions.IgnoreCase)
    let [<Hide3>](.*?) (input: string) (pat: string)                = Regex.IsMatch(input, pat, RegexOptions.IgnoreCase)
    let [<Hide3>](>>%) leftFn rightFn                               = leftFn >> Seq.iter rightFn
    let [<Hide3>](>>+) leftFn rightFn                               = leftFn >> Seq.map rightFn
    let [<Hide3>](>>+-) leftFn rightFn                              = leftFn >>+ rightFn >> Seq.concat
    let [<Hide3>](>>-) leftFn rightFn                               = leftFn >> rightFn >> Seq.concat
    let [<Hide3>](>>?!) leftFn rightFn                              = leftFn >> (Seq.where (rightFn >> not))
    let [<Hide3>](>>?) (itemsFn: 'a -> 'b seq) (pred: 'b -> bool)   = composeWhere itemsFn pred
    let [<Hide3>](>>|?) (itemsFn: 'a -> 'b list) (pred: 'b -> bool) = composeWhereList itemsFn pred
    let [<Hide3>](|%%) a b                                          = Seq.iteri b a
    let [<Hide3>](|%) a b                                           = Seq.iter b a
    let [<Hide3>](|++) a b                                          = Seq.mapi b a
    let [<Hide3>](|++|) a b                                         = Seq.mapi b a |> ToList
    let [<Hide3>](|+|) a b                                          = Seq.map b a |> ToList
    let [<Hide3>](|+||) a b                                         = Seq.map b a |> ToArray
    let [<Hide3>](|-+) items1 items2                                = items1 |> Seq.append items2
    let [<Hide3>](|-++) a b                                         = a |> Seq.concat |> b
    let [<Hide3>](|--%)                                             = TeeIter
    let [<Hide3>](|--|) (obj: 'a) (fnct: 'a -> unit)                = Tee fnct obj |> ignore
    let [<Hide3>](|--||) (obj: 'a) (fnct: unit -> unit)             = Tee (ignore >> fnct) obj
    let [<Hide3>](|.) items fn                                      = items |+ (fun x -> (x, fn x))
    let [<Hide3>](|>|) a b                                          = a |> ToList |> b
    let [<Hide3>](|>||) a b                                         = a |> Seq.toArray |> b
    let [<Hide3>](|?!) items predicate                              = items |? (not << predicate)
    let [<Hide3>](|?!=) items target                                = items |? (fun x -> x <> target)
    let [<Hide3>](|?!|) items predicate                             = items |?! predicate |> Seq.toList
    let [<Hide3>](|?!||) items predicate                            = items |?! predicate |> Seq.toArray
    let [<Hide3>](|?=) items target                                 = items |? ((=) target)
    let [<Hide3>](|~/) a b                                          = Regex.Split(a, b, RegexOptions.IgnoreCase)
    //Op: End

    //Oxp: IndentOnRegex (?<=.*?let.*?\(.*?\)
    //Op: CollapseWhitespaceKeepIndent
    //Op: IndentOnWrapSpaces =
    //Op: Sort
    let [<Hide3>](|&) obj fnct                                                                                     = Tee fnct obj
    let [<Obsolete("Please update this operator to |& to avoid order of operations bugs")>][<Hide3>](|--) obj fnct = Tee fnct obj
    //Op: End

    [<DebuggerHidden>]
    let (>>|+) a b =
        fun z ->
            z
            |> a
            |+ b

    [<DebuggerHidden>]
    let (>>|+|) a b = a >>|+ b >> ToList

    [<DebuggerHidden>]
    let (>>|+||) a b = a >>|+ b >> ToArray

    [<DebuggerHidden>]
    let (>>|%) a b =
        fun z ->
            z
            |> a
            |% b

[<AutoOpen>]
module internal ResultOperators =
    let lift (fn: 'data -> 'error) (res: Result<'data,'c>) = res |> Result.bind (fn >> Ok)

    let (|>>%) leftFn rightFn = leftFn |> lift (Seq.iter rightFn)
    let (|>>+) leftFn rightFn = leftFn |> lift (Seq.map rightFn)

    let (|>>%|) leftFn rightFn = leftFn |> lift (Seq.iter rightFn) |> ignore
    let (|>>+|) leftFn rightFn = leftFn |> lift (Seq.map rightFn) |> ignore


    /// <summary>
    /// MS Result Bind
    /// </summary>
    let inline (>>=) res fn = res |> Result.bind fn

    /// <summary>
    /// MS Result Bind Error
    /// </summary>
    let inline (>>!) res fn = res |> Result.mapError fn

    /// <summary>
    /// MS Result Lift
    /// </summary>
    [<DebuggerHidden>]
    let inline (>>/) res fn = res |> lift fn