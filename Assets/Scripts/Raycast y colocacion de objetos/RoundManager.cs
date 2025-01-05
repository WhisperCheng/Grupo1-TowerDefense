using AYellowpaper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoundManager : MonoBehaviour
{
	public static int enemiesAlive = 0;

	public Wave[] waves;

	public Transform spawnPoint;

	public float countdownInicial = 3f;

	private float countdown = 2f;

	//public Text waveCountdownText;

	//public GameManager gameManager;

	private int waveIndex = -1; // Para valor de inicio, el waveindex será -1

	private bool started = false;
	private bool finishedAllSpawnings = false;

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
		Debug.Log("Empezando partida en " + countdownInicial + " seg");
	}

    void Update()
	{
		if (enemiesAlive > 0)
		{
			return;
		}

		if (finishedAllSpawnings && enemiesAlive == 0)
		{
			Debug.Log("Fin");
			//gameManager.WinLevel();
			this.enabled = false; // <-- Si llega aquí todo lo siguiente no se ejecutará y la partida habrá terminado
		}

        if (!started && waveIndex == 0) // Si se inicia por primera vez, se asigna el countdown inicial
        {
			countdown = countdownInicial;
			started = true;
		}

		//Debug
		if(waveIndex >= 0 && waveIndex < waves.Length-1 && enemiesAlive == 0 && countdown == waves[waveIndex].restTimeUnitNextWave)
        {
			Debug.Log("Iniciando siguiente oleada en " + waves[waveIndex].restTimeUnitNextWave + " seg");
        }

		if (countdown <= 0f && enemiesAlive == 0 /*&& !HasSpawningEnded()*/ && waveIndex < waves.Length-1)
		{
			waveIndex++;
			Debug.Log("Empezando oleada nº " + (waveIndex+1));
			StartWave();
			//countdown = timeBetweenWaves;
			return;
		}

		countdown -= Time.deltaTime;

		countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

		//waveCountdownText.text = string.Format("{0:00.00}", countdown);
	}

	public void StartWave()
	{
		//PlayerStats.Rounds++;

		Wave wave = waves[waveIndex];

		// Iterar a través de los enemigos de cada ronda para iniciar el spawn de cada uno
		// de ellos, dada la información de spawn que tiene cada uno
		foreach (var item in wave.enemies)
		{
			StartCoroutine(SpawnEnemy(item));
		}
		countdown = waves[waveIndex].restTimeUnitNextWave; // Reset del countdown para cuando terminen de morir todos los enemigos
	}
	IEnumerator SpawnEnemy(KeyValuePair<InterfaceReference<IPoolable, EnemyAI>, WaveUnitInfo> enemyInfo) {
		WaveUnitInfo info = enemyInfo.Value;
		int totalEnemiesAmount = info.countPerRound;
		for (int i = 0; i < totalEnemiesAmount; i++)
        {
			if(i == 0 && info.initialWait > 0) // Si la unidad tiene un tiempo de espera inicial, se espera para luego empezar con el
				yield return new WaitForSeconds(info.initialWait);													// proceso de spawn

			int current = i + 1;

			//Sacar enemigos de la pool
			GameObject enemy = enemyInfo.Key.Value.GetFromPool(); // La InterfaceReference alberga la referencia a un GameObject
			enemy.GetComponent<NavMeshAgent>().Warp(spawnPoint.position);					// que implementa la interfaz IPoolable

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
		if (result && waveIndex == waves.Length-1)
			finishedAllSpawnings = true;
	}

}
