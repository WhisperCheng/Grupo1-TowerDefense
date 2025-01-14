using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake() // Patrón Singleton
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

    [Header("Layers Globales")]
    public int layerEnemigos;
    public int layerAliados;
    public int layerJugador;
    public int layerUI;
    public int layerPath;
    public int layerTerreno;
    public int layerCorazon;

    [Header("Colores")]
    public Color colorVeneno;
    public MaterialPropertyBlock materialPropertyVeneno;

    //[Header("Tags de enemigos")]
    //public List<string> listaTagEnemigos;

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
            Debug.Log("Partida perdida");
        }
    }

    private void ConfigureMaterialPropertyBlocks()
    {
        // -- Veneno --
        materialPropertyVeneno = new MaterialPropertyBlock();
        materialPropertyVeneno.SetColor("_BaseColor", colorVeneno);
        materialPropertyVeneno.SetColor("_Color", colorVeneno); // por si el material no tiene el toon shader
        // Sombras
        materialPropertyVeneno.SetColor("_1st_ShadeColor", colorVeneno);
        materialPropertyVeneno.SetColor("_2nd_ShadeColor", colorVeneno);
    }

    public void addForestHearth()
    {
        ForestHearthAmount++;
    }

    /*public int getForestHearthAmount()
    {
        return forestHearthAmount;
    }*/
}
