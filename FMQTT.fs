namespace FMQTT

open MQTTnet.Client
open MQTTnet
open System.Threading
open System.Threading.Tasks
open MQTTnet
open System
open MQTTnet.Packets
open System.Collections.Generic

module FMQTT = 
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
            printf $"Connecting to broker: {mq.BrokerName}..."
            mq.Client.ConnectAsync(mq.OptionsBuilder.Build(), CancellationToken.None).Wait()
            printfn "Connected!"
            mq
        member this.SubscribeToTopic (topic: string) (fn: MqttApplicationMessageReceivedEventArgs -> unit) = 
            let sub = this.Factory.CreateSubscribeOptionsBuilder() |> fun x -> x.WithTopicFilter(fun f -> f.WithTopic(topic) |> ignore).Build()
            //this.Client.SubscribeAsync((new MqttTopicFilterBuilder()).WithTopic(topic).Build()).Wait()
            //this.Client.SubscribeAsync(sub, CancellationToken.None).Wait()
            this.EventHandlers.Add(topic, fn)
            this.Client.SubscribeAsync(topic).Wait()
        member this.SubscribeToTopicBasic (topic: string) (fn: string -> obj) = this.Client.SubscribeAsync(topic).Wait()
        member this.PublishMessage (topic: string) (data: string) = 
            let amb = (new MqttApplicationMessageBuilder()).WithTopic(topic).WithPayload(data).Build()
            printfn "Sending message to mqtt..."
            this.Client.PublishAsync(amb, CancellationToken.None) |> ignore