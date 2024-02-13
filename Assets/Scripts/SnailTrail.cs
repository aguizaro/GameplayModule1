using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailTrail : MonoBehaviour
{
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private int poolSize = 30;

    private List<GameObject> _particlePool;

    private void Start() => InitObjPool();

    // create pool of particle objects
    private void InitObjPool()
    {
        _particlePool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(_particlePrefab);
            particle.SetActive(false);
            _particlePool.Add(particle);
        }
    }

    // gets inactive gameObj from pool or creates new gameObj
    public GameObject GetPooledObj()
    {
        foreach (GameObject particle in _particlePool) if (!particle.activeInHierarchy) return particle;

        // if no free particles, recyle the oldest one
        GameObject oldestParticle = _particlePool[0];
        _particlePool.RemoveAt(0);
        _particlePool.Add(oldestParticle);
        return oldestParticle;
    }
}