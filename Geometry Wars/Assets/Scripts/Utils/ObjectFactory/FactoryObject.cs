using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryObject : MonoBehaviour
{
    public enum ObjectType
    {
        Follow,
        Bouncing,
        Flocking,
        Default,
    };

    public event EventHandler<OnDespawnEventArgs> OnDespawn;

    public class OnDespawnEventArgs : EventArgs
    {
        public Vector3 despawnPosition;
        public ObjectType objectType;
    }

    [HideInInspector] public ObjectFactory originFactory;

    private int objectId = int.MinValue;
    protected ObjectType factoryObjectType = ObjectType.Default;

    public int ObjectId
    {
        get
        {
            return objectId;
        }
        set
        {
            if (objectId == int.MinValue && value != int.MinValue)
            {
                objectId = value;
            }
        }
    }

    public virtual void Reclaim(bool invokeOnReclaim = true)
    {
        if (invokeOnReclaim)
            InvokeOnDespawn();

        if (originFactory != null)
            originFactory.Reclaim(this);
        else
            Destroy(this.gameObject);
    }

    public void InvokeOnDespawn()
    {
        OnDespawn?.Invoke(this, new OnDespawnEventArgs { despawnPosition = transform.position, objectType = factoryObjectType });
        OnDespawn = null;
    }
}
