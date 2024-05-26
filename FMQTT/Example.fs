namespace FMQTT

open FMQTT.FMQTT

module Example =

    // Connect to local mosquitto broker
    let ConnectToLocal () : MqttConnection =
        MqttConnection.New
        |> MqttConnection.SetUrl "localhost" 1883
        |> MqttConnection.SetCredentials "mosquitoo" "" //is this a typo "mosquitoo"? Does it matter?
        |> MqttConnection.Connect

[<AutoOpen>]
module Settings =
    let private mqtt = lazy MqttConnection.ConnectToEnvironmentMQTT()
    let MakeToggler = FMQTT.Togglers.MakeTogglerBase mqtt.Value

    let CreateRetainedBool (onChange: _ -> unit) defaultValue topic =
        //mqtt.Value.EnsureConnected()
        let v = mqtt.Value
        MQTTObservable.CreateRetainedBool v onChange defaultValue topic
