using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    private int _maxIndexSize = 8;
    private int _currentIndex = 0;

    public Slot[] slots;
    // Start is called before the first frame update
    private void Start()
    {
        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;

        slots[_currentIndex].ToggleHighlight();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
