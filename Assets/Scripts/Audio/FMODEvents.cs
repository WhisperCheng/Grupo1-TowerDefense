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

    [field: Header("BombardierExplosion")]
    [field: SerializeField] public EventReference bombardierExplosion { get; private set; }

    [field: Header("Rose Shoot")]
    [field: SerializeField] public EventReference roseShoot { get; private set; }

    [field: Header("Carnivorous Shoot")]
    [field: SerializeField] public EventReference carnivorousShoot { get; private set; }

    [field: Header("Hitmarker")]
    [field: SerializeField] public EventReference hitmarker { get; private set; }

    [field: Header("buildPlant")]
    [field: SerializeField] public EventReference buildPlant { get; private set; }

    [field: Header("runeStun")]
    [field: SerializeField] public EventReference runeStun { get; private set; }

    [field: Header("mushroomAttack")]
    [field: SerializeField] public EventReference mushroomAttack { get; private set; }

    [field: Header("MiniEnemyHit")]
    [field: SerializeField] public EventReference miniKnightHit { get; private set; }

    [field: Header("BigEnemyHit")]
    [field: SerializeField] public EventReference bigKnightHit { get; private set; }

    [field: Header("ForestAmbience")]
    [field: SerializeField] public EventReference forestAmbience { get; private set; }

    [field: Header("WatefallAmbience")]
    [field: SerializeField] public EventReference waterfallAmbience { get; private set; }

    [field: Header("MusicMenu")]
    [field: SerializeField] public EventReference musicMenu { get; private set; }

    [field: Header("MenuClick")]
    [field: SerializeField] public EventReference menuClick { get; private set; }

    [field: Header("MenuWin")]
    [field: SerializeField] public EventReference menuWin { get; private set; }

    [field: Header("MenuLose")]
    [field: SerializeField] public EventReference menuLose { get; private set; }



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
