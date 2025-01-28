using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Dynamic;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    public EventInstance musicEventInstance;
    private List<StudioEventEmitter> eventEmitters;

    [Header("Volume")]
    [Range(0, 1)]
    [SerializeField] public float masterVolume = 1;
    [Range(0, 1)]
    [SerializeField] public float musicVolume = 1;
    [Range(0, 1)]
    [SerializeField] public float SFXVolume = 1;


    private Bus masterBus;
    private Bus musicBus;
    private Bus SFXBus;
    public static AudioManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene.");
        }
        instance = this;
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:Music/");
        SFXBus = RuntimeManager.GetBus("bus:SFXMusic/");
    }

    private void Start()
    {
        InitializeMusic(FMODEvents.instance.music);
        
    }
    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        SFXBus.setVolume(SFXVolume);
    }
    public EventInstance CreateInstance (EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();  
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }


    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);    
    }

   


    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }




    private void CleanUp()
    {

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
            CleanUp();
    }


}
