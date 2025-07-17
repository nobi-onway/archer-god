using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    private const int SIZE = 5;
    [SerializeField] private ObjectPool _arrowPrefab;

    private Pool _arrowPool;
    public Pool ArrowPool => _arrowPool ??= GetArrowPool(_arrowPrefab);

    public Pool GetArrowPool(ObjectPool prefab)
    {
        Transform container = new GameObject("Arrow-Pool").transform;
        container.SetParent(this.transform);

        return new Pool(() => ExpandPool(prefab, SIZE, container));
    }

    private ObjectPool[] ExpandPool(ObjectPool prefab, int size, Transform container)
    {
        ObjectPool[] objectPools = new ObjectPool[size];

        for (int i = 0; i < size; i++)
        {
            objectPools[i] = Instantiate(prefab, container, true);
        }

        return objectPools;
    }
}