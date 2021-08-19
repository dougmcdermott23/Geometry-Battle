using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnemy : EnemyController, IEnemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    public Vector3 Velocity { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        factoryObjectType = ObjectType.Follow;
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

        Vector2 direction = (Target.transform.position - transform.position).normalized;
        Velocity = direction * moveSpeed * Time.deltaTime;
        transform.Translate(Velocity, Space.World);
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    public override void Init(PlayerController2D target, Vector3 startPosition, GameController.GameBoundaries boundaries)
    {
        base.Init(target, startPosition, boundaries);
    }

    public override void Damage()
    {
        enemyList.Remove(this);
        base.Damage();
    }
}
