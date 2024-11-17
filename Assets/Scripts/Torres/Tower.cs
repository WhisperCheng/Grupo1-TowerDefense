using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float rango = 10f;
    public float cooldown = 1f;
    public GameObject currentTarget;
    public List<GameObject> currentTargets = new List<GameObject>();
    public PlaceManager placeManager;

    public Transform rotationPart;
    public float rotationModel = 180f;
    public float rotationSpeed = 5f;
    Animator animator;

    private void Start()
    {
        StartCoroutine(Atacar());
        placeManager = FindAnyObjectByType<PlaceManager>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        EnemyDetection();
        if (placeManager.objetoSiendoArrastrado == false)
        {
            LookRotation();
        }
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
            directionToTarget.y = 0; // Mantenemos solo la rotación en el plano XZ
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Obtenemos solo la rotación en el eje Z
            Vector3 currentEuler = rotationPart.rotation.eulerAngles;
            float targetZRotation = targetRotation.eulerAngles.y; // Usamos el valor en Y para rotación en el plano XZ

            // Invertimos el ángulo de rotación en el eje Z
            float smoothZRotation = Mathf.LerpAngle(currentEuler.y, targetZRotation + rotationModel, Time.deltaTime * rotationSpeed);

            rotationPart.rotation = Quaternion.Euler(currentEuler.x, smoothZRotation, currentEuler.z);
        }
    }

    private IEnumerator Atacar()
    {
        while (true)
        {
            if (currentTarget != null && placeManager.objetoSiendoArrastrado == false)
            {
                Shoot();
                animator.SetBool("ataque", true);
            }
            yield return null;
            if (currentTarget == null) 
            {
                animator.SetBool("ataque", false);
            }
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
