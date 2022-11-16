namespace FMQTT

open MQTTnet.Client
open MQTTnet
//open Protocol
open MQTTnet.Protocol
open System.Threading
open System.Threading.Tasks
open MQTTnet
open System
open MQTTnet.Packets
open System.Collections.Generic

module FMQTT = 
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
        static member NoLocal (b: ClientModel<_>) = { b with NoLocal            = true }
        static member SendOnSubcribe  (b: ClientModel<_>) = { b with SendOnSubcribe            = MqttRetainHandling.SendAtSubscribeIfNewSubscriptionOnly }
        static member Retain  (b: ClientModel<_>) = { b with Retain  = true }
        static member Topic x (b: ClientModel<_>) = { b with Topic = x }
        static member OnChange (x: 'a -> unit) (b: ClientModel<'a>) = { b with OnChangeStrong = x }

    let tee (x: 'obj -> unit) (y: 'obj) : 'obj =
        x y
        y
    let (|AsPayloadString|) (x: MqttApplicationMessageReceivedEventArgs) = System.Text.Encoding.ASCII.GetString(x.ApplicationMessage.Payload)
    let (|--) a b = a |> tee b

    type MQTTMessageEventArgs = MqttApplicationMessageReceivedEventArgs

    type MyMQTT =
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
        static member SetBrokerName (bn: string) (mq: MyMQTT) = {mq with BrokerName = bn}
        static member SetClientId (clientId: string) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithClientId(clientId)}
        static member SetUrl (url: string) (port: int) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTcpServer(url, port)}
        static member SetCredentials (user: string) (pass: string) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithCredentials(user, pass)}
        static member UseTLS (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTls()}
        static member Connect (mq: MyMQTT) = 
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
            this.Client.add_ApplicationMessageReceivedAsync(fun x -> 
                let y = x.ApplicationMessage.ConvertPayloadToString()
                b.OnChangeWeak y
                Task.CompletedTask
            )
        member this.SubscribeToTopic (topic: string) (fn: MqttApplicationMessageReceivedEventArgs -> unit) = 
            let sub = this.Factory.CreateSubscribeOptionsBuilder() |> fun x -> x.WithTopicFilter(fun f -> f.WithTopic(topic) |> ignore).Build()
            this.EventHandlers.Add(topic, fn)
            this.Client.SubscribeAsync(topic).Wait()

        member this.SubscribeToTopicBasic (topic: string) (fn: string -> unit) = this.SubscribeToTopic topic (fun x -> x.ApplicationMessage.ConvertPayloadToString() |> fn)

        member this.PublishMessage (topic: string) (data: string) = 
            let amb = (new MqttApplicationMessageBuilder()).WithRetainFlag().WithTopic(topic).WithPayload(data).Build()
            //printfn "Sending message to mqtt..."
            this.Client.PublishAsync(amb, CancellationToken.None) |> ignore
   