using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public bool infiniteMoney = false;
    public GameObject moneyContainer;
    public int gems = 50; //número de gemas que tenga inicialmente X
    public static MoneyManager Instance { get; private set; }
    private void Awake()
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
    
    // Start is called before the first frame update
    void Start()
    {
        if (infiniteMoney) moneyContainer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(infiniteMoney && gems < 9999)
        {
            gems = 9999;
        }
    }
    public void RemoveMoney(int money)
    {
        gems -= money;
    }
    public void AddMoney (int money)
    {
        gems += money;
    }
    public int GetMoney()
    {
        return gems;
    }
}
