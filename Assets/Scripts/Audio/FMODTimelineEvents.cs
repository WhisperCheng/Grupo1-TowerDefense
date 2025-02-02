using UnityEngine;
using UnityEngine.Playables;
using FMOD.Studio;
using FMODUnity;
using static UnityEngine.Rendering.DebugUI;

public class FMODTimelineMusic : MonoBehaviour
{
    public PlayableDirector director;
    public EventReference musicEvent; 

    private EventInstance musicInstance;

    private void Start()
    {
        // Crea la instancia de la m�sica, pero no la reproduzcas todav�a.
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
    }

    public void PlayMusic()
    {

        // Inicia la m�sica si no est� reproduci�ndose
        PLAYBACK_STATE state;
        musicInstance.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING)
        {
            musicInstance.start();
        }
    }


    public void StopMusic()
    {
        // Detiene la m�sica con fade out
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        // Libera la instancia cuando termine la cinem�tica
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}