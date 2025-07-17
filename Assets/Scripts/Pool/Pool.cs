using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool
{
    private Queue<ObjectPool> _inPoolQueue;
    private List<ObjectPool> _outPoolList;
    private readonly Func<ObjectPool[]> OnExpandPool;

    public Pool(Func<ObjectPool[]> onExpandPool)
    {
        OnExpandPool = onExpandPool;

        _outPoolList = new();
        _inPoolQueue = new();

        HandleExpandPool();
    }

    private void HandleExpandPool()
    {
        foreach (ObjectPool objectPool in OnExpandPool())
        {
            objectPool.OnReturnToPool += AddToPool;

            objectPool.ReturnToPool();
        }
    }

    public ObjectPool GetObjectPool()
    {
        if (_inPoolQueue.Count == 0) HandleExpandPool();

        ObjectPool objectPool = _inPoolQueue.Dequeue();

        _outPoolList.Add(objectPool);
        objectPool.ReleasePool();

        return objectPool;
    }

    private void AddToPool(ObjectPool objectPool)
    {
        if (_outPoolList.Contains(objectPool)) _outPoolList.Remove(objectPool);

        _inPoolQueue.Enqueue(objectPool);
    }
}