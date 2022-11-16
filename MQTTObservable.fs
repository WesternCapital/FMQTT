namespace FMQTT

open MQTTnet.Client
open MQTTnet
open System.Threading
open System.Threading.Tasks
open FMQTT
open FMQTT.FMQTT
open System

open MQTTnet.Protocol

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
                (fun x -> this.backingValue <- x.ApplicationMessage.ConvertPayloadToString())
                //Encoding.Default.GetString(x.ApplicationMessage.Payload)
        
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
                

    type MQTTObservableGeneric<'a> private () =
        member val private backingValue : 'a option = None with get, set
        member val private initVal : 'a option = None with get, set
        member val private clientModel = FMQTT.ClientModel.Create<'a> "" with get, set
        member val private serializer : 'a -> string = (fun x -> x.ToString()) with get, set
        member val private deserializer : string -> 'a = (fun x -> failwith "needs fn") with get, set
        member val private hasReceivedCallback : bool = false with get, set
        member val private client : MyMQTT =
            let envVar n =
                
                let getVar userLevel = 
                    let k = $"MQTT_{n}"
                    let envVars = System.Environment.GetEnvironmentVariables(userLevel)
                    if envVars.Contains k then Some <| envVars.[k].ToString()
                    else None 
                    //System.Environment.GetEnvironmentVariable(k, u)
                getVar EnvironmentVariableTarget.User
                |> function
                | None -> getVar EnvironmentVariableTarget.Machine
                | Some x -> Some x
                |> Option.defaultValue ""
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
            |> MyMQTT.Connect
        
        member private this.SetBackingValue v = this.backingValue <- Some v
        
        member this.Init() : unit =
            let onChange (stringToDeserialize: string) : unit = 
                let noopp x = x
                try 
                    this.deserializer stringToDeserialize
                with ex -> 
                    noopp ex |> ignore
                    this.initVal.Value
                |> this.SetBackingValue
                this.backingValue.Value
                |> fun x -> x
                |> this.clientModel.OnChangeStrong
                |> fun x -> x
                this.hasReceivedCallback <- true
            this.client.SubscribeToTopicWithModel
                { this.clientModel with OnChangeWeak = onChange }
            ()
        
        member this.SetValue v = this.Value <- v
        
        member this.Publish() = this.client.PublishMessage this.clientModel.Topic (this.serializer this.backingValue.Value)
        
        member this.Value
            with get() = this.backingValue.Value
            and set(stringValue) = 
                this.SetBackingValue stringValue
                this.Publish()

        member this.WaitForCallback (ms: int) =
            let start = DateTime.Now
            let ms = (float ms)
            while this.hasReceivedCallback |> not && DateTime.Now.Subtract(start).TotalMilliseconds < ms do
                System.Threading.Thread.Sleep 10
            let noop x = 
                ()
            noop ((DateTime.Now.Subtract(start).TotalMilliseconds), this.hasReceivedCallback)
            if this.hasReceivedCallback |> not then
                this.SetBackingValue this.initVal.Value
            
        static member Create (s: 'a -> string) (d: string -> 'a) (initVal: 'a) (client: ClientModel<'a>) =
            let t = new MQTTObservableGeneric<'a>()
            t.clientModel <- client
            t.serializer <- s
            t.deserializer <- d
            t.initVal <- Some initVal
            t.Init()
            client.SendOnSubcribe
            |> function
            | MqttRetainHandling.SendAtSubscribe
            | MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly -> 
                t.WaitForCallback 1000
                let q = t.hasReceivedCallback
                if t.backingValue.IsNone  then 
                    t.SetBackingValue t.initVal.Value
                if not q then t.SetValue t.initVal.Value
            | MqttRetainHandling.DoNotSendOnSubscribe -> 
                t.Value <- initVal
            | _ -> ()
            t
        
        static member CreateRetained<'a> (s: 'a -> string) (d: string -> 'a) (onChange: 'a -> unit) (iv: 'a) (topic: string) =
            ClientModel.Create<'a> topic
            |> ClientBuilder.SendOnSubcribe
            |> ClientBuilder.RetainAsPublished
            |> ClientBuilder.OnChange onChange
            |> MQTTObservableGeneric.Create s d iv

        static member CreateRetainedBool (onChange: bool -> unit) defaultValue topic : MQTTObservableGeneric<bool> = 
            MQTTObservableGeneric.CreateRetained<bool>
                (fun (x: bool) -> x.ToString()) 
                (fun s ->
                    System.Boolean.TryParse s
                    |> function
                    | true, x -> x
                    | false, _ -> false
                    ) 
                onChange 
                defaultValue
                topic
    