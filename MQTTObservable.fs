namespace FMQTT

open MQTTnet.Client
open MQTTnet
open System.Threading
open System.Threading.Tasks
open FMQTT
open FMQTT.FMQTT
open System

module MQTTObservable =
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

    type MQTTObservable() =
        member val private backingValue = "" with get, set
        member val private topicPath = "" with get, set
        
        member val private client : MyMQTT =
            let envVar n = System.Environment.GetEnvironmentVariable($"MQTT_{n}", EnvironmentVariableTarget.User)
            let vars = 
                {|
                    URL = envVar "URL" 
                    Port = envVar "Port" |> parseInt |> function Some x -> x | _ -> 0
                    User = envVar "User" 
                    Password = envVar "Password"
                |}
            MyMQTT.New
            |> MyMQTT.SetUrl vars.URL vars.Port
            |> MyMQTT.SetCredentials  vars.User vars.Password
            |> fun x -> 
                x |> MyMQTT.Connect |> ignore
                x
        
        member this.Init() =
            this.client.SubscribeToTopic 
                this.topicPath 
                (fun x -> this.backingValue <- x)
        
        member this.Value
            with get() = this.backingValue
            and set(stringValue) = 
                this.client.PublishMessage this.topicPath stringValue
                this.backingValue <- stringValue

        new(topicPath) as this =
            MQTTObservable()
            then
                this.topicPath <- topicPath
                this.Init()
                