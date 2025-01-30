using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class PostProcessingControl : MonoBehaviour
{
    public static PostProcessingControl Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private PostProcessVolume _postProcessVolume;
    private DepthOfField _depthOfField;
    int maxValueFocalLegth = 300;
    int minValueFocalLeght = 59;
    // Start is called before the first frame update
    void Start()
    {
        _postProcessVolume = GetComponent<PostProcessVolume>();
        _postProcessVolume.profile.TryGetSettings<DepthOfField>(out _depthOfField);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PostProcessingVolumeOn()
    {
        _depthOfField.focalLength.value = maxValueFocalLegth;
    }
    public void PostProcessingVolumeOff()
    {
        _depthOfField.focalLength.value = minValueFocalLeght;
    }
}
