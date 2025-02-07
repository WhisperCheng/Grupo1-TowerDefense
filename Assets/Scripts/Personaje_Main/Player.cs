using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    public float health;

    [Header("Cooldown")]
    public float cooldown;
    private float _currentCooldown;

    [Header("Hotbar a controlar")]
    public HotbarController hotbar;

    [Header("Barra de vida y Jugador")]
    public HealthBar _healthBar;
    public GameObject playerModel;
    public GameObject shootingSource;

    [Header("Cámara")]
    public CinemachineFreeLook virtualCamera;

    [Header("Partículas")]
    public ParticleSystem respawnParticles;
    public ParticleSystem deathParticles;
    public ParticleSystem hitParticles;
    //public ParticleSystem shootProyectileParticles;
    public GameObject particlesParent;

    private ProyectilFabric _proyectilFabric;
    private bool _canShoot = false;
    private bool _isShowingCrosier = false;
    private bool _isHoveringOverAButton = false;

    private float _currentHealth;
    private float _maxHealth;

    public Animator animator;

    private TimerMuerte _timer;
    private CharacterController _characterController;
    private NavMeshObstacle _navMeshObstacle;

    // Start is called before the first frame update
    void Start()
    {
        //_healthBar = GetComponentInChildren<HealthBar>();
        _currentHealth = health;
        _maxHealth = health;
        _proyectilFabric = GetComponent<ProyectilFabric>();
        _timer = gameObject.GetComponent<TimerMuerte>();
        _characterController = gameObject.GetComponent<CharacterController>();
        _navMeshObstacle = gameObject.GetComponent<NavMeshObstacle>();
        _timer.Player = this;
    }

    // Update is called once per frame
    void Update()
    {
        ManageCooldown();
        animator.SetBool("ataque", _isShowingCrosier);
    }

    public void Die()
    {
        //throw new System.NotImplementedException();
        _timer.ActivateRespawnTimer(); // Desaparecer temporalmente + mostrar UI espera de respawn
        ParticleSystem particulas = InstantiateParticlesOnPlayer(deathParticles);
        particulas.transform.position += Vector3.up * 0.1f; // Subir partículas ligeramente del suelo
        PlaceManager.Instance.DiscardCurrentTower(); // Cancelar la colocación de la torre actual, si es que
                                                     // se estaba colocando algo
        hotbar.DeselectCurrentButton(); // Deseleccionar botón actual
        hotbar.DisableHotbar(); // Deshabilitar hotbar
        if (_navMeshObstacle) _navMeshObstacle.enabled = false; // Desactivar el hueco del jugador para que los 
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
        hotbar.EnableHotbar(); // Habilitar hotbar
        if (_navMeshObstacle) _navMeshObstacle.enabled = true; // Reactivar el hueco del jugador para simular un espacio
    }
    private ParticleSystem InstantiateParticlesOnPlayer(ParticleSystem pSys)
    {
        ParticleSystem pSysAction =
       PlaceManager.Instance.StartParticleGameObjEffect(pSys, gameObject.transform.position, false);
        pSysAction.gameObject.transform.parent = particlesParent.transform; // Asignando padre

        return pSysAction;
    }
    public void TryShootProyectile(InputAction.CallbackContext ctx)
    {
        if (Time.deltaTime != 0 && _currentHealth > 0) // Si el juego no está pausado y el jugador está vivo
        {
            if (!UIUtils.IsPointerOverInteractableUIElement() && _isShowingCrosier && _canShoot && !_isHoveringOverAButton
            && ctx.started && !PlaceManager.Instance.bloqueoDisparo)
            { // Si tiene el bastón alzado y no hay cooldown y no se está haciendo click encima de un botón, se dispara
                _proyectilFabric.LanzarProyectil();
                _canShoot = false;
                GameObject particles = ShootingMagicParticlesPool.Instance.GetMagicShootingParticle();
                particles.transform.position = shootingSource.transform.position;
                particles.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    public void ShowCrosier()
    {
        _isShowingCrosier = true;
        _currentCooldown = cooldown;
    }

    public void HideCrosier()
    {
        _isShowingCrosier = false;
    }
    public bool CheckIfIsShowingCrosier()
    {
        return _isShowingCrosier;
    }

    private void ManageCooldown()
    {
        if (!_canShoot)
        {
            _currentCooldown -= Time.deltaTime;

            if (_currentCooldown <= 0)
            {
                _canShoot = true;
                _currentCooldown = cooldown;
            }
        }
    }
}
