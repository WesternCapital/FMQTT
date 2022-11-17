module Utils

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
