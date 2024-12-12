using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableTower: MonoBehaviour
{
    private bool isACopy = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setIsACopy(bool value)
    {
        isACopy = value;
    }

    public bool getIsACopy()
    {
        return isACopy;
    }
}
