using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Dynamic;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public EventInstance musicEventInstance;

    private List<EventInstance> eventInstances;

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
        eventInstances = new List<EventInstance>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        SFXBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        //Esto no es precisamente optimo ni escalable sin embargo, no hay tiempo para investigar mejores maneras
        string currentScene = SceneManager.GetActiveScene().name;

        // Condicional según el nombre de la escena
        if (currentScene == "LevelOne" || currentScene == "TutorialScene")
        {
           InitializeMusic(FMODEvents.instance.music);
        }
        else if (currentScene == "MainMenu")
        {
           
        }
        masterVolume = PlayerPrefs.GetFloat("masterVolume", masterVolume);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", SFXVolume);
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
        eventInstances.Add(eventInstance);
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

   


    public void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateInstance(musicEventReference);
        musicEventInstance.start();
    }




    private void CleanUp()
    {

        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();

        }

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
