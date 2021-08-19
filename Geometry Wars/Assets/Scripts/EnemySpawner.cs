using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Spawner
{
    public enum EnemyTypes
    {
        Follow,
        Bouncing,
        Flocking
    };

    [Header("Follow Enemy")]
    [SerializeField] private float followSpawnInterval;
    [SerializeField] private int followNumSpawns;
    [SerializeField] private int followSpawnIncrease;
    [SerializeField] private int followMaxSpawns;

    [Header("Bouncing Enemy")]
    [SerializeField] private float bouncingSpawnInterval;
    [SerializeField] private int bouncingNumSpawns;
    [SerializeField] private int bouncingSpawnIncrease;
    [SerializeField] private int bouncingMaxSpawns;

    [Header("Flocking Enemy")]
    [SerializeField] private float flockingSpawnInterval;
    [SerializeField] private int flockingNumSpawns;
    [SerializeField] private int flockingSpawnIncrease;
    [SerializeField] private int flockingMaxSpawns;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SpawnFollowEnemyTimer(followSpawnInterval));
        StartCoroutine(SpawnBouncingEnemyTimer(bouncingSpawnInterval));
        StartCoroutine(SpawnFlockingEnemyTimer(flockingSpawnInterval));
    }

    private IEnumerator SpawnFollowEnemyTimer(float spawnInterval)
    {
        yield return new WaitForSeconds(spawnInterval);

        if (EnemyController.enemyList.Count < followMaxSpawns + bouncingMaxSpawns + flockingMaxSpawns)
        {
            SpawnEnemies(followNumSpawns, EnemyTypes.Follow);

            if (followNumSpawns < followMaxSpawns)
                followNumSpawns += followSpawnIncrease;
            if (followNumSpawns > followMaxSpawns)
                followNumSpawns = followMaxSpawns;
        }

        StartCoroutine(SpawnFollowEnemyTimer(spawnInterval));
    }

    private IEnumerator SpawnBouncingEnemyTimer(float spawnInterval)
    {
        yield return new WaitForSeconds(spawnInterval);

        if (EnemyController.enemyList.Count < followMaxSpawns + bouncingMaxSpawns + flockingMaxSpawns)
        {
            SpawnEnemies(bouncingNumSpawns, EnemyTypes.Bouncing);

            if (bouncingNumSpawns < bouncingMaxSpawns)
                bouncingNumSpawns += bouncingSpawnIncrease;
            if (bouncingNumSpawns > bouncingMaxSpawns)
                bouncingNumSpawns = bouncingMaxSpawns;
        }

        StartCoroutine(SpawnBouncingEnemyTimer(spawnInterval));
    }

    private IEnumerator SpawnFlockingEnemyTimer(float spawnInterval)
    {
        yield return new WaitForSeconds(spawnInterval);

        if (EnemyController.enemyList.Count < followMaxSpawns + bouncingMaxSpawns + flockingMaxSpawns)
        {
            Vector3 firstPos = new Vector3();
            Vector3 secondPos = new Vector3();
            List<Vector3> spawnLocations = new List<Vector3> { boundaries.TopLeft, boundaries.TopRight, boundaries.BotLeft, boundaries.BotRight };

            GetTwoFurthestPointsInList(ref firstPos, ref secondPos, spawnLocations);

            SpawnEnemies(flockingNumSpawns / 2, EnemyTypes.Flocking, new SetSpawn(firstPos));
            SpawnEnemies(flockingNumSpawns / 2, EnemyTypes.Flocking, new SetSpawn(secondPos));

            if (flockingNumSpawns < flockingMaxSpawns)
                flockingNumSpawns += flockingSpawnIncrease;
            if (flockingNumSpawns > flockingMaxSpawns)
                flockingNumSpawns = flockingMaxSpawns;
        }

        StartCoroutine(SpawnFlockingEnemyTimer(spawnInterval));
    }

    private void SpawnEnemies(int numEnemies, EnemyTypes enemyType, SetSpawn setSpawn = null)
    {
        FactoryObject[] enemiesSpawned = SpawnObjects(numEnemies, (int)enemyType);

        foreach (FactoryObject enemy in enemiesSpawned)
        {
            Vector3 spawnLocation = (setSpawn != null) ? setSpawn.Spawn : RandomizeSpawnLocation();

            IEnemy enemyInstance = enemy.GetComponent<IEnemy>();
            enemyInstance.Init(player, spawnLocation, boundaries);
            EnemyController.enemyList.Add(enemyInstance);

            enemy.OnDespawn += EnemyController_OnDespawn;
            enemy.OnDespawn += gameController.EnemyController_OnDespawn;
        }
    }

    private void EnemyController_OnDespawn(object sender, EnemyController.OnDespawnEventArgs e)
    {
        DeathParticlesController.DeathType deathType;

        switch (e.objectType)
        {
            case FactoryObject.ObjectType.Follow:
                deathType = DeathParticlesController.DeathType.Follow;
                break;
            case FactoryObject.ObjectType.Bouncing:
                deathType = DeathParticlesController.DeathType.Bouncing;
                break;
            case FactoryObject.ObjectType.Flocking:
                deathType = DeathParticlesController.DeathType.Flocking;
                break;
            default:
                deathType = DeathParticlesController.DeathType.Default;
                break;
        }

        gameController.SpawnDeathParticles(e.despawnPosition, deathType);
    }

    private void GetTwoFurthestPointsInList(ref Vector3 firstPos, ref Vector3 secondPos, List<Vector3> positions)
    {
        float firstPosDist = Mathf.NegativeInfinity;
        float secondPosDist = Mathf.NegativeInfinity;

        foreach (Vector3 corner in positions)
        {
            float distance = Vector3.Distance(player.transform.position, corner);
            if (distance > firstPosDist)
            {
                secondPos = firstPos;
                secondPosDist = firstPosDist;
                firstPos = corner;
                firstPosDist = distance;
            }
            else if (distance > secondPosDist)
            {
                secondPos = corner;
                secondPosDist = distance;
            }
        }
    }
}
