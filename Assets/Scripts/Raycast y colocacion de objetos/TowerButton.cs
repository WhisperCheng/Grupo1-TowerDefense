using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    public Tower towerPrefab;
    protected Button thisButton;
    public void StartButton()
    {
        PlaceManager.Instance.DesignMainTower(towerPrefab);
        PlaceManager.Instance.GenerateTower();
    }
    private void Start()
    {
        thisButton = GetComponent<Button>();
    }

    private void Update()
    {
        if (towerPrefab.Money > MoneyManager.Instance.GetMoney())
        {
            thisButton.interactable = false;
        }
        else
        {
            thisButton.interactable = true;
        }
    }
}
