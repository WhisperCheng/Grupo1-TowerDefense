using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enredaderas : MonoBehaviour
{
    // Start is called before the first frame update
    private float OriginalSpeed;
    [SerializeField] private float SpeedPercentage;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && !collision.isTrigger)
        {
            NavMeshAgent enemyNavMesh = collision.GetComponent<NavMeshAgent>();
            if (enemyNavMesh != null)
            {
                OriginalSpeed = enemyNavMesh.speed;
                enemyNavMesh.speed = (SpeedPercentage * OriginalSpeed) / 100f; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameManager.Instance.tagEnemigos) && !other.isTrigger)
        {
            NavMeshAgent enemyNavMesh = other.GetComponent<NavMeshAgent>();
            if (enemyNavMesh != null)
            {
                enemyNavMesh.speed = OriginalSpeed;
            }
        }
    }
}
