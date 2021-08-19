using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyController : FactoryObject
{
    public static List<IEnemy> enemyList = new List<IEnemy>();

    [SerializeField] private float waitTimeOnInit;

    public Rigidbody2D RigidBody { get; private set; }
    public PlayerController2D Target { get; private set; }
    public GameController.GameBoundaries GameBoundaries { get; private set; }
    public bool CanMove { get; private set; }

    protected virtual void Awake()
    {
        RigidBody = GetComponent<Rigidbody2D>();
    }

    public virtual void Init(PlayerController2D playerController2D, Vector3 startPosition, GameController.GameBoundaries boundaries)
    {
        transform.position = startPosition;
        Target = playerController2D;
        GameBoundaries = boundaries;
        CanMove = false;
        StartCoroutine(WaitOnSpawn(waitTimeOnInit));
    }

    public virtual void Damage()
    {
        Reclaim();
    }

    public FactoryObject.ObjectType GetEnemyType()
    {
        return factoryObjectType;
    }

    private IEnumerator WaitOnSpawn(float timer)
    {
        yield return new WaitForSeconds(timer);

        CanMove = true;
    }
}
