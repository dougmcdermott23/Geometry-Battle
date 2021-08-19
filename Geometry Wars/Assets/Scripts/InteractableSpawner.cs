using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSpawner : Spawner
{
    public enum InteractableTypes
    {
        Bomb
    };

    [Header("Bomb")]
    [Min(1f)][SerializeField] private float bombMinSpawnInterval;
    [Min(1f)][SerializeField] private float bombMaxSpawnInterval;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SpawnBombTimer(Random.Range(bombMinSpawnInterval, bombMaxSpawnInterval)));
    }

    private IEnumerator SpawnBombTimer(float spawnInterval)
    {
        yield return new WaitForSeconds(spawnInterval);

        SpawnInteractables(1, InteractableTypes.Bomb);

        StartCoroutine(SpawnBombTimer(Random.Range(bombMinSpawnInterval, bombMaxSpawnInterval)));
    }

    private void SpawnInteractables(int numInteractables, InteractableTypes interactableType, SetSpawn setSpawn = null)
    {
        FactoryObject[] interactablesSpawned = SpawnObjects(numInteractables, (int)interactableType);

        foreach (FactoryObject interactable in interactablesSpawned)
        {
            Vector3 spawnLocation = (setSpawn != null) ? setSpawn.Spawn : RandomizeSpawnLocation();

            IInteractable interactableInstance = interactable.GetComponent<IInteractable>();
            interactableInstance.Init(spawnLocation);

            interactable.OnDespawn += BombController_OnDespawn;
            interactable.OnDespawn += gameController.BombController_OnDespawn;
        }
    }

    private void BombController_OnDespawn(object sender, FactoryObject.OnDespawnEventArgs e)
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
}
