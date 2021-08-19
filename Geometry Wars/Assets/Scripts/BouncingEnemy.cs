using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEnemy : EnemyController, IEnemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    private Vector2 direction;
    private GameController.GameBoundaries boundaries;

    public Vector3 Velocity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        factoryObjectType = ObjectType.Bouncing;
        RigidBody.isKinematic = true;
    }

    private void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (!CanMove)
            return;

        Velocity = direction * moveSpeed * Time.deltaTime;
        CheckBoundaries();
        transform.Translate(Velocity, Space.World);
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void CheckBoundaries()
    {
        Vector3 newVelocity = Velocity;
        Vector3 pos = transform.position + Velocity;

        bool verticalHit = (pos.y > GameBoundaries.TopLeft.y) || (pos.y < GameBoundaries.BotLeft.y);
        bool horizontalHit = (pos.x > GameBoundaries.TopRight.x) || (pos.x < GameBoundaries.TopLeft.x);

        if (verticalHit)
        {
            newVelocity.y *= -1;
            direction.y *= -1;
        }
        if (horizontalHit)
        {
            newVelocity.x *= -1;
            direction.x *= -1;
        }

        Velocity = newVelocity;
    }

    public override void Init(PlayerController2D target, Vector3 startPosition, GameController.GameBoundaries boundaries)
    {
        base.Init(target, startPosition, boundaries);
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    public override void Damage()
    {
        enemyList.Remove(this);
        base.Damage();
    }
}
