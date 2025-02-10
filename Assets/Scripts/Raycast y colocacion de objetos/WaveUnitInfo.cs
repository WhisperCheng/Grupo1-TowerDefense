using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveUnitInfo
{
    public int initialWait;
    public int countPerRound;
    public float spawnRate;
    [Range(0.1f, 5f)]
    public float spawnRateMultiplier = 1f;

    public float ActualSpawnRate { get; set; }
    public bool FinishedSpawning { get; set; } = false;
    

}
