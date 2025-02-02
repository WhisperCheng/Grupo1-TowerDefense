using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Slot : MonoBehaviour
{
    public Color colorOnSelectedButton;
    private Color _normalButtonColor;
    private Button _currentButton;
    private Image _buttonImage;
    // Start is called before the first frame update

    private void Awake()
    {
        _currentButton = GetComponent<Button>();
        _normalButtonColor = _currentButton.colors.normalColor;
        _buttonImage = GetComponent<Image>();
    }
    void Start()
    {
        
    }

    public void ToggleHighlight()
    {
        Debug.Log(gameObject);
        GetComponent<Image>().color = _currentButton.colors.selectedColor;
    }
}
