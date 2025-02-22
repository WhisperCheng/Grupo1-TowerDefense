using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AnimaticController : MonoBehaviour
{
    [SerializeField] private TMP_Text textToFade; 
    [SerializeField] private float fadeSpeed = 0.15f;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject nextCanvas; 

    private bool isFading = true;

    public bool acabarCutscene = false;

    public PlayableDirector timeline;
    private void Start()
    {
            
            Color textColor = textToFade.color;
            textColor.a = 0;
            textToFade.color = textColor;
        
        
    }

    private void Update()
    {
        if (isFading && textToFade != null)
        {
            
            Color textColor = textToFade.color;
            textColor.a += fadeSpeed * Time.deltaTime;

           
            if (textColor.a >= 1)
            {
                textColor.a = 1;
                isFading = false;
            }

            textToFade.color = textColor;
        }
        
        EndCutscene();
    }

    public void CutsceneHasEnded()
    {
        acabarCutscene = true;
    }

    public void EndCutscene()
    {
        if (Input.GetKeyDown(KeyCode.Space) || acabarCutscene)
        {
            timeline.Stop();
            parentObject.SetActive(false);
            nextCanvas.SetActive(true);

            FMODTimelineMusic.Instance.StopMusic();
            AudioManager.instance.InitializeMusic(FMODEvents.instance.musicMenu);
        }
    }

}