using UnityEngine;

public abstract class EnemigoIA : EntityAI, IEnemy
{
    //Ataque
    [Header("Ataque")]
    public float attackDamage;

    // Variables
    [Header("Variables Enemigo IA")]
    public float speed;
    public float actionRadio;
    public bool showActionRadio;
    public float cooldown;
    protected Vector3 destination;

    bool canAttack;
   
    int enemyMask = 1 << 6;
    private Transform _nearestRival;

    public abstract void WhileWalking();

    protected Transform NearestForestHearthPos(string tag)
    {
        Transform nearestForestHearthPos = null;
        float minorDistance = Mathf.Infinity;

        GameObject[] heartList = GameObject.FindGameObjectsWithTag(tag);

        // Se comprueba y elige al corazón con menor distancia
        if (heartList.Length > 0)
        {
            foreach (GameObject gObj in heartList)
            {
                // Distancia entre el enemigo y el objetivo
                float actualDistance = Vector3.Distance(transform.position, gObj.transform.position);
                if (actualDistance < minorDistance)
                {
                    /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                     * no hay muros por delante*/
                    if (ThereAreNoObstacles(gObj.transform))
                    {
                        minorDistance = actualDistance;
                        nearestForestHearthPos = gObj.transform;
                    }
                }
            }
        }
        return nearestForestHearthPos; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
    }                               // tenerlo en cuenta

    protected Transform NearestRival(Collider[] listaChoques)
    {
        // Esta lista almacenará el resultado de llamar a OverlapSphere
        //Collider[] listaChoques;

        //listaChoques = Physics.OverlapSphere(transform.position, radio, mascara);
        Transform enemigoMasCercano = null;
        float menorDistancia = Mathf.Infinity; // TODO

        // Se comprueba y elige el enemigo con menor distancia
        if (listaChoques.Length > 0)
        {
            foreach (Collider choque in listaChoques)
            {
                float distanciaActual = Vector3.Distance(transform.position, choque.transform.position);
                if (distanciaActual < menorDistancia)
                {
                    /* Se detectan enemigos dentro del radio de acción pero hay que comprobar que
                     * no hay muros por delante*/
                    if (ThereAreNoObstacles(choque.transform))
                    {
                        menorDistancia = distanciaActual;
                        enemigoMasCercano = choque.transform;
                    }
                }
            }
        }
        return enemigoMasCercano; // puede llegar a ser nulo si no hay nada al rededor, hay que                    
        // tenerlo en cuenta
    }

    public void test()
    {
        throw new System.NotImplementedException();
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, actionRadio);
        //Gizmos.DrawRay(transform.position, transform.forward);
    }
#endif
}
