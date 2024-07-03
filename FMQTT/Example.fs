namespace FMQTT

open FMQTT.FMQTTCore

module LocalBroker =
    // Connect to local mosquitto broker
    let ConnectToLocal () : MqttConnection =
        let password = System.Environment.GetEnvironmentVariable "MQTT_Password"
        let url = System.Environment.GetEnvironmentVariable "MQTT_URL"
        let port =
            try
                System.Environment.GetEnvironmentVariable "MQTT_Port" |> int
            with _ -> 1883
        let user = System.Environment.GetEnvironmentVariable "MQTT_User"
        MqttConnection.New
        |> MqttConnection.SetUrl url port
        |> MqttConnection.SetCredentials user password
        |> MqttConnection.Connect

[<AutoOpen>]
module Settings =
    let private mqtt = lazy MqttConnection.ConnectToEnvironmentMQTT()
    let MakeToggler = FMQTT.Togglers.MakeTogglerBase mqtt.Value

    let CreateRetainedBool (onChange: _ -> unit) defaultValue topic =
        //mqtt.Value.EnsureConnected()
        let v = mqtt.Value
        MQTTObservable.CreateRetainedBool v onChange defaultValue topic