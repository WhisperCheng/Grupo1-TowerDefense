using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemigosTest : MonoBehaviour
{
    public float spawnInterval = 2.0f; // Intervalo en segundos para instanciar los mini caballeros
    

    private void Start()
    {
        StartCoroutine(SpawnMiniKnights());
    }

    private IEnumerator SpawnMiniKnights()
    {
        while (true)
        {
            GameObject miniKnight = MiniKnightPool.Instance.GetMiniKnight();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
