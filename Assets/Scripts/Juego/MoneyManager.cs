using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
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
    public int gems = 50; //número de gemas que tenga inicialmente X
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
