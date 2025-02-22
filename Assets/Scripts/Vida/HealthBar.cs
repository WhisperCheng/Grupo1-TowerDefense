using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float _timeToDrain = 0.25f;
    [SerializeField] private Gradient _healthBarGradient;
    [SerializeField] private bool _billboard;
    private Image _image;
    private Camera _camera;

    private Coroutine drainHealthCoroutine;
    private float _targetAmount = 1f;
    private Color _newHealthBarColor;
    // Start is called before the first frame update
    void Start()
    {
        Image[] arrayImages = GetComponentsInChildren<Image>();
        _image = arrayImages.Length > 1 ? arrayImages[arrayImages.Length-1 -1]: GetComponentInChildren<Image>();
        // En caso de haber una(s) imagen(es) de fondo, toma la otra imagen como la que funciona como la barra de vida
        _image.color = _healthBarGradient.Evaluate(_targetAmount);
        _camera = Camera.main;
        CheckHealthBarGradient();
    }

    // Update is called once per frame
    void Update()
    {
        if (_billboard)
        {
            _camera = Camera.main;
            transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        }
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        _targetAmount = currentHealth / maxHealth; // Valor entr 0 y 1
        if (gameObject.activeInHierarchy)
        {
            
            drainHealthCoroutine = StartCoroutine(DrainHealthBar()); // Iniciar corutina con el suavizado de la vida
           
        }
        CheckHealthBarGradient();
    }

    public void ResetHealthBar() // Reset instant�neo de la barra de vida
    {
        _targetAmount = 1; // Valor entr 0 y 1
        _image.fillAmount = _targetAmount;
        CheckHealthBarGradient();
        _image.color = _newHealthBarColor;
    }

    // Corutina para ejecutar por separado el while que anima suavemente la transici�n de cambio del nuevo valor de vida
    private IEnumerator DrainHealthBar()
    {
        Color currentColor = _image.color;
        float fillAmount = _image.fillAmount; // Cantidad de llenado de la barra de vida
        float elapsedTime = 0f; // Tiempo transcurrido, inicialmente a 0
        while ((elapsedTime < _timeToDrain)) // Solo se hace la transici�n si la barra de vida est� activa y
        {                                               // no se ha completado el tiempo de transici�n
            float howLongInSeconds = (elapsedTime / _timeToDrain); // Tiempo que tarda en hacer la transici�n
            fillAmount = Mathf.Lerp(fillAmount, _image.fillAmount, howLongInSeconds); // Actualizar a nuevos valores externos (por si varias entidades hacen da�o a la vez)
            elapsedTime += Time.deltaTime; // Actualizar el tiempo / se va sumando
            _image.fillAmount = Mathf.Lerp(fillAmount, _targetAmount, howLongInSeconds);
            // Nuevo valor "suavizado" del fillAmount de la imagen
            _image.color = Color.Lerp(currentColor, _newHealthBarColor, howLongInSeconds);
            yield return null; // Esperar al siguiente frame
        }
    }

    // Seg�n el valor de vida actual, se cambia el color actual de la vida
    private void CheckHealthBarGradient()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_targetAmount);
    }
}
