namespace FMQTT

module Togglers =
    let MakeTogglerBase (mqtt: MqttConnection) name =
        #if USEDISK
        let names = DiskObservable.CreateRetainedStringList ignore $"Toggles/.Names"
        let obs = DiskObservable.CreateRetainedBool ignore true $"Toggles/{name}"
        #else
        let names = MQTTObservable.CreateRetainedStringList mqtt ignore $"Toggles/.Names"
        let observableToggler = MQTTObservable.CreateRetainedBool mqtt ignore true $"Toggles/{name}"
        #endif
        if names.Value.Contains name |> not then
            names.Value.Add name
            names.Publish()
        observableToggler
