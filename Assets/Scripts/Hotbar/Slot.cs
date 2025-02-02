using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Slot : MonoBehaviour
{
    private Button currentButton;
    // Start is called before the first frame update
    void Start()
    {
        currentButton = GetComponent<Button>();
    }

    public void ToggleHighlight()
    {

    }
}
