using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    protected int _maxIndexSize = 8;
    protected int _currentIndex = 0;

    public Slot[] slots;
    public float buttonsTransitionTime;
    public float buttonsOnSelectedScale;
    public LeanTweenType animationType;
    // Start is called before the first frame update
    protected void Start()
    {
        _currentIndex = 0;
        _maxIndexSize = slots.Length - 1;
        ConfigureSlots();
        EnableHotbar();
        slots[_currentIndex].ToggleHighlight();
        slots[_currentIndex].SelectButton();
    }

    protected void ConfigureSlots()
    {
        foreach (Slot slot in slots) // Tiempo de transición de animaciones al cambiar de botón
        {
            slot.TransitionTime = buttonsTransitionTime;
            slot.OnSelectedScale = buttonsOnSelectedScale;
            slot.AnimationType = animationType;
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        ConfigureSlots();

        float inputScrollValue = GameManager.Instance.playerControls.actions["ScrollWheel"].ReadValue<Vector2>().y;
        if (inputScrollValue > 0.1f) ChangeIndex(Direction.Left);
        if (inputScrollValue < -0.1f) ChangeIndex(Direction.Right);
    }

    protected void ChangeIndex(Direction direction)
    {
        int relativeIndex = _currentIndex;
        for (int i = 0; i <= _maxIndexSize; i++)
        {
            relativeIndex += (int)direction; // Avance del índice relativo
            // Ajustando a los límites disponibles el índice relativo
            if (relativeIndex > _maxIndexSize) relativeIndex = 0;
            if (relativeIndex < 0) relativeIndex = _maxIndexSize;

            // Si se puede interactuar con el botón, se procede a seleccionarlo
            if (slots[relativeIndex].Button.interactable) 
            {
                
                // Se deselecciona el botón actualmente seleccionado
                if (slots[_currentIndex].Selected) slots[_currentIndex].ToggleHighlight();
                _currentIndex = relativeIndex;
                slots[_currentIndex].ToggleHighlight();// Se selecciona el nuevo botón
                break; // Se para de mirar los siguientes botones ya que ya se ha seleccionado uno nuevo
            }
        }

        if (!GameUIManager.Instance.activeBuildUI) // si el menú está escondido, mostrarlo
        {
            GameUIManager.Instance.ShowBuildUI(GameUIManager.Instance.menusTransitionTime);
        }
    }

    protected void SetIndex(int newIndex)
    {
        if (newIndex < 0) newIndex = 0;
        if (newIndex > _maxIndexSize) newIndex = _maxIndexSize;

        int desiredIndex = newIndex;

        if (slots[desiredIndex].Button.interactable) // Si se puede interactuar con ese botón
        {
            // Se deselecciona el botón actualmente seleccionado
            if (slots[_currentIndex].Selected) slots[_currentIndex].ToggleHighlight();
            _currentIndex = newIndex;
            slots[_currentIndex].ToggleHighlight();// Se selecciona el nuevo botón
        }
        // Si no no se puede hacer nada
    }

    protected void EnableHotbar()
    {
        GameManager.Instance.playerControls.actions["Button1"].performed += Hotbar1;
        GameManager.Instance.playerControls.actions["Button2"].performed += Hotbar2;
        GameManager.Instance.playerControls.actions["Button3"].performed += Hotbar3;
        GameManager.Instance.playerControls.actions["Button4"].performed += Hotbar4;
        GameManager.Instance.playerControls.actions["Button5"].performed += Hotbar5;
        GameManager.Instance.playerControls.actions["Button6"].performed += Hotbar6;
        GameManager.Instance.playerControls.actions["Button7"].performed += Hotbar7;
        GameManager.Instance.playerControls.actions["Button8"].performed += Hotbar8;
        GameManager.Instance.playerControls.actions["Button9"].performed += Hotbar9;
        GameManager.Instance.playerControls.actions["Click"].performed += OnClick;
        GameManager.Instance.playerControls.actions["RightClick"].performed += OnRightClick;
    }

    protected void Hotbar1(InputAction.CallbackContext ctx) { SetIndex(0); }
    protected void Hotbar2(InputAction.CallbackContext ctx) { SetIndex(1); }
    protected void Hotbar3(InputAction.CallbackContext ctx) { SetIndex(2); }
    protected void Hotbar4(InputAction.CallbackContext ctx) { SetIndex(3); }
    protected void Hotbar5(InputAction.CallbackContext ctx) { SetIndex(4); }
    protected void Hotbar6(InputAction.CallbackContext ctx) { SetIndex(5); }
    protected void Hotbar7(InputAction.CallbackContext ctx) { SetIndex(6); }
    protected void Hotbar8(InputAction.CallbackContext ctx) { SetIndex(7); }
    protected void Hotbar9(InputAction.CallbackContext ctx) { SetIndex(8); }

    protected void OnClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlaceManager.Instance.OnClickPlaceTower(ctx); // Colocar torre
            if(_currentIndex != 0) // De hacer click con el primer botón se encarga otro evento
            slots[_currentIndex].Button.onClick.Invoke();
            // Si se está colocando una torre, volver a mostrar otra en caso de querer colocar más
            // Si se está con el bastón, disparar
        }
    }

    protected void OnRightClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (slots[_currentIndex].Selected)
            { // Se deselecciona el botón actualmente seleccionado
                slots[_currentIndex].ToggleHighlight();
                
            }
            PlaceManager.Instance.OnRightClickPlaceTower(ctx); // Cancelar colocación de la torre
        }
    }

    public void SelectCurrentButton()
    {
        PlaceManager.Instance.DiscardCurrentTower();
        PlaceManager.Instance.OnTriggerButton(slots[_currentIndex].Button);
        PlaceManager.Instance.InvokeCurrentButton();
    }

    public void SelectCurrentTowerButton()
    {
        PlaceManager.Instance.DiscardCurrentTower();
        PlaceManager.Instance.OnTriggerButton(slots[_currentIndex].Button);
        PlaceManager.Instance.InvokeCurrentButton();
    }

    //private enum HotBarDirection { Up, Down, Left, Right }; // Solo se va a usar lefy y right
    protected enum Direction
    {
        Left = -1,
        Right = 1
    };

}
