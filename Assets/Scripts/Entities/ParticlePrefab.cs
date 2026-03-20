using System.Collections.Generic;
using UnityEngine;

public class ParticlePrefab : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    ParticleSystem system;

    private bool spawn;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject obj)
    {
        if (spawn)
            return;

        spawn = true;
        List<ParticleCollisionEvent> events = new();
        system.GetCollisionEvents(obj, events);
        Vector3 position = events[0].intersection;
        GameObject summonObj = ObjectPool.Instance.GetFromPool(prefab.name);
        summonObj.transform.position = position;
    }
}
