using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake() // Patr�n Singleton
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("WayPoints")]
    public GameObject[] wayPoints;

    [Header("Respawn Enemigos")]
    public Transform respawnEnemigos;

    [Header("Controles Jugador")]
    public PlayerInput playerControls;

    [Header("Tags Globales")]
    public string tagCorazonDelBosque;
    public string tagEnemigos;
    public string tagAliados;
    public string tagWaypoints;
    public string tagPlayer;
    public string tagTerreno;
    public string tagPath;
    public string tagTorresCamino;
    public string tagTorres;
    public string tagPuentes;
    public string tagInteractableUI;
    public string tagCoronas;

    [Header("Layers Globales")]
    public int layerEnemigos;
    public int layerAliados;
    public int layerTorres;
    public int layerJugador;
    public int layerUI;
    public int layerPath;
    public int layerTerreno;
    public int layerCorazon;
    public int layerAreaDeco;
    public int layerTrap;
    public int layerPathBordes;

    [Header("Colores")]
    public Color colorVeneno;
    public MaterialPropertyBlock materialPropertyVeneno;

    public int ForestHearthAmount { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        ConfigureMaterialPropertyBlocks();
    }

    // Update is called once per frame
    void Update()
    {
        if (ForestHearthAmount <= 0)
        {
            // TODO: Perder partida
            GameUIManager.Instance.LoseLevel();
            AudioManager.instance.musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.menuLose, this.transform.position);
            Time.timeScale = 0f;
            Debug.Log("Partida perdida");
            this.enabled = false;
        }
    }

    private void ConfigureMaterialPropertyBlocks()
    {
        materialPropertyVeneno = ColorUtils.CreateToonShaderPropertyBlock(colorVeneno);
    }

    public void addForestHearth()
    {
        ForestHearthAmount++;
    }
}
