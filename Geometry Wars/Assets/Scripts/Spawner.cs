using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spawner : MonoBehaviour
{
    [Header("No Spawn Radius")]
    [SerializeField] private float playerNoSpawnRadius;

    [Header("Object Factory")]
    [SerializeField] protected ObjectFactory objectFactory;
    [SerializeField] protected Transform objectStorage;

    protected GameController gameController;
    protected PlayerController2D player;
    protected GameController.GameBoundaries boundaries;

    protected class SetSpawn
    {
        public Vector3 Spawn { get; }
        public SetSpawn(Vector3 location)
        {
            Spawn = location;
        }
    }

    protected virtual void Start()
    {
        objectFactory.Init();
    }

    protected FactoryObject[] SpawnObjects(int numObjects, int objectId)
    {
        FactoryObject[] objectsSpawned = new FactoryObject[numObjects];

        for (int i = 0; i < numObjects; i++)
            objectsSpawned[i] = SpawnObject(objectId);

        return objectsSpawned;
    }

    private FactoryObject SpawnObject(int objectId)
    {
        var objectInstance = objectFactory.Get(objectId);
        if (objectStorage != null) objectInstance.transform.parent = objectStorage;

        return objectInstance;
    }

    protected Vector3 RandomizeSpawnLocation()
    {
        Vector3 spawnLocation = Vector3.zero;
        bool spawnSet = false;

        while (!spawnSet)
        {
            float randX = Random.Range(0, boundaries.Width) - boundaries.Width / 2f;
            float randY = Random.Range(0, boundaries.Height) - boundaries.Height / 2f;
            Vector3 testLocation = new Vector3(randX, randY) + boundaries.Origin;

            if (Vector3.Distance(testLocation, player.transform.position) > playerNoSpawnRadius)
            {
                spawnLocation = testLocation;
                spawnSet = true;
            }
        }

        return spawnLocation;
    }

    public void Init(GameController controller, PlayerController2D playerController2D, GameController.GameBoundaries gameBoundaries)
    {
        gameController = controller;
        player = playerController2D;
        boundaries = gameBoundaries;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (player != null) Gizmos.DrawWireSphere(player.transform.position, playerNoSpawnRadius);
    }
}
