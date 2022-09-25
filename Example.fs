namespace FMQTT

open FMQTT.FMQTT

module Example = 

    // Connect to local mosquitto broker
    let ConnectToLocal () : MyMQTT = 
        MyMQTT.New
        |> MyMQTT.SetUrl "localhost" 1883
        |> MyMQTT.SetCredentials "mosquitoo" ""
        |> MyMQTT.Connect

