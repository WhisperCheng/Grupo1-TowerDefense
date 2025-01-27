using AYellowpaper;
using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
	[SerializedDictionary("Enemigo (Prefab)","Info de la unidad en la oleada")]
	public SerializedDictionary<InterfaceReference<IPoolable, EnemyAI>, WaveUnitInfo> enemies;
	public float restTimeUntilNextWave;
}
