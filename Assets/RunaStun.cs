using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunaStun : MonoBehaviour
{
    private bool canAttack = true;
    [SerializeField] private float duration = 3f;
    private float _currentDuration = 0f;
    [SerializeField] private float cooldown = 2f;
    private float originalSpeed;
    private NavMeshAgent navmesh;
    private bool isStunned = false;

    void Update()
    {
        if (isStunned)
        {
            _currentDuration -= Time.deltaTime;
            if (_currentDuration <= 0)
            {
                navmesh.speed = originalSpeed;
                isStunned = false;
                StartCoroutine(wait());
                Debug.Log("Enemigo destunneado");
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && canAttack)
        {
            navmesh = collision.GetComponent<NavMeshAgent>();
            if (navmesh != null)
            {
                Debug.Log("Ataque");
                originalSpeed = navmesh.speed;
                navmesh.speed = 0;
                _currentDuration = duration;
                isStunned = true;
                canAttack = false; 
                Debug.Log("Enemigo stunneado");
            }
        }
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(cooldown);
        canAttack = true;
    }
}
