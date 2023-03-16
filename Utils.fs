module internal Utils
    open ExceptionalCode
    open System
    open System.Collections.Generic
    open System.Diagnostics

    let Trim(text: string): string =
        if isNull text then ""
        else text.Trim()

    let private tryParseWith (tryParseFunc: string -> (bool * _)) =
        tryParseFunc
        >> function
        | true, v -> Some v
        | false, _ -> None

    let private tryParseTrimmedWith (tryParseFunc: string -> (bool * 'b)) v = v |> Trim |> tryParseWith tryParseFunc

    let parseInt = tryParseTrimmedWith Int32.TryParse

    let tee (x: 'obj -> unit) (y: 'obj) : 'obj =
        x y
        y

    let BREAK t =
        if str t = "Dev/Winmerge" then
            //System.Diagnostics.Debugger.Launch() |> ignore
            Debugger.Break()
        ()

    let noop x =
        ()

    let ns x =
        if isNull x then ""
        else x


    let RunProcessStartInfoWithOutputFullest timeout (onOutput: (string -> unit) option) (onError: (string -> unit) option) (cleanOut: string list -> string list) (procStartInfo: ProcessStartInfo) =
        let timeout =
            if timeout = 0 then Int32.MaxValue else timeout
        procStartInfo.RedirectStandardOutput <- true
        procStartInfo.RedirectStandardError <- true
        procStartInfo.UseShellExecute <- false

        let timer = Stopwatch.StartNew()
        let outputs = List<string>()
        let errors = List<string>()
        let outputHandler f (_sender: obj) (args: DataReceivedEventArgs) = f args.Data
        let p = new Process(StartInfo = procStartInfo)
        p.OutputDataReceived.AddHandler(DataReceivedEventHandler(outputHandler outputs.Add))
        p.ErrorDataReceived.AddHandler(DataReceivedEventHandler(outputHandler errors.Add))
        let r = (<|>)
        
        onOutput 
        |> fun x -> x
        <|> fun (fn: string -> unit) -> 
                p.ErrorDataReceived.AddHandler(DataReceivedEventHandler(outputHandler fn))
                |> fun x -> x
                |> ignore
        onError  <|> fun fn -> p.ErrorDataReceived.AddHandler(DataReceivedEventHandler(outputHandler fn))
        let started =
            try
                p.Start()
            with ex ->
                //ProcessStartInfoPipe.ToRunnableString procStartInfo
                //|> noop
                ex.Data.Add("filename", procStartInfo.FileName)
                reraise ()

        if not started then failwithf "Failed to start process %s" procStartInfo.FileName
        //printfn "Started %s %s with pid %i" procStartInfo.FileName procStartInfo.Arguments p.Id

        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        try
            p.WaitForExit timeout
            |> ignore
        with ex ->
            raise ex

        timer.Stop()
        //printfn "Finished %s after %i milliseconds" procStartInfo.FileName timer.ElapsedMilliseconds
        let remBlanks l =
            l
            |?| (String.IsNullOrEmpty >> not)
        TryRepeatedly 5 100
            ^ fun () ->
                let q =
                    {|
                        Outputs = outputs |>| cleanOut
                        Errors = errors |>| remBlanks
                        ProcessID = p.Id
                        ExitCode = p.ExitCode
                    |}
                let r = 5
                q
        |> function
        | Ok x -> x
        | Error err -> raise err