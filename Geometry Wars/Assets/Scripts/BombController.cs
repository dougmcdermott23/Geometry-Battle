using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : FactoryObject, IInteractable
{
    private MeshRenderer bombMeshRenderer;
    private MeshRenderer blastMeshRenderer;
    private Color blastBaseColor;
    private bool isTriggered;
    private float explosionTimer;
    private Vector3 startingBlastSize;
    private Vector3 maxBlastSize; 
    private Vector3 bombStartPosition;

    [SerializeField] private float explosionRange;
    [SerializeField] private float explosionBlastTime;

    [Header("Animation")]
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationHeight;
    [SerializeField] private float animationSpeed;

    public float InteractionRange { get; private set; }

    private void Awake()
    {
        bombMeshRenderer = GetComponent<MeshRenderer>();
        blastMeshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        blastBaseColor = blastMeshRenderer.material.color;

        startingBlastSize = Vector3.one;
        maxBlastSize = new Vector3(explosionRange * 2f, explosionRange * 2f);
    }

    private void Update()
    {
        if (!isTriggered)
        {
            transform.position = new Vector3(transform.position.x, animationCurve.Evaluate((Time.time * animationSpeed % animationCurve.length)) * animationHeight + bombStartPosition.y, transform.position.z);
        }
        else
        {
            ReclaimOnTimer();
        }
    }

    public void Init(Vector3 startPosition)
    {
        blastMeshRenderer.transform.localScale = startingBlastSize;
        blastMeshRenderer.material.color = blastBaseColor;

        bombMeshRenderer.enabled = true;
        blastMeshRenderer.gameObject.SetActive(false);

        transform.position = startPosition;
        bombStartPosition = startPosition;
        isTriggered = false;
    }

    public void OnInteraction()
    {
        if (!isTriggered)
        {
            isTriggered = true;

            for (int i = EnemyController.enemyList.Count - 1; i >= 0; i--)
            {
                if (Vector3.Distance(transform.position, EnemyController.enemyList[i].transform.position) < explosionRange)
                    EnemyController.enemyList[i].Damage();
            }

            InitBlast();
        }
    }

    private void InitBlast()
    {
        InvokeOnDespawn();

        bombMeshRenderer.enabled = false;
        blastMeshRenderer.gameObject.SetActive(true);
        explosionTimer = explosionBlastTime;

        SoundManager.PlaySound(SoundManager.Sound.Bomb);
    }

    private void ReclaimOnTimer()
    {
        if (explosionTimer <= 0)
        {
            blastMeshRenderer.gameObject.SetActive(false);
            Reclaim(false);
        }
        else
        {
            float t = Mathf.InverseLerp(0, explosionBlastTime, explosionBlastTime - explosionTimer);

            Vector3 blastSize = Vector3.Lerp(Vector3.one, maxBlastSize, t);
            blastMeshRenderer.transform.localScale = blastSize;

            Color blastUpdateColor = Color.Lerp(blastBaseColor, Color.clear, t);
            blastMeshRenderer.material.color = blastUpdateColor;

            explosionTimer -= Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
