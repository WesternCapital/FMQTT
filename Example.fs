namespace FMQTT

open FMQTT.FMQTT

module Example = 

    // Connect to local mosquitto broker
    let ConnectToLocal () : MqttConnection = 
        MqttConnection.New
        |> MqttConnection.SetUrl "localhost" 1883
        |> MqttConnection.SetCredentials "mosquitoo" ""
        |> MqttConnection.Connect
