using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    private int _maxIndexSize = 8;
    private int _currentIndex = 0;

    public Slot[] slots;
    public float buttonsTransitionTime;
    public float buttonsOnSelectedScale;
    public LeanTweenType animationType;
    // Start is called before the first frame update
    private void Start()
    {
        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;
        
        foreach (Slot slot in slots) // Tiempo de transición de animaciones al cambiar de botón
        {
            slot.TransitionTime = buttonsTransitionTime;
            slot.OnSelectedScale = buttonsOnSelectedScale;
            slot.AnimationType = animationType;
        }
        slots[_currentIndex].ToggleHighlight();
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (Slot slot in slots) // Tiempo de transición de animaciones al cambiar de botón
        {
            slot.TransitionTime = buttonsTransitionTime;
            slot.OnSelectedScale = buttonsOnSelectedScale;
            slot.AnimationType = animationType;
        }

        float inputScrollValue = GameManager.Instance.playerControls.actions["ScrollWheel"].ReadValue<Vector2>().y;
        if (inputScrollValue > 0.1f) ChangeIndex(Direction.Left);
        if (inputScrollValue < -0.1f) ChangeIndex(Direction.Right);
    }

    private void ChangeIndex(Direction direction)
    {
        slots[_currentIndex].ToggleHighlight(); // Se deselecciona el botón actualmente seleccionado

        _currentIndex += (int)direction;
        if (_currentIndex > _maxIndexSize) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxIndexSize;

        slots[_currentIndex].ToggleHighlight(); // Se selecciona el nuevo botón

        if (!GameUIManager.Instance.activeBuildUI) // si el menú está escondido, mostrarlo
        {
            GameUIManager.Instance.ShowBuildUI(GameUIManager.Instance.menusTransitionTime);
        }
    }

    /*protected void OnEnable()
    {
        //GameManager.Instance.playerControls.actions["ScrollWheel"].performed += OnChangedHotbarButton;
    }*/

    private void OnChangedHotbarButton(InputAction.CallbackContext ctx)
    {
        PlaceManager.Instance.OnClickButtons(ctx);
    }

    private void SetIndex(int newIndex) // TODO: Click derecho + callback para cada numero del teclado
    {
        slots[_currentIndex].ToggleHighlight(); // Se deselecciona el botón actualmente seleccionado
        if (newIndex < 0) newIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        _currentIndex = newIndex;

        slots[_currentIndex].ToggleHighlight(); // Se selecciona el nuevo botón
    }

    public void SelectCurrentButton()
    {
        slots[_currentIndex].Button.onClick.Invoke();
    }

    public void SelectNextTower()
    {
        PlaceManager.Instance.DiscardCurrentTower();
        PlaceManager.Instance.OnClickButton(slots[_currentIndex].Button);
        slots[_currentIndex].Button.onClick.Invoke();
        //PlaceManager.Instance.PreviewNewTower();
    }

    //private enum HotBarDirection { Up, Down, Left, Right }; // Solo se va a usar lefy y right
    private enum Direction
    {
        Left = -1,
        Right = 1
    };

}
