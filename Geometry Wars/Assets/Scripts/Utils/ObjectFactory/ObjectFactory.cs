using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ObjectFactory : ScriptableObject
{
    [SerializeField] private FactoryObject[] preFabs;
    [SerializeField] private bool recycle;
    [SerializeField] private Vector3 despawnPosition;

    private List<FactoryObject>[] pools;

    public FactoryObject Get(int objectId = 0)
    {
        FactoryObject instance;

        if (recycle)
        {
            if (pools == null)
                CreatePools();
            List<FactoryObject> pool = pools[objectId];
            int lastIndex = pool.Count - 1;
            if (lastIndex >= 0)
            {
                instance = pool[lastIndex];
                instance.gameObject.SetActive(true);
                pool.RemoveAt(lastIndex);
            }
            else
            {
                instance = Instantiate(preFabs[objectId]);
                instance.ObjectId = objectId;
                instance.originFactory = this;
            }
        }
        else
        {
            instance = Instantiate(preFabs[objectId]);
            instance.ObjectId = objectId;
        }

        return instance;
    }

    public Object GetRandom()
    {
        return Get(Random.Range(0, preFabs.Length));
    }

    public void Reclaim(FactoryObject objectToRecycle)
    {
        if (recycle)
        {
            if (pools == null)
                CreatePools();
            pools[objectToRecycle.ObjectId].Add(objectToRecycle);
            objectToRecycle.gameObject.SetActive(false);
            objectToRecycle.transform.position = despawnPosition;
        }
        else
        {
            Destroy(objectToRecycle.gameObject);
        }
    }

    public void Init()
    {
        if (pools == null)
            CreatePools();
        else
        {
            foreach (List<FactoryObject> pool in pools)
                pool.Clear();
        }
    }

    private void CreatePools()
    {
        pools = new List<FactoryObject>[preFabs.Length];
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i] = new List<FactoryObject>();
        }
    }
}
