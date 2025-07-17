using System;

public interface IObjectPool
{
    IObjectPool ReleasePool();
    void ReturnToPool();
}