using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Tags Globales")]
    public string tagCorazonDelBosque;
    public string tagEnemigos;
    public string tagWaypoints;

    [Header("Layers Globales")]
    public int layerEnemigos;
    public int layerJugador;
    public int layerUI;
    public int layerPath;
    public int layerTerreno;

    //[Header("Tags de enemigos")]
    //public List<string> listaTagEnemigos;

    private int forestHearthAmount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (forestHearthAmount <= 0)
        {
            // TODO: Perder partida
            Debug.Log("Partida perdida");
        }
    }

    public void addForestHearth()
    {
        forestHearthAmount++;
    }

    /*public int getForestHearthAmount()
    {
        return forestHearthAmount;
    }*/
}
