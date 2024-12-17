using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerButton : MonoBehaviour
{
    //public GameObject objeto;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton(Tower tower)
    {
        PlaceManager.Instance.DesignMainTower(tower);
        PlaceManager.Instance.GenerateTower(tower);
    }
}
