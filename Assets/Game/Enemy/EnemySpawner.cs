using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
  public enum PhaseType
  {
    Normal,
    Boss,
  }

  public enum EnemyTier
  {
    Zako,
    MidBoss,
    Boss,
  }

  [Serializable]
  public class EnemyEntry
  {
    public GameObject prefab;
    public EnemyTier tier = EnemyTier.Zako;
    public float threatValue = 1f;
    [Tooltip("抽選される重み。値が大きいほど選ばれやすい")]
    public float spawnWeight = 1f;
  }

  [Serializable]
  public class PhaseConfig
  {
    public float spawnThreshold = 5f;
    public float spawnInterval = 1.5f;
    public List<EnemyEntry> enemyPool = new();
  }

  [Header("配置")]
  [SerializeField] Transform player;
  [SerializeField] float minSpawnDistance = 4f;
  [SerializeField] Vector2 spawnAreaCenter = Vector2.zero;
  [SerializeField] Vector2 spawnAreaSize = new Vector2(16f, 9f);

  [Header("演出")]
  [SerializeField] GameObject spawnWarningPrefab;
  [SerializeField] float warningDuration = 1.5f;
  [SerializeField] float activationDelay = 0.5f;

  [Header("フェーズ")]
  [SerializeField] List<PhaseConfig> normalPhases = new();
  [SerializeField] PhaseConfig bossPhase = new();
  [SerializeField] float bossPhaseTimeout = 60f;

  public event Action OnBossPhaseEnd;

  PhaseConfig currentPhase;
  PhaseType currentPhaseType;
  float currentThreatSum;
  float bossTimer;
  bool bossDefeated;
  bool phaseActive;
  Coroutine spawnLoop;
  readonly List<HPController> aliveEnemies = new();

  void Start()
  {
    if (player == null)
    {
      GameObject playerGo = GameObject.FindWithTag("Player");
      if (playerGo != null) player = playerGo.transform;
    }
  }

  void Update()
  {
    if (!phaseActive || currentPhaseType != PhaseType.Boss) return;

    bossTimer += Time.deltaTime;
    if (bossDefeated || bossTimer >= bossPhaseTimeout)
      EndBossPhase();
  }

  public void StartPhase(PhaseType type, int normalPhaseIndex = 0)
  {
    currentPhaseType = type;
    if (type == PhaseType.Boss)
    {
      currentPhase = bossPhase;
    }
    else
    {
      currentPhase = normalPhaseIndex < normalPhases.Count
        ? normalPhases[normalPhaseIndex]
        : new PhaseConfig();
    }
    currentThreatSum = 0f;
    bossTimer = 0f;
    bossDefeated = false;
    phaseActive = true;

    if (spawnLoop != null) StopCoroutine(spawnLoop);
    spawnLoop = StartCoroutine(SpawnLoop());
  }

  IEnumerator SpawnLoop()
  {
    if (currentPhaseType == PhaseType.Boss)
    {
      EnemyEntry bossEntry = PickEntry();
      if (bossEntry != null)
        yield return StartCoroutine(SpawnRoutine(bossEntry));
      yield break;
    }

    while (phaseActive && currentPhaseType == PhaseType.Normal)
    {
      while (currentThreatSum < currentPhase.spawnThreshold)
      {
        EnemyEntry pick = PickEntry();
        if (pick == null) break;
        currentThreatSum += pick.threatValue;
        StartCoroutine(SpawnRoutine(pick));
      }
      yield return new WaitForSeconds(currentPhase.spawnInterval);
    }
  }

  EnemyEntry PickEntry()
  {
    List<EnemyEntry> pool = currentPhase != null ? currentPhase.enemyPool : null;
    if (pool == null || pool.Count == 0) return null;

    float totalWeight = 0f;
    foreach (EnemyEntry entry in pool)
      totalWeight += Mathf.Max(0f, entry.spawnWeight);

    if (totalWeight <= 0f)
      return pool[UnityEngine.Random.Range(0, pool.Count)];

    float roll = UnityEngine.Random.Range(0f, totalWeight);
    foreach (EnemyEntry entry in pool)
    {
      roll -= Mathf.Max(0f, entry.spawnWeight);
      if (roll <= 0f) return entry;
    }
    return pool[pool.Count - 1];
  }

  IEnumerator SpawnRoutine(EnemyEntry entry)
  {
    Vector2 pos = PickSpawnPosition();

    GameObject warning = null;
    if (spawnWarningPrefab != null)
      warning = Instantiate(spawnWarningPrefab, pos, Quaternion.identity);
    yield return new WaitForSeconds(warningDuration);
    if (warning != null) Destroy(warning);

    if (entry.prefab == null) yield break;

    GameObject enemyGo = Instantiate(entry.prefab, pos, Quaternion.identity);
    EnemyControllerBase controller = enemyGo.GetComponent<EnemyControllerBase>();
    HPController hp = enemyGo.GetComponent<HPController>();
    if (controller != null) controller.enabled = false;

    if (hp != null)
    {
      aliveEnemies.Add(hp);
      float capturedThreat = entry.threatValue;
      EnemyTier capturedTier = entry.tier;
      hp.OnDeath += () => HandleEnemyDeath(hp, capturedThreat, capturedTier);
    }

    yield return new WaitForSeconds(activationDelay);
    if (controller != null) controller.enabled = true;
  }

  Vector2 PickSpawnPosition()
  {
    Vector2 pos = spawnAreaCenter;
    int safety = 16;
    while (safety-- > 0)
    {
      pos = spawnAreaCenter + new Vector2(
          UnityEngine.Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
          UnityEngine.Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f));
      if (player == null) break;
      if (Vector2.Distance(pos, player.position) >= minSpawnDistance) break;
    }
    return pos;
  }

  void HandleEnemyDeath(HPController hp, float threatValue, EnemyTier tier)
  {
    aliveEnemies.Remove(hp);
    currentThreatSum = Mathf.Max(0f, currentThreatSum - threatValue);
    if (tier == EnemyTier.Boss && currentPhaseType == PhaseType.Boss)
      bossDefeated = true;
  }

  void EndBossPhase()
  {
    if (spawnLoop != null) StopCoroutine(spawnLoop);
    spawnLoop = null;
    phaseActive = false;
    OnBossPhaseEnd?.Invoke();
  }

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    if (player != null)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(player.position, minSpawnDistance);
    }
  }
}
