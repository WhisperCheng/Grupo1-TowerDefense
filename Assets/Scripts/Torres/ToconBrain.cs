using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ToconBrain : MonoBehaviour
{
    // Variables
    private GameObject _currentTarget;
    private Vector3 _homeTarget;

    private float _spawnCooldown;
    private int _maxNumAliados;
    private int _currentNumAliados;

    private bool _locked = true;
    //private float _rango;

    //private List<GameObject> listaSetas;

    // Properties
    public GameObject ObjetivoActual { get { return _currentTarget; } set { _currentTarget = value; } }
    public Vector3 HomePos { get { return _homeTarget; } private set { _homeTarget = value; } }
    //public float Rango { get; set; }
    public float SpawnCooldown { private get { return _spawnCooldown; } set { _spawnCooldown = value; } }
    public int MaxNumAliados { private get { return _maxNumAliados; } set { _maxNumAliados = value; } }

    private void Start()
    {
        //listaSetas = new List<GameObject>();
        
    }

    public void ActivarSpawn()
    {
        Invoke("SpawnSetas", _spawnCooldown);
    }

    private void Update()
    {
        if (_currentTarget)
        {

        }
    }

    private void SpawnSetas()
    {
        
        if (!_locked && _currentTarget && gameObject.activeSelf && (_currentNumAliados < _maxNumAliados))
        {// Solo se sigue en el bucle hasta que el objeto es desactivado, está fuera de rango o es enviado a la pool y si hay menos aliados ya
         // creados que la cantidad máxima
            GameObject ally = AllyPool.Instance.GetAlly();
            //ally.transform.position = _homeTarget;
            ally.GetComponent<NavMeshAgent>().Warp(_homeTarget);
            ally.GetComponent<Mov_Seta>().SetToconBrain(this);
            _currentNumAliados++;
            //yield return new WaitForSeconds(_spawnCooldown);
            //Debug.Log(_currentNumAliados + " " + _maxNumAliados);
        }
        Invoke("SpawnSetas", _spawnCooldown); // Loop indefinido con X tiempo de espera
    }

    private void AsignarEnemigo()
    {
        /*Collider[] listaChoques;
        listaChoques = Physics.OverlapSphere(transform.position, radio);
        foreach (Collider enemigo in listaChoques)
        {
            if (enemigo.CompareTag("Enemy"))
            {
                animator.SetBool("Caminar", true);
                _navAgent.SetDestination(enemigo.transform.position);
            }
        }*/
    }

    public void QuitarAliado()
    {
        _currentNumAliados--;
    }

    public void ResetValues(Vector3 homePos, float newCooldown, int maxAllies)
    {
        _homeTarget = homePos;
        _spawnCooldown = newCooldown;
        _currentTarget = null;
        _currentNumAliados = 0;
        _maxNumAliados = maxAllies;
        _locked = false;
    }
}
