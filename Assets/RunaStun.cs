using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunaStun : MonoBehaviour
{
    private bool canAttack = true;
    public float cooldown = 2f;
    private float _currentCooldown = 0f;
    private float originalSpeed;
    private NavMeshAgent navmesh;
    private bool isStunned = false;

    void Update()
    {
        // Si está stunneado, decrementa el cooldown
        if (isStunned)
        {
            _currentCooldown -= Time.deltaTime;
            if (_currentCooldown <= 0)
            {
                // Restablece la velocidad y reinicia el estado
                navmesh.speed = originalSpeed;
                isStunned = false;
                Debug.Log("Enemigo destunneado");
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag(GameManager.Instance.tagEnemigos) && canAttack)
        {
            navmesh = collision.GetComponent<NavMeshAgent>();
            if (navmesh != null)
            {
                Debug.Log("Ataque");
                originalSpeed = navmesh.speed;
                navmesh.speed = 0;
                _currentCooldown = cooldown;
                isStunned = true;
                canAttack = false; // Impide múltiples activaciones
                Debug.Log("Enemigo stunneado");
            }
        }
    }
}
