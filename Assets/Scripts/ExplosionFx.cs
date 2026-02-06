using System.Collections;
using UnityEngine;

public class ExplosionFx : MonoBehaviour
{
    [SerializeField] private float returnDelay = 0.7f;

    private PoolMember _poolMember;
    private Coroutine _co;

    private void Awake()
    {
        _poolMember = GetComponent<PoolMember>();
    }

    private void OnEnable()
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(ReturnRoutine());
    }

    private IEnumerator ReturnRoutine()
    {
        yield return new WaitForSeconds(returnDelay);
        _poolMember.ReturnToPool();
    }
}
