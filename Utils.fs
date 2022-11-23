module internal Utils
    open ExceptionalCode

    let Trim(text: string): string =
        if isNull text then ""
        else text.Trim()
        
    let private tryParseWith (tryParseFunc: string -> (bool * _)) =
        tryParseFunc
        >> function
        | true, v -> Some v
        | false, _ -> None
    
    let private tryParseTrimmedWith (tryParseFunc: string -> (bool * 'b)) v = v |> Trim |> tryParseWith tryParseFunc
    
    let parseInt = tryParseTrimmedWith System.Int32.TryParse
    
    let tee (x: 'obj -> unit) (y: 'obj) : 'obj =
        x y
        y
    
    let BREAK t = 
        if str t = "Dev/Winmerge" then
            //System.Diagnostics.Debugger.Launch() |> ignore
            System.Diagnostics.Debugger.Break()
        ()
    let noop x = 
        ()
    