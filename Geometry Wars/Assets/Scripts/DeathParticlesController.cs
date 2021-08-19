using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathParticlesController : FactoryObject
{
    public enum DeathType
    {
        Follow,
        Bouncing,
        Flocking,
        Default,
    };

    [SerializeField] private Material[] materialArray;
    [SerializeField] private float despawnTimer = 1f;

    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();   
    }

    public void Init(Vector3 position, DeathType type = DeathType.Default)
    {
        transform.position = position;
        particles.GetComponent<Renderer>().material = materialArray[(int)type];
        particles.Play();
        StartCoroutine(DespawnOnTimer(despawnTimer));
    }

    private IEnumerator DespawnOnTimer(float timer)
    {
        yield return new WaitForSeconds(timer);

        Reclaim();
    }
}
