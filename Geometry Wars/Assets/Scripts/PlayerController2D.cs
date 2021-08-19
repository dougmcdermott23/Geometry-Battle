using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Tools.Utils;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public event EventHandler OnPlayerTookDamage;

    [SerializeField] private ObjectFactory projectileFactory;
    [SerializeField] private Transform projectileStorage;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelerationTime;
    [SerializeField] private float timeBetweenShots;

    private PlayerControls playerControls;

    private GameController.GameBoundaries boundaries;
    private ParticleSystem particles;
    private Vector3 velocity;
    private Vector2 moveDirection;
    private float velocityXSmoothing;
    private float velocityYSmoothing;
    private float shootTimer;

    private void Awake()
    {
        projectileFactory.Init();
        projectileStorage = GameObject.Find("Projectile Storage").transform;
        
        playerControls = new PlayerControls();
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed && shootTimer <= 0)
        {
            FireProjectile();
            shootTimer = timeBetweenShots;
        }
        else
        {
            shootTimer -= Time.deltaTime;
        }

        CheckBoundaries();
        Move();
        Rotate();
    }

    private void CheckBoundaries()
    {
        if (transform.position.x < boundaries.BotLeft.x ||
            transform.position.x > boundaries.BotRight.x ||
            transform.position.y < boundaries.BotLeft.y ||
            transform.position.y > boundaries.TopLeft.y)
        {
            DamagePlayer();
        }
    }

    private void Move()
    {
        Vector2 moveInput = playerControls.Player.Movement.ReadValue<Vector2>().normalized;
        Vector3 targetVelocity = moveInput * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity.x, ref velocityXSmoothing, accelerationTime);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocity.y, ref velocityYSmoothing, accelerationTime);

        transform.Translate(velocity * Time.deltaTime, Space.World);

        if (particles)
        {
            var emmision = particles.emission;
            emmision.enabled = moveInput != Vector2.zero;
        }
    }

    private void FireProjectile()
    {
        var projectileInstance = projectileFactory.Get();
        if (projectileStorage != null) projectileInstance.transform.parent = projectileStorage;

        ProjectileController projectileController = projectileInstance.GetComponent<ProjectileController>();
        projectileController.Init(transform.position, UtilsClass.GetMouseWorldPosition(Mouse.current.position.ReadValue()));

        SoundManager.PlaySound(SoundManager.Sound.Shoot);
    }

    private void Rotate()
    {
        Vector3 targetPosition = UtilsClass.GetMouseWorldPosition(Mouse.current.position.ReadValue());
        float angle = UtilsClass.GetAngleFromVectorFloat(targetPosition - transform.position);
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void DamagePlayer()
    {
        OnPlayerTookDamage?.Invoke(this, EventArgs.Empty);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void Init(GameController.GameBoundaries gameBoundaries)
    {
        boundaries = gameBoundaries;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            DamagePlayer();
        }
        else if (collision.CompareTag("Interactable"))
        {
            IInteractable interactable = collision.GetComponent<IInteractable>();
            interactable.OnInteraction();
        }
    }
}
