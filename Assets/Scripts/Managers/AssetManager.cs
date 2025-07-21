using Netick.Unity;
using UnityEngine;

public class AssetManager : MonoSingleton<AssetManager>
{
    [SerializeField] private GameObject _arrowPrefab;
    public GameObject ArrowPrefab => _arrowPrefab;
}