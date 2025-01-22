using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("Music")]
    [field: SerializeField] public EventReference music { get; private set; }

    [field: Header("Magic Attack")]
    [field: SerializeField] public EventReference magicAttack { get; private set; }

    [field: Header("Magic Impact")]
    [field: SerializeField] public EventReference magicImpact { get; private set; }

    [field: Header("Rose Shoot")]
    [field: SerializeField] public EventReference roseShoot { get; private set; }

    [field: Header("Hitmarker")]
    [field: SerializeField] public EventReference hitmarker { get; private set; }

    public static FMODEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events intance in the scene");
        }
        instance = this;
    }
}
