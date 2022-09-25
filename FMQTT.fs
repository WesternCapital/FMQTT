namespace FMQTT

open MQTTnet.Client
open MQTTnet
open System.Threading
open System.Threading.Tasks

module FMQTT = 
    type MyMQTT =
        {
            Factory: MqttFactory
            Client: IMqttClient
            OptionsBuilder: MqttClientOptionsBuilder
        }
        static member New = 
            let factory = new MqttFactory()
            let client = factory.CreateMqttClient()
            {
                Factory = factory
                Client = client
                OptionsBuilder = new MqttClientOptionsBuilder()
            }
        static member SetClientId (clientId: string) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithClientId(clientId)}
        static member SetUrl (url: string) (port: int) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTcpServer(url, port)}
        static member SetCredentials (user: string) (pass: string) (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithCredentials(user, pass)}
        static member UseTLS (mq: MyMQTT) = {mq with OptionsBuilder = mq.OptionsBuilder.WithTls()}
        static member Connect (mq: MyMQTT) = 
            printf "Connecting to broker... "
            mq.Client.ConnectAsync(mq.OptionsBuilder.Build(), CancellationToken.None).Wait()
            printfn "Connected!"
            mq
        
        member this.SubscribeToTopic (topic: string) (fn: string -> unit) = 
            let sub = this.Factory.CreateSubscribeOptionsBuilder() |> fun x -> x.WithTopicFilter(fun f -> f.WithTopic(topic) |> ignore).Build()
            this.Client.SubscribeAsync(sub, CancellationToken.None).Wait()
            this.Client.add_ApplicationMessageReceivedAsync(fun x -> 
                let y = x.ApplicationMessage.ConvertPayloadToString()
                fn y
                Task.CompletedTask
            )
        member this.PublishMessage (topic: string) (data: string) = 
            let amb = (new MqttApplicationMessageBuilder()).WithTopic(topic).WithPayload(data).Build()
            printfn "Sending message to mqtt..."
            this.Client.PublishAsync(amb, CancellationToken.None) |> ignore