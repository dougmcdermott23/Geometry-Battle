using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingEnemy : EnemyController, IEnemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    [Header("Boids Variables")]
    [SerializeField] private float visionRadius;
    [Range(0f, 5f)] [SerializeField] private float separationDistance = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float alignmentWeight = 0.125f;
    [Range(0f, 1f)] [SerializeField] private float cohesionWeight = 0.01f;
    [Range(0f, 1f)] [SerializeField] private float followPlayerWeight = 0.5f;
    public Vector3 Velocity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        factoryObjectType = ObjectType.Flocking;
        RigidBody.isKinematic = true;
    }

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 flockVelocity = CalculateFlockMovement();
        Vector3 predatorVelocity = FollowPlayer();

        Velocity += flockVelocity + predatorVelocity;

        BoundPosition();

        if (Velocity.magnitude > moveSpeed)
            Velocity = Velocity.normalized * moveSpeed;

        transform.Translate(Velocity * Time.deltaTime, Space.World);
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private Vector3 CalculateFlockMovement()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        float flockCount = 0;

        foreach (IEnemy enemy in EnemyController.enemyList)
        {
            if (transform == enemy.transform)
                continue;

            if (Vector3.Distance(transform.position, enemy.transform.position) <= visionRadius && enemy.GetEnemyType() == FactoryObject.ObjectType.Flocking)
            {
                flockCount++;

                // Separation
                if (Vector3.Distance(transform.position, enemy.transform.position) < separationDistance)
                    separation -= (enemy.transform.position - transform.position);

                // Alignment
                alignment += enemy.Velocity;

                // Cohesion
                cohesion += enemy.transform.position;
            }
        }

        // Normalize vectors and apply weights for alignment and cohesion
        if (flockCount > 0)
        {
            alignment /= flockCount;
            alignment = (alignment - Velocity) * alignmentWeight;

            cohesion /= flockCount;
            cohesion = (cohesion - transform.position) * cohesionWeight;
        }

        return separation + alignment + cohesion;
    }

    private List<IEnemy> FlockList()
    {
        List<IEnemy> flockList = new List<IEnemy>();

        foreach (IEnemy enemy in EnemyController.enemyList)
        {
            if (transform == enemy.transform)
                continue;

            if (Vector3.Distance(transform.position, enemy.transform.position) <= visionRadius && enemy.GetEnemyType() == FactoryObject.ObjectType.Flocking)
                flockList.Add(enemy);
        }

        return flockList;
    }

    private Vector3 Separation(List<IEnemy> flockList)
    {
        Vector3 separation = new Vector3();

        foreach (IEnemy enemy in flockList)
        {
            if (Vector3.Distance(transform.position, enemy.transform.position) < separationDistance)
                separation -= (enemy.transform.position - transform.position);
        }

        return separation;
    }

    private Vector3 Alignment(List<IEnemy> flockList)
    {
        Vector3 perceivedVelocity = new Vector3();

        foreach (IEnemy enemy in flockList)
            perceivedVelocity += enemy.Velocity;

        perceivedVelocity /= flockList.Count;
        perceivedVelocity = (perceivedVelocity - Velocity) * alignmentWeight;

        return perceivedVelocity;
    }

    private Vector3 Cohesion(List<IEnemy> flockList)
    {
        Vector3 centerOfMass = new Vector3();

        foreach (IEnemy enemy in flockList)
            centerOfMass += enemy.transform.position;

        centerOfMass /= flockList.Count;
        centerOfMass = (centerOfMass - transform.position) * cohesionWeight;

        return centerOfMass;
    }

    private Vector3 FollowPlayer()
    {
        Vector3 followPlayer = new Vector3();

        if (Vector3.Distance(transform.position, Target.transform.position) < visionRadius)
        {
            followPlayer = Target.transform.position - transform.position;
            followPlayer *= followPlayerWeight;
        }

        return followPlayer;
    }

    private void BoundPosition()
    {
        Vector3 boundVelocity = Velocity;

        if (transform.position.x < GameBoundaries.BotLeft.x)
            boundVelocity.x = moveSpeed * 1f;
        else if (transform.position.x > GameBoundaries.BotRight.x)
            boundVelocity.x = moveSpeed * -1f;

        if (transform.position.y < GameBoundaries.BotLeft.y)
            boundVelocity.y = moveSpeed * 1f;
        else if (transform.position.y > GameBoundaries.TopLeft.y)
            boundVelocity.y = moveSpeed * -1f;

        Velocity = boundVelocity;
    }

    public override void Init(PlayerController2D target, Vector3 startPosition, GameController.GameBoundaries boundaries)
    {
        base.Init(target, startPosition, boundaries);
        Vector2 direction = (Target.transform.position - transform.position).normalized;
        Velocity = direction * moveSpeed;
    }

    public override void Damage()
    {
        enemyList.Remove(this);
        base.Damage();
    }
}
