using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float rango = 10f;
    public float cooldown = 1f;
    public GameObject currentTarget;
    public List<GameObject> currentTargets = new List<GameObject>();

    public Transform rotationPart;
    public float rotationSpeed = 5f;

    private void Start()
    {
        StartCoroutine(Atacar());
    }

    private void Update()
    {
        EnemyDetection();
        LookRotation();
    }

    private void EnemyDetection()
    {
        currentTargets.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, rango);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                currentTargets.Add(collider.gameObject);
            }
        }

        if (currentTargets.Count > 0 && currentTarget == null)
        {
            currentTarget = currentTargets[0];
        }

        if (currentTarget != null && !currentTargets.Contains(currentTarget))
        {
            currentTarget = null;
        }
    }

    private void LookRotation()
    {
        if (currentTarget != null)
        {
            Vector3 directionToTarget = currentTarget.transform.position - rotationPart.position;
            directionToTarget.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            rotationPart.rotation = Quaternion.Lerp(rotationPart.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private IEnumerator Atacar()
    {
        while (true)
        {
            if (currentTarget != null)
            {
                Shoot();
                yield return new WaitForSeconds(cooldown);
            }
            yield return null;
        }
    }

    private void Shoot()
    {
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rango);
    }
}
