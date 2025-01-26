using AYellowpaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Localization.Settings;

public class RoundManager : MonoBehaviour
{
    public static int enemiesAlive = 0;

    public Wave[] waves;

    public Transform spawnPoint;

    public float countdownInicial = 3f;

    public bool tutorialMode = false;

    private float countdown = 3f;

    public TextMeshProUGUI waveCountdownText;
    public TextMeshProUGUI newWaveInText;

    private int waveIndex = -1; // Para valor de inicio, el waveindex será -1

    private bool waveInProgress = false;
    private bool finishedLastRoundSpawnings = false;
    private bool finishedCurrentWaveTrigger = false;

    public static RoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        countdown = countdownInicial;
        Debug.Log("Empezando partida en " + countdownInicial + " seg");
    }

    void Update()
    {
        UpdateCountdown();

        if (enemiesAlive > 0)
        {
            return;
        }

        if (finishedLastRoundSpawnings && enemiesAlive == 0)
        {
            Debug.Log("Fin");
            //gameManager.WinLevel(); // TODO
            this.enabled = false; // <-- Si llega aquí todo lo siguiente no se ejecutará y la partida habrá terminado
        }

        //Debug
        if (waveIndex >= 0 && waveIndex < waves.Length - 1 && enemiesAlive == 0 && finishedCurrentWaveTrigger)
        {
            if (waveInProgress) waveInProgress = false; // Al terminar la oleada, se pone started a false para iniciar el proceso del countdown
            Debug.Log("Iniciando siguiente oleada en " + countdown + " seg");
            finishedCurrentWaveTrigger = false; // Se vuelve false para no volver a entrar en la condición hasta
        }                                                           //  la siguiente vez que se vuelva a terminar la oleada

        if (countdown <= 0f && enemiesAlive == 0 /*&& !HasSpawningEnded()*/ && waveIndex < waves.Length - 1)
        {
            InitializeNewWave();
            return;
        }
    }

    private void UpdateCountdown()
    {
        if (enemiesAlive == 0 && !waveInProgress) // Si se han muerto todos los enemigos de la oleada y no hay rondas activas
        {
            var lang = LocalizationSettings.SelectedLocale;
            if (!float.IsInfinity(countdown)) // Si el tiempo de espera no es infinito se resta tiempo
            {
                // Se empieza a correr el contador hasta llegar a 0
                countdown -= Time.deltaTime;

                countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

                newWaveInText.gameObject.SetActive(true);
                
                string comingWaveText = LocalizationSettings.StringDatabase.GetLocalizedString("Localization Table",
                "_NuevaOleada", lang);
                newWaveInText.text = comingWaveText;
                waveCountdownText.text = string.Format("{0:00.00}", countdown) + " s";
                waveCountdownText.fontSize = 36;
            } // Si es infinito entonces al presionar la G se llamará al evento , que automáticamente reseteará el contador
            else                                                                                    // a 0 e iniciará la ronda
            { // Se cambia el texto para indicar que hay que presionar la tecla G para continuar 
                string waitingForGText = LocalizationSettings.StringDatabase.GetLocalizedString("Localization Table",
                "_PresionarGNuevaOleada", lang);
                waveCountdownText.text = waitingForGText;
                waveCountdownText.fontSize = 20;
            }
        }
        else
        {
            newWaveInText.gameObject.SetActive(false);
            waveCountdownText.fontSize = 28;
            var lang = LocalizationSettings.SelectedLocale;
            string activeWaveText =
            LocalizationSettings.StringDatabase.GetLocalizedString("Localization Table", "_RondaActiva", lang);
            waveCountdownText.text = activeWaveText;

        }
    }

    /*private void ManageNormalRounds()
    {

    }*/ // TODO

    private void InitializeNewWave()
    {
        waveIndex++;
        Debug.Log("Empezando oleada nº " + (waveIndex + 1));
        StartWave();
        waveInProgress = true;
    }

    public void StartRoundAfterInfiniteRest(InputAction.CallbackContext ctx)
    {
        if (!waveInProgress && enemiesAlive == 0 && ctx.performed && float.IsInfinity(countdown))
        {
            countdown = 0.25f; // 0.5 segundos de espera para no comenzar la ronda muy de golpe
        }
    }

    public void StartWave()
    {
        Wave wave = waves[waveIndex];

        // Iterar a través de los enemigos de cada ronda para iniciar el spawn de cada uno
        // de ellos, dada la información de spawn que tiene cada uno
        foreach (var item in wave.enemies)
        {
            StartCoroutine(SpawnEnemy(item));
        }
        countdown = waves[waveIndex].restTimeUnitNextWave; // Reset del countdown para cuando terminen de morir todos los enemigos
        Debug.Log(countdown);
    }
    IEnumerator SpawnEnemy(KeyValuePair<InterfaceReference<IPoolable, EnemyAI>, WaveUnitInfo> enemyInfo)
    {
        WaveUnitInfo info = enemyInfo.Value;
        int totalEnemiesAmount = info.countPerRound;
        for (int i = 0; i < totalEnemiesAmount; i++)
        {
            if (i == 0 && info.initialWait > 0) // Si la unidad tiene un tiempo de espera inicial, se espera para luego empezar con el
                yield return new WaitForSeconds(info.initialWait);                                                  // proceso de spawn

            int current = i + 1;

            //Sacar enemigos de la pool
            GameObject enemy = enemyInfo.Key.Value.GetFromPool(); // La InterfaceReference alberga la referencia a un GameObject
            enemy.GetComponent<NavMeshAgent>().Warp(spawnPoint.position);                   // que implementa la interfaz IPoolable

            enemiesAlive++;
            if (info.ActualSpawnRate == 0) // Cuando empiece desde el principio, al actualSpawnRate se
                info.ActualSpawnRate = info.spawnRate;              // le asigna el valor inicial correspondiente
            if (i == totalEnemiesAmount - 1)
            {
                info.FinishedSpawning = true; // Si se llega al final del bucle, se marca como que el spawn de la unidad ha terminado
                CheckIfHasSpawningEnded(); // Comprobar si todas las corutinas de spawn han terminado, en caso positivo se actualiza
                                           // el bool finished a true
            }
            else
            {
                yield return new WaitForSeconds(info.ActualSpawnRate);

                //info.ActualSpawnRate = info.spawnRate * (info.spawnRateMultiplierAtEnd * ( current / (float) totalEnemiesAmount));
                info.ActualSpawnRate = info.ActualSpawnRate * info.spawnRateMultiplier;
            }
            // Luego se le asigna el nuevo valor del actualSpawnRate, que se corresponde con una operación progresiva de acorde al
            // valor del spawnRateMultiplierAtEnd y el índice de la unidad actual a generar.
            // Al llegar al final el actualSpawnRate será igual a = spawnRate * spawnRateMultiplierAtEnd
        }
    }

    // Cada vez que termina una corutina de generar enemigos, se comprueba si las demás han terminado utilizando su WaveUnitInfo.FinishedSpawning
    // Cuando la última corutina termine, esta pondre el boleano finished a true, lo cual dará fin a la partida
    private void CheckIfHasSpawningEnded()
    {
        bool result = true;
        foreach (var enemyDictionary in waves[waveIndex].enemies)
        {
            if (!enemyDictionary.Value.FinishedSpawning)
                result = false;
        }
        finishedCurrentWaveTrigger = result; // Se cambia el trigger de haber terminado la oleada para su posterior uso
        if (result && waveIndex == waves.Length - 1)          //  No se cambiará hasta haber destruido a todos los enemigos
            finishedLastRoundSpawnings = true;
    }
}
