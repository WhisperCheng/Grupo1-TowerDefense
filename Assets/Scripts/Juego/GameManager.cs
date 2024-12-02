using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int forestHearthAmount = 0;
    public static GameManager Instance { get; private set; }

    public Transform[] wayPoints;
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

    [Header("Tags Globales")]
    public string tagCorazonDelBosque;
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
