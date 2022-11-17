namespace FMQTT

open MQTTnet.Client
open MQTTnet
open MQTTnet.Protocol
open System.Threading
open System.Threading.Tasks
open MQTTnet
open System
open MQTTnet.Packets
open System.Collections.Generic
open Utils
open System.Linq
[<AutoOpen>]
module FMQTT = 
    
    let tee (x: 'obj -> unit) (y: 'obj) : 'obj =
        x y
        y
    let (|AsPayloadString|) (x: MqttApplicationMessageReceivedEventArgs) = System.Text.Encoding.ASCII.GetString(x.ApplicationMessage.Payload)
    let (|--) a b = a |> tee b

    type MQTTMessageEventArgs = MqttApplicationMessageReceivedEventArgs
        
    type ClientModel<'a> = 
        {
            NoLocal : bool
            Retain : bool
            Topic: string
            OnChangeWeak: string -> unit
            OnChangeStrong: 'a -> unit
            SendOnSubcribe: MqttRetainHandling
        }
        static member Create topic =
            {
                NoLocal = false
                Retain = false
                OnChangeStrong = ignore
                OnChangeWeak = ignore
                SendOnSubcribe = MqttRetainHandling.DoNotSendOnSubscribe
                Topic = topic
            }

    type ClientBuilder =
        static member NoLocal (b: ClientModel<_>)         = { b with NoLocal        = true }
        static member SendOnSubcribe (b: ClientModel<_>)  = { b with SendOnSubcribe = MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly }
        static member Retain (b: ClientModel<_>)          = { b with Retain         = true }
        static member Topic x (b: ClientModel<_>)         = { b with Topic          = x }
        static member OnChange fn (b: ClientModel<'a>)    = { b with OnChangeStrong = fn }

    type MqttConnection =
        {
            BrokerName: string
            Factory: MqttFactory
            Client: IMqttClient
            OptionsBuilder: MqttClientOptionsBuilder
            EventHandlers: Dictionary<string, MQTTMessageEventArgs -> unit>
        }
        static member New = 
            let factory = new MqttFactory()
            let client = factory.CreateMqttClient()
            
            {
                BrokerName = ""
                Factory = factory
                Client = client
                OptionsBuilder = new MqttClientOptionsBuilder()
                EventHandlers = new Dictionary<string, MQTTMessageEventArgs -> unit>()
            }
            |-- fun x ->
                    x.Client.add_ApplicationMessageReceivedAsync(fun xx -> 
                        if x.EventHandlers.ContainsKey xx.ApplicationMessage.Topic then
                            x.EventHandlers.[xx.ApplicationMessage.Topic] xx
                        Task.CompletedTask
                    )
        static member SetBrokerName (bn: string) (mq: MqttConnection) = {mq with BrokerName = bn}
        static member SetClientId (clientId: string) (mq: MqttConnection) = {mq with OptionsBuilder = mq.OptionsBuilder.WithClientId(clientId)}
        static member SetUrl (url: string) (port: int) (mq: MqttConnection) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTcpServer(url, port)}
        static member SetCredentials (user: string) (pass: string) (mq: MqttConnection) = {mq with OptionsBuilder = mq.OptionsBuilder.WithCredentials(user, pass)}
        static member UseTLS (mq: MqttConnection) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTls()}
        static member Connect (mq: MqttConnection) = 
            //printf $"Connecting to broker: {mq.BrokerName}..."
            mq.Client.ConnectAsync(mq.OptionsBuilder.Build(), CancellationToken.None).Wait()
            //printfn "Connected!"
            mq
        
        member this.SubscribeToTopicWithModel (b: ClientModel<_>) = 
            let sub =
                this.Factory.CreateSubscribeOptionsBuilder() 
                |> fun x -> 
                    MqttTopicFilterBuilder()
                    |> fun x -> x.WithRetainHandling b.SendOnSubcribe
                    |> fun x -> x.WithRetainAsPublished b.Retain
                    |> fun x -> x.WithTopic b.Topic
                    |> fun x -> x.WithNoLocal b.NoLocal
                    |> x.WithTopicFilter
                    |> fun x -> x.Build()
            this.Client.SubscribeAsync(sub, CancellationToken.None).Wait()
            if this.EventHandlers.ContainsKey b.Topic then
                let x = 5
                let o = this.EventHandlers.[b.Topic] 
                this.EventHandlers.[b.Topic] <- (fun x -> o x; x.ApplicationMessage.ConvertPayloadToString() |> b.OnChangeWeak)
                
            else
                let x = 5
                this.EventHandlers.Add(b.Topic, 
                    fun x -> 
                        x.ApplicationMessage.ConvertPayloadToString() |> b.OnChangeWeak)
            //this.Client.add_ApplicationMessageReceivedAsync(fun x -> 
            //    let y = x.ApplicationMessage.ConvertPayloadToString()
            //    b.OnChangeWeak y
            //    Task.CompletedTask
            //)
        member this.SubscribeToTopic (topic: string) (fn: MqttApplicationMessageReceivedEventArgs -> unit) = 
            let sub = this.Factory.CreateSubscribeOptionsBuilder() |> fun x -> x.WithTopicFilter(fun f -> f.WithTopic(topic) |> ignore).Build()
            this.EventHandlers.Add(topic, fn)
            this.Client.SubscribeAsync(topic).Wait()

        member this.SubscribeToTopicBasic (topic: string) (fn: string -> unit) = this.SubscribeToTopic topic (fun x -> x.ApplicationMessage.ConvertPayloadToString() |> fn)

        member this.PublishMessage (topic: string) (data: string) = 
            let amb = (new MqttApplicationMessageBuilder()).WithRetainFlag().WithTopic(topic).WithPayload(data).Build()
            //printfn "Sending message to mqtt..."
            try
                this.Client.PublishAsync(amb, CancellationToken.None) |> ignore
            with ex -> 
                this.Client.ConnectAsync(this.OptionsBuilder.Build(), CancellationToken.None).Wait()
                this.PublishMessage topic data
                //let n x = ()
                //n ex
                //()
   
        static member ConnectToEnvironmentMQTT() =
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
            MqttConnection.New
            |> MqttConnection.SetUrl vars.URL vars.Port
            |> MqttConnection.SetCredentials  vars.User vars.Password
            |> MqttConnection.Connect
    
    type MQTTObservableGeneric<'a> private () =
        member val private backingValue : 'a option = None with get, set
        member val private initVal : 'a option = None with get, set
        member val private clientModel = ClientModel.Create<'a> "" with get, set
        member val private serializer : 'a -> string = (fun x -> x.ToString()) with get, set
        member val private deserializer : string -> 'a = (fun x -> failwith "needs fn") with get, set
        member val private hasReceivedCallback : bool = false with get, set
        member val private client : MqttConnection option = None with get,set
        
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
            this.client.Value.SubscribeToTopicWithModel
                { this.clientModel with OnChangeWeak = onChange }
            ()
        
        member this.SetValue v = this.Value <- v
        
        member this.Publish() = this.client.Value.PublishMessage this.clientModel.Topic (this.serializer this.backingValue.Value)
        
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
            
        static member Create m (s: 'a -> string) (d: string -> 'a) (initVal: 'a) (client: ClientModel<'a>) =
            let t = new MQTTObservableGeneric<'a>()
            t.clientModel <- client
            t.serializer <- s
            t.deserializer <- d
            t.initVal <- Some initVal
            t.client <- Some m
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
        
        static member CreateRetained<'a> m (s: 'a -> string) (d: string -> 'a) (onChange: 'a -> unit) (iv: 'a) (topic: string) =
            ClientModel.Create<'a> topic
            |> ClientBuilder.SendOnSubcribe
            |> ClientBuilder.Retain
            |> ClientBuilder.OnChange onChange
            |> MQTTObservableGeneric.Create m s d iv

        static member CreateRetainedBool m (onChange: bool -> unit) defaultValue topic : MQTTObservableGeneric<bool> = 
            MQTTObservableGeneric.CreateRetained<bool>
                m
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
    
        static member CreateRetainedInt m (onChange: int -> unit) defaultValue topic : MQTTObservableGeneric<int> = 
            MQTTObservableGeneric.CreateRetained<int>
                m
                (fun i -> i.ToString())
                (fun i ->
                    System.Int32.TryParse i
                    |> function
                    | true, i -> i
                    | false, _ -> 0
                    ) 
                onChange 
                defaultValue
                topic