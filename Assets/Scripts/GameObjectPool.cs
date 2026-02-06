using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    [Header("Pool Setup")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int prewarmCount = 30;
    [SerializeField] private bool expandable = true;

    private readonly Queue<PoolMember> _pool = new Queue<PoolMember>();

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError($"[{name}] Pool sin prefab asignado.");
            return;
        }

        for (int i = 0; i < prewarmCount; i++)
        {
            var member = CreateNew();
            Release(member);
        }
    }

    private PoolMember CreateNew()
    {
        GameObject go = Instantiate(prefab, transform);
        go.SetActive(false);

        var member = go.GetComponent<PoolMember>();
        if (member == null) member = go.AddComponent<PoolMember>();
        member.Pool = this;

        return member;
    }

    public PoolMember Get(Vector3 position, Quaternion rotation)
    {
        PoolMember member = _pool.Count > 0 ? _pool.Dequeue() : (expandable ? CreateNew() : null);

        if (member == null) return null;

        Transform t = member.transform;
        t.SetParent(null);
        t.position = position;
        t.rotation = rotation;

        member.gameObject.SetActive(true);
        return member;
    }

    public void Release(PoolMember member)
    {
        if (member == null) return;

        member.gameObject.SetActive(false);
        member.transform.SetParent(transform);
        _pool.Enqueue(member);
    }
}
