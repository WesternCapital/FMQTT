namespace FMQTT

module Togglers =
    let MakeTogglerBase (mqtt: MqttConnection) name =
        //let names = DiskObservable.CreateRetainedStringList ignore $"Toggles/.Names"
        //let obs = DiskObservable.CreateRetainedBool ignore true $"Toggles/{name}"
        let names = MQTTObservable.CreateRetainedStringList mqtt ignore $"Toggles/.Names"
        let observableToggler = MQTTObservable.CreateRetainedBool mqtt ignore true $"Toggles/{name}"
        if names.Value.Contains name |> not then
            names.Value.Add name
            names.Publish()
        observableToggler
    //let MakeToggler = FMQTT.Togglers.MakeTogglerBase mqtt.Value