using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    public float health;

    [Header("Barra de vida y Jugador")]
    public HealthBar _healthBar;
    public GameObject playerModel;

    [Header("Cámara")]
    public CinemachineFreeLook virtualCamera;

    [Header("Partículas")]
    public ParticleSystem respawnParticles;
    public ParticleSystem deathParticles;
    public ParticleSystem hitParticles;
    public GameObject particlesParent;
   
    private float _currentHealth;
    private float _maxHealth;

    private TimerMuerte timer;

    public void Die()
    {
        //throw new System.NotImplementedException();
        timer.ActivateRespawnTimer(); // Desaparecer temporalmente + mostrar UI espera de respawn
        ParticleSystem particulas = InstantiateParticlesOnPlayer(deathParticles);
        particulas.transform.position += Vector3.up * 0.1f; // Subir partículas ligeramente del suelo
    }

    public float GetHealth() { return _currentHealth; }
    public float GetMaxHealth() { return _maxHealth; }

    public void TakeDamage(float damageAmount)
    {
        // Daño
        _currentHealth -= damageAmount;
        //OnDamageTaken();
        ParticleSystem hit = InstantiateParticlesOnPlayer(hitParticles);
        hit.transform.position += Vector3.up * 1.1f;
        // Actualizar barra de vida
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth);
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    public void ReSpawn(Transform destination)
    {
        var delta = destination.position - gameObject.transform.position;

        gameObject.transform.position = destination.transform.position;
        //virtualCamera.PreviousStateIsValid = false;
        virtualCamera.OnTargetObjectWarped(transform, delta);
        _healthBar.UpdateHealthBar(_maxHealth, _currentHealth = _maxHealth);
        ParticleSystem particulas = InstantiateParticlesOnPlayer(respawnParticles);
        // Calcular el centro del jugador y cambiar la posición de las partículas a ese centro
        particulas.transform.position = PlaceManager.Instance.GetGameObjectCenter(gameObject);
    }
    private ParticleSystem InstantiateParticlesOnPlayer(ParticleSystem pSys)
    {
        ParticleSystem pSysAction =
       PlaceManager.Instance.StartParticleGameObjEffect(pSys, gameObject.transform.position);
        pSysAction.gameObject.transform.parent = particlesParent.transform; // Asignando padre
       
        return pSysAction;
    }

    // Start is called before the first frame update
    void Start()
    {
        //_healthBar = GetComponentInChildren<HealthBar>();
        _currentHealth = health;
        _maxHealth = health;
        timer = gameObject.GetComponent<TimerMuerte>();
        timer.Player = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
