using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public void StartButton(Tower tower)
    {
        PlaceManager.Instance.DesignMainTower(tower);
        PlaceManager.Instance.GenerateTower();
    }
}
