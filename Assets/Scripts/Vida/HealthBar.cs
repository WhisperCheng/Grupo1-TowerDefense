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
        _image = GetComponentsInChildren<Image>().Length > 1 ?
            GetComponentsInChildren<Image>()[GetComponentsInChildren<Image>().Length-1]
            : GetComponentInChildren<Image>(); // En caso de haber una(s) imagen(es) de fondo, toma la otra imagen como la que
                                                // funciona como la barra de vida
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
            //Quaternion lookRotation = Camera.main.transform.rotation;
            //transform.rotation = lookRotation;
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

    public void ResetHealthBar() // Reset instantáneo de la barra de vida
    {
        _targetAmount = 1; // Valor entr 0 y 1
        _image.fillAmount = _targetAmount;
        CheckHealthBarGradient();
        _image.color = _newHealthBarColor;
    }

    // Corutina para ejecutar por separado el while que anima suavemente la transición de cambio del nuevo valor de vida
    private IEnumerator DrainHealthBar()
    {
        Color currentColor = _image.color;
        float fillAmount = _image.fillAmount; // Cantidad de llenado de la barra de vida
        float elapsedTime = 0f; // Tiempo transcurrido, inicialmente a 0
        while ((elapsedTime < _timeToDrain)) // Solo se hace la transición si la barra de vida está activa y
        {                                               // no se ha completado el tiempo de transición
            float howLongInSeconds = (elapsedTime / _timeToDrain); // Tiempo que tarda en hacer la transición
            fillAmount = Mathf.Lerp(fillAmount, _image.fillAmount, howLongInSeconds); // Actualizar a nuevos valores externos (por si varias entidades hacen daño a la vez)
            elapsedTime += Time.deltaTime; // Actualizar el tiempo / se va sumando
            _image.fillAmount = Mathf.Lerp(fillAmount, _targetAmount, howLongInSeconds);
            // Nuevo valor "suavizado" del fillAmount de la imagen
            _image.color = Color.Lerp(currentColor, _newHealthBarColor, howLongInSeconds);
            yield return null; // Esperar al siguiente frame
        }
    }

    // Según el valor de vida actual, se cambia el color actual de la vida
    private void CheckHealthBarGradient()
    {
        _newHealthBarColor = _healthBarGradient.Evaluate(_targetAmount);
    }
}
