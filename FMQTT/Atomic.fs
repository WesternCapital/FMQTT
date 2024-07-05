// fsharplint:disable TypeNames PublicValuesNames MemberNames
namespace ExceptionalCode
//Op: Auto
open Microsoft.FSharp.Quotations
open System
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Text.RegularExpressions
open System.Threading

open ExceptionalCode
//Op: End

[<AutoOpen>]
module internal Atomic =
    let eq = (=)
    //Op: CorrectWhitespace
    //Op: IndentOn =
    let sleep (ms: int)  = Thread.Sleep ms
    type FileAttribute   = CallerFilePathAttribute
    type LineAttribute   = CallerLineNumberAttribute
    type MemberAttribute = CallerMemberNameAttribute
    type FI              = FileInfo
    type DI              = DirectoryInfo
    //Op: End
    let retryWithMessages count =
        let mutable x = 0
        fun message ->
            x <- x + 1
            if x > count then
                failwith message

    let retry count =
        let mutable x = 0
        fun () ->
            x <- x + 1
            if x > count then
                failwith "Too many attempts"

    type Is =
        static member GreaterThan a b = b > a

    type Prop() =
        //Op: PropsAx Ticks Url ProjectFile PID FullName ADUsername UserProfileId FieldName UserProfileID Command company_id Adds ConnectionString Date descr FileNameWithoutExtension FirstName FullPath id Id Key LastName Length manager_id market_id Message Name name path PathName PathX product_descr Project store_id Text UserRoleID Value Values week_starting X XElement Y Z
        //Op: CollapseWhitespaceKeepIndent
        //Op: IndentOn (
        //Op: IndentOn :
        //Op: IndentOn )
        //Op: Sort
        static member inline Adds                     (o: ^a) = (^a : (member Adds                     : _ ) o)
        static member inline ADUsername               (o: ^a) = (^a : (member ADUsername               : _ ) o)
        static member inline Command                  (o: ^a) = (^a : (member Command                  : _ ) o)
        static member inline company_id               (o: ^a) = (^a : (member company_id               : _ ) o)
        static member inline ConnectionString         (o: ^a) = (^a : (member ConnectionString         : _ ) o)
        static member inline Date                     (o: ^a) = (^a : (member Date                     : _ ) o)
        static member inline descr                    (o: ^a) = (^a : (member descr                    : _ ) o)
        static member inline FieldName                (o: ^a) = (^a : (member FieldName                : _ ) o)
        static member inline FileNameWithoutExtension (o: ^a) = (^a : (member FileNameWithoutExtension : _ ) o)
        static member inline FirstName                (o: ^a) = (^a : (member FirstName                : _ ) o)
        static member inline FullName                 (o: ^a) = (^a : (member FullName                 : _ ) o)
        static member inline FullPath                 (o: ^a) = (^a : (member FullPath                 : _ ) o)
        static member inline id                       (o: ^a) = (^a : (member id                       : _ ) o)
        static member inline Id                       (o: ^a) = (^a : (member Id                       : _ ) o)
        static member inline Key                      (o: ^a) = (^a : (member Key                      : _ ) o)
        static member inline LastName                 (o: ^a) = (^a : (member LastName                 : _ ) o)
        static member inline Length                   (o: ^a) = (^a : (member Length                   : _ ) o)
        static member inline manager_id               (o: ^a) = (^a : (member manager_id               : _ ) o)
        static member inline market_id                (o: ^a) = (^a : (member market_id                : _ ) o)
        static member inline Message                  (o: ^a) = (^a : (member Message                  : _ ) o)
        static member inline Name                     (o: ^a) = (^a : (member Name                     : _ ) o)
        static member inline name                     (o: ^a) = (^a : (member name                     : _ ) o)
        static member inline path                     (o: ^a) = (^a : (member path                     : _ ) o)
        static member inline PathName                 (o: ^a) = (^a : (member PathName                 : _ ) o)
        static member inline PathX                    (o: ^a) = (^a : (member PathX                    : _ ) o)
        static member inline PID                      (o: ^a) = (^a : (member PID                      : _ ) o)
        static member inline product_descr            (o: ^a) = (^a : (member product_descr            : _ ) o)
        static member inline Project                  (o: ^a) = (^a : (member Project                  : _ ) o)
        static member inline ProjectFile              (o: ^a) = (^a : (member ProjectFile              : _ ) o)
        static member inline store_id                 (o: ^a) = (^a : (member store_id                 : _ ) o)
        static member inline Text                     (o: ^a) = (^a : (member Text                     : _ ) o)
        static member inline Ticks                    (o: ^a) = (^a : (member Ticks                    : _ ) o)
        static member inline Url                      (o: ^a) = (^a : (member Url                      : _ ) o)
        static member inline UserProfileId            (o: ^a) = (^a : (member UserProfileId            : _ ) o)
        static member inline UserProfileID            (o: ^a) = (^a : (member UserProfileID            : _ ) o)
        static member inline UserRoleID               (o: ^a) = (^a : (member UserRoleID               : _ ) o)
        static member inline Value                    (o: ^a) = (^a : (member Value                    : _ ) o)
        static member inline Values                   (o: ^a) = (^a : (member Values                   : _ ) o)
        static member inline week_starting            (o: ^a) = (^a : (member week_starting            : _ ) o)
        static member inline X                        (o: ^a) = (^a : (member X                        : _ ) o)
        static member inline XElement                 (o: ^a) = (^a : (member XElement                 : _ ) o)
        static member inline Y                        (o: ^a) = (^a : (member Y                        : _ ) o)
        static member inline Z                        (o: ^a) = (^a : (member Z                        : _ ) o)
        //Op: End

        //Op: CollapseWhitespaceKeepIndent
        static member inline Execute (o : ^a) = (^a : (member Execute : (_ -> _ seq)) o)
        //Op: End

        //Op: CollapseWhitespaceKeepIndent
        //Op: IndentOn (
        //Op: IndentOn :
        //Op: IndentOn )
        //Op: Sort
        static member inline Stringify     (o: ^a) = (^a : (member Stringify     : unit -> string ) o)
        static member inline ToTraceString (o: ^a) = (^a : (member ToTraceString : unit -> string ) o)
        //Op: End

    let TryRepeatedly (attempts: int) (sleepMS: int) fn =
        let rec _fn attempt =
            try
                fn() |> Ok
            with ex ->
                if attempt < attempts then
                    System.Threading.Thread.Sleep sleepMS
                    if attempt > 2 then
                        $"Attempt #%i{attempt}: %s{ex.Message}"
                        |> System.Console.WriteLine
                    _fn <| attempt + 1
                else
                    ex |> Error
        _fn 0

    let Flatten<'a>(x: 'a option option) =
        match x with
        | Some x -> x
        | _ -> None

    let timed handleTiming fnToRun =
        let start = DateTime.Now
        let result = fnToRun()
        DateTime.Now - start |> handleTiming
        result

    let timedX fnToRun =
        let start = DateTime.Now
        let data = fnToRun()
        data, int (DateTime.Now - start).TotalMilliseconds

    [<DebuggerHidden>]
    let nullDef dv fn arg =
            if isNull arg then dv
            else fn arg

    let Option_Bind (fn: 'a -> 'b option) (maybe: 'a option) : 'b option =
        maybe
        |> function
        | Some x -> fn x |> Some
        | None -> None
        |> Flatten

    let Nullable_SomeToFN (fn: 'a -> 'b) (maybe: Nullable<'a>) =
        if maybe.HasValue then maybe.Value |> fn |> Some
        else None

    let private cache = System.Collections.Concurrent.ConcurrentDictionary<string, obj>()

    type Existence =
        | Exists
        | NotExists

    let EmptyTo dv (x: string) =
        if String.IsNullOrEmpty x then dv
        else x

    let FullName = nullDef "" (fun (f: FileInfo) -> f.FullName)
    [<DebuggerHidden>]
    let NoneToX (maybe: 'a option) (defaultValue: 'a): 'a =
        match maybe with
        | Some x -> x
        | None -> defaultValue
    type hidden = DebuggerHiddenAttribute
    //Op: CorrectWhitespace
    //Op: IndentOn =
    //Op: Sort
    let [<hidden>]flipParams2 (fn: 'a -> 'b -> 'c)       = (fun (b: 'b) (a: 'a) -> fn a b)
    let [<hidden>]IsNullOrEmpty x                        = String.IsNullOrEmpty x
    let [<hidden>]makeFunc (fn: 'a -> 'b) : Func<'a, 'b> = new System.Func<'a, 'b>(fn)
    //Op: End

    //Op: CorrectWhitespace
    //Op: IndentOn =
    //Op: Sort
    let [<hidden>](|AbsDec|) (x: decimal)                                = Math.Abs x
    let [<hidden>](|AbsInt|) (x: int)                                    = Math.Abs x
    let [<hidden>](|KVP|) (k: KeyValuePair<'a, 'b>)                      = (k.Key, k.Value)
    let [<hidden>](|NS|) (x: string) : string                            = x |> ns
    let [<hidden>]Append (stringToAppend: string) something              = something + stringToAppend
    let [<hidden>]AppendNewLine                                          = Append "\r\n"
    let [<hidden>]CaseInsentiveCompare string1 string2                   = String.Compare(ns string1, ns string2, StringComparison.OrdinalIgnoreCase) |> eq 0
    let [<hidden>]Curry2 fn a b                                          = fn(a, b)
    let [<hidden>]Curry3 fn a b c                                        = fn(a, b, c)
    let [<hidden>]Curry4 fn a b c d                                      = fn(a, b, c, d)
    let [<hidden>]EmptyFrameworkDictionary()                             = Dictionary<_, _>()
    let [<hidden>]emptyStringFn()                                        = ""
    let [<hidden>]EmptyToZero                                            = EmptyTo "0"
    let [<hidden>]flip                                                   = flipParams2
    let [<hidden>]foldx a b c                                            = Seq.fold b a c
    let [<hidden>]fst3 (a, _, _)                                         = a
    let [<hidden>]getType x                                              = x.GetType()
    let [<hidden>]getTypeName x                                          = (x |> getType).Name
    let [<hidden>]groupBy selector items                                 = System.Linq.Enumerable.GroupBy(items, makeFunc selector)
    let [<hidden>]ignoreList (_: 'a list)                                = ()
    let [<hidden>]inline ToList x                                        = Seq.toList x
    let [<hidden>]invoke fn                                              = fn ()
    let [<hidden>]IsEmpty                                                = IsNullOrEmpty
    let [<hidden>]IsNotEmpty                                             = IsNullOrEmpty >> not
    let [<hidden>]isNotNull x                                            = x |> (isNull >> not)
    let [<hidden>]Join (delimiter: string) (items: #seq<string>)         = String.Join(delimiter, items)
    let [<hidden>]JoinCommaAndSpace (items: #seq<string>)                = Join ", " items
    let [<hidden>]JoinLines(text: #seq<string>): string                  = String.Join("\r\n", text |> Seq.toArray)
    let [<hidden>]JoinPeriodSpace txt                                    = Join ". " txt
    let [<hidden>]Label label value                                      = sprintf "%s: %s" label value
    let [<hidden>]LabelInt label value                                   = $"%s{ label }: %i{ value }"
    let [<hidden>]LabelObj label value                                   = sprintf "%s: %s" ($"%A{ label }".Trim(char @"""")) ( $"%A{ value }".Trim(char @""""))
    let [<hidden>]LabelTuple (label, value)                              = Label label value
    let [<hidden>]len x                                                  = Seq.length x
    let [<hidden>]Length x                                               = Seq.length x
    let [<hidden>]Lines (text: string)                                   = Regex.Split(text |> ns, @"\r\n|\n\r|\n|\r")
    let [<hidden>]lower x                                                = (x |> ns).ToLower()
    let [<hidden>]makeAction (fn: unit -> unit) : Action                 = Action(fn)
    let [<hidden>]makeAction1 (fn: 'a -> unit) : Action<'a>              = new System.Action<'a>(fn)
    let [<hidden>]makeFuncU (fn: unit -> 'a) : Func<'a>                  = new Func<'a>(fn)
    let [<hidden>]MapGrid (fn: 'item -> 'result) (rows: 'item list list) = rows |+| List.map fn
    let [<hidden>]NoneTo (defaultValue: 'a) maybe : 'a                   = NoneToX maybe defaultValue
    let [<hidden>]now ()                                                 = DateTime.Now
    let [<hidden>]NowMinusDT (offset: DateTime)                          = DateTime.Now.Subtract offset
    let [<hidden>]NowMinusTS (offset: TimeSpan)                          = DateTime.Now.Subtract offset
    let [<hidden>]OPns                                                   = Option.defaultValue ""
    let [<hidden>]Option_SomeToFN fn maybe                               = AtomicX.Option_SomeToFN fn maybe
    let [<hidden>]range start endx                                       = [start .. endx]
    let [<hidden>]ret item ()                                            = item
    let [<hidden>]retFn (fn: 'a -> 'b) : 'a -> unit -> 'b                = fun x () -> fn x
    let [<hidden>]retInstead x _                                         = x
    let [<hidden>]returner item ()                                       = item
    let [<hidden>]retX () item                                           = item
    let [<hidden>]Split (pattern: char) (str1: string): string list      = str1 |> ns |> fun x -> x.Split pattern |> ToList
    let [<hidden>]str x                                                  = $"{ x }"
    let [<hidden>]Substring start take (NS txt)                          = txt.Substring(start, take)
    let [<hidden>]swap                                                   = flipParams2
    let [<hidden>]today ()                                               = DateTime.Today
    let [<hidden>]ToLower(NS str)                                        = str.ToLower()
    let [<hidden>]ToString(obj: obj)                                     = if isNull obj then "" else obj.ToString()
    let [<hidden>]TotalMilliseconds (t: TimeSpan)                        = t.TotalMilliseconds |> decimal
    let [<hidden>]ToUpper(NS str)                                        = str.ToUpper()
    let [<hidden>]Trim (str: string)                                     = str |> ns |> fun x -> x.Trim()
    let [<hidden>]TrimEnd                                                = ns >> fun x -> x.TrimEnd()
    let [<hidden>]upper str                                              = ToUpper str
    let singleton                                                        = List.singleton
    type SpecificTypeHandler                                             = (exn -> string) -> exn -> string option
 //let [<hidden>]str x                                                   = $"%A{ x }"
    //Op: End
    let (|AsString|) (obj: 'a) = str obj
    type SP =
        static member Left num (NS str) =
            str.Substring
                (0,
                 (if str.Length < num then str.Length
                  else num))

    let Replace (needle: string) (replacement: string) (haystack: string) =
        if needle.Length > 0 then
            haystack.Replace(needle, replacement)
        else
            haystack

    let memoizeNamed (paramParts: string list) paramVal (fn: 'b -> 'a) =
        let joinedKeyName = paramParts |> JoinCommaAndSpace
        match cache.TryGetValue joinedKeyName with
        | true, v -> v :?> 'a
        | false, _ ->
            let v = fn paramVal
            cache.TryAdd(joinedKeyName, unbox v) |> ignore
            v

    let foldi fold first source =
        source
        |> Seq.fold (fun (prev, i) c -> (fold i prev c, i + 1)) (first, 0)
        |> fst

    let HomeFolder =
        lazy
        let z = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)
        let z = ns(z)
        z.TrimEnd('\\')
        + @"\"

    let CleanUpFilePath x =
        x
        |> RegexReplace "/" @"\"
        |> RegexReplace @"^~\\" HomeFolder.Value

    let SingleOrNone x =
        x
        |> ToList
        |> function
        | [x] -> Some x
        | _ -> None

    let FirstOrNone(items: 'a seq): 'a option =
        items
        |> Seq.toList
        |> function
        | [] -> None
        | firstEl :: _ -> Some firstEl

    let FirstOrDefault (defaultValue: 'a) (items: 'a seq): 'a =
        items
        |> FirstOrNone
        |> (NoneTo defaultValue)

    let SomeIf predicate value =
        if predicate then Some(value)
        else None

    let EmptyToDefault (defaultValue: string) (source: string) =
        if source |> IsNullOrEmpty then defaultValue
        else source

    [<hidden>]
    let UnwrapSome =
        function
        | Some (Some x) -> Some x
        | _ -> None

    [<hidden>]
    let UnwrapSomes<'a>(somes: seq<'a option>) =
        somes
        |> Seq.where (fun oo -> oo.IsSome)
        |> Seq.map (fun oo -> oo.Value)
        |> Seq.toList

    let TrimStart(startText: string) : string -> string = RegexStrip("^" + Regex.Escape(startText |> ns))

    let TrimEndSpecific (endText: string) txt = RegexStrip (Regex.Escape(endText |> ns) + "$") txt

    type RegexBuilderElement =
        | Input of string
        | Pattern of string
        | Replacement of string
        | RegexOptions of RegexOptions

    type RegexBuilder =
        {
            Input: string option
            Pattern: string option
            Replacement: string option
            ReplacementCount: int option
            RegexOptions: RegexOptions
            MatchEvaluator: MatchEvaluator option
        }
        static member Empty =
            {
                Input = None
                Replacement = None
                Pattern = None
                ReplacementCount = None
                RegexOptions = RegexOptions.IgnoreCase
                MatchEvaluator = None
            }

    type RegexPatternAttribute() =
        inherit System.Attribute()

    type RX =
        //Op: CollapseWhitespaceKeepIndent
        //Op: IndentOn rb
        //Op: IndentOn }
        static member MultiLine       rb = { rb with RegexOptions = rb.RegexOptions ||| RegexOptions.Multiline       }
        static member Singleline      rb = { rb with RegexOptions = rb.RegexOptions ||| RegexOptions.Singleline      }
        static member ExplicitCapture rb = { rb with RegexOptions = rb.RegexOptions ||| RegexOptions.ExplicitCapture }
        //Op: End

        static member Init (elements: RegexBuilderElement list) =
            let foldRx state item =
                match item with
                | Input x -> state |> RX.Input x
                | Pattern x -> state |> RX.Pattern x
                | Replacement x -> state |> RX.Replacement x
                | RegexOptions x -> state |> RX.Options x
            elements
            |> ToList
            |> List.fold foldRx RegexBuilder.Empty

        static member private InvalidArgs() = failwith "Invalid arguments for RegexBuilder"

        [<hidden>]
        static member Empty = RegexBuilder.Empty

        [<hidden>]
        static member Input text rb = { rb with Input = Some text }
        static member RegexPad (rb: RegexBuilder) = RX.RegexPadFull rb |> ignore

        static member RegexPadFull (rb: RegexBuilder) : Process option =
            #if DEBUG
            let ns =
                function
                | Some x -> x
                | _ -> ""
            //Op: CollapseWhitespaceKeepIndent
            //Op: IndentOn (
            //Op: IndentOn ,
            //Op: IndentOn )
            File.WriteAllText(@"c:\temp\regexpad.input.txt"                          , rb.Input |> ns               )
            File.WriteAllText(@"c:\temp\regexpad.pattern.txt"                        , rb.Pattern |> ns             )
            File.WriteAllText(@"c:\temp\regexpad.replacement.txt"                    , rb.Replacement |> ns         )
            File.WriteAllText(@"c:\temp\regexpad.options.txt"                        , rb.RegexOptions |> int |> str)
            Process.Start    (@"C:\DEV\Releases\RegexPadCurrent\Current\RegexPad.exe", "import"                     )
            //Op: End
            |> Some
            #else
            None
            #endif

        static member RegexPadWait (rb: RegexBuilder) : unit =
            (RX.RegexPadFull rb)
            |> function
            | Some x -> x.WaitForExit()
            | _ -> ()

        [<hidden>]
        static member Pattern ([<RegexPattern>]regexPattern) (rb: RegexBuilder) = { rb with Pattern = Some regexPattern }

        [<hidden>]
        static member PatternEscaped ([<RegexPattern>]regexPattern) (rb: RegexBuilder) = regexPattern |> Regex.Escape |> swap RX.Pattern rb

        [<hidden>]
        static member StartWithPattern ([<RegexPattern>]regexPattern) = RX.Pattern regexPattern RX.Empty

        [<hidden>]
        static member StartWithInput x = RX.Input x RX.Empty

        [<hidden>]
        static member FromInput = RX.StartWithInput

        [<hidden>]
        static member FromPattern ([<RegexPattern>]regexPattern) = RX.StartWithPattern regexPattern

        [<hidden>]
        static member Replacement text rb = { rb with Replacement = Some text }

        [<hidden>]
        static member ReplacementCount text rb = { rb with ReplacementCount = Some text }

        [<hidden>]
        static member ReplacementFn (rep: Match -> string) rb =
            { rb with MatchEvaluator = MatchEvaluator(rep) |> Some }

        static member Options opts rb = { rb with RegexOptions = opts }

        [<hidden>]
        static member IsMatch =
            function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } -> Regex.IsMatch(i, p, o)
            | _ -> RX.InvalidArgs()

        [<hidden>]
        static member IsMatchOrContains =
            function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } ->
                try Regex.IsMatch(i, p, o)
                with ex -> i.ToUpper().Contains(p.ToUpper())
            | _ -> RX.InvalidArgs()

        static member GroupNames (rb: RegexBuilder) =
            rb
            |> function
            | { Pattern = Some(p) } ->
                let r = Regex(p)
                r.GetGroupNames()
                |> ToList
            | _ -> []

        static member MatchesDicts (rb: RegexBuilder) : IDictionary<string, string> list =
            let names =
                rb
                |> RX.GroupNames
            rb
            |> RX.Matches
            |+ fun x ->
                names
                |+ fun q -> q, x.Groups.[q].Value
                |> dict
            |> ToList

        static member Matches (rb: RegexBuilder) : Match list =
            rb
            |> function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } ->
                Regex.Matches(i, p, o)
                |> Seq.cast<Match>
                |> ToList
            | _ -> RX.InvalidArgs()

        static member FirstGroupMatches =
            let firstGroupValue(m: Match) = m.Groups.[1].Value
            RX.Matches
            >> (Seq.map firstGroupValue)
            >> ToList

        static member Create =
            function
            | { Pattern = Some pattern; RegexOptions = ro } -> Regex(pattern, ro)
            | _ -> RX.InvalidArgs()

        static member Replace =
            function
            | { Input = Some(i); Pattern = Some(p); Replacement = Some(r); RegexOptions = o; ReplacementCount = None } -> Regex.Replace(i, p, r, o)
            | { Input = Some(i); Pattern = Some(p); Replacement = Some(r); RegexOptions = o; ReplacementCount = Some rc } ->
                let mutable hit = 0
                let me = MatchEvaluator(fun m ->
                    hit <- hit + 1
                    if hit > rc then m.Value
                    else r)
                Regex.Replace(i, p, me, o)
            | { Input = Some(i); Pattern = Some(p); MatchEvaluator = Some(me); RegexOptions = _; ReplacementCount = None } -> Regex.Replace(i, p, me)
            | _ -> RX.InvalidArgs()

        static member Strip =
            function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } -> Regex.Replace(i, p, "", o)
            | _ -> RX.InvalidArgs()

        static member Match =
            function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } -> Regex.Match(i, p, o)
            | _ -> RX.InvalidArgs()

        [<DebuggerHidden>]
        static member private SplitBase x : bool * (string list) =
            let isMatch = x |> RX.IsMatch
            x
            |> function
            | { Input = Some(i); Pattern = Some(p); Replacement = None; RegexOptions = o } -> Regex.Split(i, p, o)
            | _ -> RX.InvalidArgs()
            |> Seq.toList
            |> fun x -> (isMatch, x)


        [<DebuggerHidden>]
        static member SplitOnSuccess =
            RX.SplitBase
            >> fun (a, b) ->
                if a then b
                else []

        [<DebuggerHidden>]
        static member Split : RegexBuilder -> string list = RX.SplitBase >> snd

        static member Escape = Regex.Escape

        static member Unescape = Regex.Unescape

    type ShouldIncludeStackTrace =
        | IncludeStackTrace
        | DoNotIncludeStackTrace

    type ShouldRecurse =
        | Recurse
        | DoNotRecurse

    let AppendIfNotEmpty (whatToAppend: string) (stringVal: string) =
        if String.IsNullOrEmpty stringVal then ""
        else stringVal + whatToAppend

    let rec TheNewBestExceptionToString (specificTypeHandlers: SpecificTypeHandler list) includeStackTrace recursive (ex: exn) =
        let rec work indentLevel (ex: exn) =
            if ex |> isNull then ""
            else
                let nextIndentLevel = indentLevel + 1
                let work = work nextIndentLevel
                let exnType = $"Exception: %s{ex.GetType().Name}"
                let msg = $"Message: {ex.Message}"
                let prependIndentation str =
                    let indent = new string(' ', 4 * indentLevel)
                    str
                    |> Lines
                    |+ (+) indent
                    |> JoinLines

                let stackTrace =
                    function
                    | IncludeStackTrace -> ex.StackTrace
                    | DoNotIncludeStackTrace -> ""

                let aggHandle (ex: AggregateException) =
                    ex.InnerExceptions
                    |> Seq.cast<exn>
                    |+ work
                    |> JoinLines

                let recursion =
                    function
                    | Recurse ->
                        match ex with
                        | null -> ""
                        | :? AggregateException as ex -> aggHandle ex
                        | ex ->
                            match (ex.InnerException) with
                            | null -> ""
                            | iex -> work iex
                    | _ -> ""

                let internalSpecificTypeHandlers =
                    match ex with
                    //Op: Auto
                    | null                                -> ""
                    | :? ReflectionTypeLoadException as x -> x.LoaderExceptions |+ TheNewBestExceptionToString specificTypeHandlers IncludeStackTrace Recurse |> JoinLines
                    | _                                   -> ""
                    //Op: End

                specificTypeHandlers
                |+ fun handler -> handler work ex
                |> UnwrapSomes
                |?| (String.IsNullOrWhiteSpace >> not)
                |> function
                | [] ->
                    [
                        exnType |> prependIndentation
                        msg |> prependIndentation
                        stackTrace includeStackTrace |> prependIndentation
                        recursion recursive
                        internalSpecificTypeHandlers
                    ]
                | x -> x
                |?| (String.IsNullOrWhiteSpace >> not)
                |+| ns
                |> JoinLines
                |> AppendIfNotEmpty "\r\n"
        if isNull ex then ""
        else work 0 ex |> TrimEnd

    let TheNewBestExceptionToStringFull ex = TheNewBestExceptionToString [] IncludeStackTrace Recurse ex

    let MapGridParallel (fn: 'item -> 'result) (rows: 'item list list) =
        let (+) lst fn =
            lst
            |> Array.ofList
            |> Array.Parallel.map fn
            |> ToList
        rows
        |+| fun row ->
            row
            |+| fn

    let NormalizeLineEndings (valx: string) (newLine: string): string =
        let nl : string =
            if newLine = "" then Environment.NewLine
            else newLine
        Regex.Replace(valx, @"\r\n|\n\r|\n|\r", nl)

    let FirstValidPathX paths =
        paths
        |?| fun x -> File.Exists x
        |> function
        | [] -> None
        | x :: _ -> Some x


    let ContainsCI (nonRegexNeedle: string) (haystack: string) : bool =
        let nonRegexNeedle = nonRegexNeedle |> ns |> Regex.Escape
        let haystack = haystack |> ns
        Regex.IsMatch(haystack, nonRegexNeedle, RegexOptions.IgnoreCase)

    let (<-<) txt label = Label label txt
    let (<--<) txt label = txt |> str |> Label (str label)
    let (>->) label txt = Label label txt
    let (>-->) label obj = Label (str label) (str obj)

    let (<<-<) obj label = LabelObj label obj
    let (>->>) label obj = LabelObj label obj

    open DerivedPatterns
    let (|Fst|) x = fst x
    let (|T2of3|) (_,x,_) = x
    let (|T3of3|) (_,_,x) = x
    let myNameofWeak (q: Expr) =
        match q with
        //Op: CollapseWhitespaceKeepIndent
        //Op: IndentOnWrapSpaces ->
        //Op: Sort
        //Op: Push
        //| Patterns.Lambda (Fst v)                               -> v.Name
        | Lambdas(_, Patterns.Call(T2of3 mi))                     -> mi.Name
        | Patterns.Call (T2of3 y)                                 -> y.Name
        | Patterns.Let(_, _, Lambdas(_, Patterns.Call(T2of3 mi))) -> mi.Name
        | Patterns.NewUnionCase (Fst o)                           -> o.Name
        | Patterns.PropertyGet (T2of3 mi)                         -> mi.Name
        | Patterns.ValueWithName (T3of3 n)                        -> n
        //Op: End
        | somethingElse                                           -> "Unexpected format" >--> somethingElse |> failwith

    let myNameof (q: Expr<_>) =
        match q with
        //Op: Pop
        //| Patterns.Lambda (Fst v)                               -> v.Name
        | Lambdas(_, Patterns.Call(T2of3 mi))                     -> mi.Name
        | Patterns.Call (T2of3 y)                                 -> y.Name
        | Patterns.Let(_, _, Lambdas(_, Patterns.Call(T2of3 mi))) -> mi.Name
        | Patterns.NewUnionCase (Fst o)                           -> o.Name
        | Patterns.PropertyGet (T2of3 mi)                         -> mi.Name
        | Patterns.ValueWithName (T3of3 n)                        -> n
        //Op: End
        | Patterns.Lambda (_, exp)                                -> myNameofWeak exp
        | somethingElse                                           -> "Unexpected format" >--> somethingElse |> failwith

    type DirectoryResult =
        | DirectoryExists of DirectoryInfo
        | DirectoryMissing of DirectoryInfo
        | DirectoryFailure of (string * exn)
        | DirectoryInfoConstructorFailed of string
        | EmptyDirectoryPathProvided

        static member ToOption =
            function
            | DirectoryExists x -> Some x
            | _ -> None

        static member ExistsToFn fn dr =
            dr
            |> function
            | DirectoryExists d -> fn d |> Some
            | _ -> None

        static member ExistsToFnQuiet fn dr = DirectoryResult.ExistsToFn fn dr |> ignore
        static member StringifyError =
            function
            | DirectoryExists di                   -> "DirectoryExists" >-> di.FullName
            | DirectoryMissing di                  -> "DirectoryMissing" >-> di.FullName
            | DirectoryFailure (txt, exn)          -> "DirectoryFailure" >-> txt >-> TheNewBestExceptionToStringFull exn
            | DirectoryInfoConstructorFailed txt   -> "DirectoryInfoConstructorFailed" >-> txt
            | EmptyDirectoryPathProvided           -> "EmptyDirectoryPathProvided"

    let ValidateDirectory path: DirectoryResult =
        try
            let di = DirectoryInfo path
            if di.Exists then DirectoryExists di
            else DirectoryMissing di
        with ex -> DirectoryFailure(path, ex)

    let DirectoryFullName (d: DirectoryInfo) = d.FullName

    let DirectoryParents(dir: DirectoryInfo): DirectoryInfo list =
        let rec _parents (dir: DirectoryInfo) lst: DirectoryInfo list =
            let p = dir.Parent
            if not (isNull p) then
                let q = _parents dir.Parent lst
                let r = dir :: q
                r
            else
                lst
        _parents dir List.empty

    let SafeCreate(di: DirectoryInfo) =
        let _create (di: DirectoryInfo) =
            try
                if di.Exists |> not then
                    di.Create()
                    di.Refresh()
                di.FullName |> ValidateDirectory

            with ex -> DirectoryFailure((di |> DirectoryFullName), ex)

        DirectoryParents di
        |> Seq.rev
        |+ _create
        |> Seq.last