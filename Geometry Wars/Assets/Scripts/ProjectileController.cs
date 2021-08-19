using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools.Utils;

public class ProjectileController : FactoryObject
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxLifetime;

    private TrailRenderer trail;
    private float lifetime;
    private Vector3 moveDirection;

    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        Move();
        ReclaimOnTimer();
    }

    private void Move()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void ReclaimOnTimer()
    {
        if (lifetime <= 0)
            Reclaim();
        else
            lifetime -= Time.deltaTime;
    }

    public void Init(Vector3 startPosition, Vector3 targetPosition)
    {
        transform.position = startPosition;
        lifetime = maxLifetime;

        moveDirection = (targetPosition - startPosition).normalized;
        float angle = UtilsClass.GetAngleFromVectorFloat(moveDirection);
        transform.eulerAngles = new Vector3(0, 0, angle);

        trail?.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            IEnemy enemy = collision.GetComponent<IEnemy>();
            enemy.Damage();
            Reclaim();

            SoundManager.PlaySound(SoundManager.Sound.EnemyHit);
        }
    }
}
