using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallEmitter : MonoBehaviour
{
    private StudioEventEmitter emitter;

    void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.waterfallAmbience, this.gameObject);
        emitter.OverrideAttenuation = true;
        emitter.OverrideMinDistance = 1;
        emitter.OverrideMaxDistance = 50;
        emitter.Play();
    }
}
