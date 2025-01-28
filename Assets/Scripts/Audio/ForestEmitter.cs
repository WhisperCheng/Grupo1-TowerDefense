using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestEmitter : MonoBehaviour
{
    private StudioEventEmitter emitter;

    void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.forestAmbience, this.gameObject);
        emitter.Play();
    }
}
