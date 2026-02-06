using UnityEngine;

public class PoolMember : MonoBehaviour
{
    [HideInInspector] public GameObjectPool Pool;

    public void ReturnToPool()
    {
        if (Pool != null) Pool.Release(this);
        else Destroy(gameObject);
    }
}
