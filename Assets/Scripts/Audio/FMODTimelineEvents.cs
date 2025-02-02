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
        // Crea la instancia de la música, pero no la reproduzcas todavía.
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
    }

    public void PlayMusic()
    {

        // Inicia la música si no está reproduciéndose
        PLAYBACK_STATE state;
        musicInstance.getPlaybackState(out state);
        if (state != PLAYBACK_STATE.PLAYING)
        {
            musicInstance.start();
        }
    }


    public void StopMusic()
    {
        // Detiene la música con fade out
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        // Libera la instancia cuando termine la cinemática
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}