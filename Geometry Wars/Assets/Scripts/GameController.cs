using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    // UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreText;
    private const string HIGHSCORE = "highscore";
    private int score;
    private int highscore;

    // Camera
    [Header("Camera")]
    [SerializeField] private float cameraShakeDurationDeath;
    [SerializeField] private float cameraShakeMagnitudeDeath;
    [SerializeField] private float cameraShakeDurationExplosion;
    [SerializeField] private float cameraShakeMagnitudeExplosion;
    [SerializeField] private float cameraShakeDurationEnemyHit;
    [SerializeField] private float cameraShakeMagnitudeEnemyHit;
    private CameraShake cameraShake;

    // Game Boundaries
    [Header("Game Boundaries")]
    [SerializeField] private Vector3 origin;
    [Min(1f)] [SerializeField] private float height;
    [Min(1f)] [SerializeField] private float width;
    private GameBoundaries gameBoundaries;

    // Player
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    private PlayerController2D player;

    // Spawners
    private EnemySpawner enemySpawner;
    private InteractableSpawner interactableSpawner;

    // Factory
    [Header("DeathParticles")]
    [SerializeField] private ObjectFactory deathParticleFactory;
    [SerializeField] private Transform deathParticleStorage;

    // Other
    [Header("Other")]
    [SerializeField] private float resetTimer;

    private void Start()
    {
        // UI
        score = 0;
        highscore = 0;
        highscore = PlayerPrefs.GetInt(HIGHSCORE, highscore);
        scoreText.SetText(string.Format("SCORE: {0}", score));
        highscoreText.SetText(string.Format("HIGHSCORE: {0}", highscore));
        SoundManager.SetSoundEnabled(true);

        // Camera
        cameraShake = Camera.main.GetComponent<CameraShake>(); ;

        // Init Game Boundaries
        gameBoundaries = new GameBoundaries(origin, width, height);

        // Init Player
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController2D>();
        player.Init(gameBoundaries);
        player.OnPlayerTookDamage += Player_OnPlayerTookDamage;

        // Init Spawner
        enemySpawner = GetComponent<EnemySpawner>();
        enemySpawner.Init(this, player, gameBoundaries);
        interactableSpawner = GetComponent<InteractableSpawner>();
        interactableSpawner.Init(this, player, gameBoundaries);

        // Init Factory
        deathParticleFactory.Init();
        deathParticleStorage = GameObject.Find("Death Particle Storage").transform;
    }

    private void Player_OnPlayerTookDamage(object sender, System.EventArgs e)
    {
        StartCoroutine(ResetScene());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(gameBoundaries.BotLeft, gameBoundaries.BotRight);
        Gizmos.DrawLine(gameBoundaries.BotRight, gameBoundaries.TopRight);
        Gizmos.DrawLine(gameBoundaries.TopRight, gameBoundaries.TopLeft);
        Gizmos.DrawLine(gameBoundaries.TopLeft, gameBoundaries.BotLeft);
    }

    public void SpawnDeathParticles(Vector3 position, DeathParticlesController.DeathType type = DeathParticlesController.DeathType.Default)
    {
        var deathParticleInstance = deathParticleFactory.Get();
        if (deathParticleStorage != null) deathParticleInstance.transform.parent = deathParticleStorage;

        DeathParticlesController deathParticlesController = deathParticleInstance.GetComponent<DeathParticlesController>();
        deathParticlesController.Init(position, type);
    }

    public void BombController_OnDespawn(object sender, FactoryObject.OnDespawnEventArgs e)
    {
        StartCoroutine(cameraShake.Shake(cameraShakeDurationExplosion, cameraShakeMagnitudeExplosion));
    }

    public void EnemyController_OnDespawn(object sender, FactoryObject.OnDespawnEventArgs e)
    {
        score++;
        scoreText.SetText(string.Format("SCORE: {0}", score));

        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt(HIGHSCORE, highscore);
            highscoreText.SetText(string.Format("HIGHSCORE: {0}", highscore));
        }

        StartCoroutine(cameraShake.Shake(cameraShakeDurationEnemyHit, cameraShakeMagnitudeEnemyHit));
    }

    private IEnumerator ResetScene()
    {
        player.gameObject.SetActive(false);
        SpawnDeathParticles(player.transform.position);

        SoundManager.PlaySound(SoundManager.Sound.Lose);
        StartCoroutine(cameraShake.Shake(cameraShakeDurationDeath, cameraShakeMagnitudeDeath));

        yield return new WaitForSeconds(resetTimer);

        EnemyController.enemyList.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public struct GameBoundaries
    {
        public Vector3 Origin { get; }
        public float Width { get; }
        public float Height { get; }
        public Vector3 BotLeft { get; }
        public Vector3 BotRight { get; }
        public Vector3 TopLeft { get; }
        public Vector3 TopRight { get; }

        public GameBoundaries(Vector3 origin, float width, float height)
        {
            Origin = origin;
            Width = width;
            Height = height;

            BotLeft = new Vector3(-width / 2f, -height / 2f) + origin;
            BotRight = new Vector3(width / 2f, -height / 2f) + origin;
            TopLeft = new Vector3(-width / 2f, height / 2f) + origin;
            TopRight = new Vector3(width / 2f, height / 2f) + origin;
        }
    }
}
