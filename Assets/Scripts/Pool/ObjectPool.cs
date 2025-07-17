using System;
using UnityEngine;

public abstract class ObjectPool : MonoBehaviour, IObjectPool
{
    public event Action<ObjectPool> OnReturnToPool;

    public IObjectPool ReleasePool()
    {
        this.gameObject.SetActive(true);
        return this;
    }

    public void ReturnToPool()
    {
        this.gameObject.SetActive(false);

        OnReturnToPool?.Invoke(this);
    }
}