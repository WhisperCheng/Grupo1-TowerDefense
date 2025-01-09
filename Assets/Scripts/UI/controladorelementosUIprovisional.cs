using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class controladorelementosUIprovisional : MonoBehaviour
{
    // Referencia al TextMeshPro para mostrar el cronómetro
    [SerializeField] private TMP_Text timerText;

    // Tiempo inicial en segundos (2 minutos)
    private float remainingTime = 120f;

    // Bandera para controlar si el cronómetro está activo
    public bool isTimerRunning = false;

    private void Start()
    {
        // Inicia el cronómetro
        StartTimer();
    }

    private void Update()
    {
        // Solo actualiza si el cronómetro está corriendo
        if (isTimerRunning)
        {
            UpdateTimer();
        }
    }

    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateTimer()
    {
        // Reduce el tiempo restante
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            // Calcula minutos y segundos
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);

            // Actualiza el texto del cronómetro
            timerText.text = $"{minutes:0}:{seconds:00}";
        }
        else
        {
            // Detén el cronómetro cuando llegue a 0
            isTimerRunning = false;
            remainingTime = 0;

            // Asegúrate de que el texto muestre 0:00
            timerText.text = "0:00";

            // Aquí puedes agregar cualquier lógica para manejar cuando el cronómetro llega a 0
            OnTimerEnd();
        }
    }

    private void OnTimerEnd()
    {
        Debug.Log("¡El tiempo ha terminado!");
        // Agrega aquí cualquier lógica que desees cuando el cronómetro termine.
    }
}
