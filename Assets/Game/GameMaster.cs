using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum FieldStatus
{
    Normal,
    Battle,
}

public class GameMaster : MonoBehaviour
{
    public static GameMaster Instance { get; private set; }

    [SerializeField] GameObject settingCanvas;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] PlayerController player;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] ShopController shopController;
    [SerializeField] float normalPhaseDuration = 60f;
    [SerializeField] int normalPhaseRepeat = 3;
    [SerializeField] int storage = 0;

    [Space(10)]
    [Header("ScrollBar")]
    [SerializeField] RectTransform scrollBar;

    [Space(10)]
    [Header("Enemy Move Limit")]
    [SerializeField] float enemyMinX = -8f;
    [SerializeField] float enemyMaxX = 8f;
    [SerializeField] float enemyMinY = -4f;
    [SerializeField] float enemyMaxY = 4f;
    public Animator globalVolumeAnimator;
    public Animator replay_animator;

    public UnityEvent<HPController> DeathEvent { get; } = new();

    public bool isMenuOpen { get; private set; }
    public int phaseIndex { get; private set; }
    bool bossPhaseEnded;
    float phaseStartTime;
    float currentPhaseDuration;
    public FieldStatus fieldStatus { get; private set; }
    public List<string> logs { get; private set; } = new();
    public int score { get; private set; }
    public int Storage => storage;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (player != null)
        {
            var playerHP = player.GetComponent<HPController>();
            if (playerHP != null)
                playerHP.OnDeath += OnPlayerDeath;
        }
        if (enemySpawner != null)
        {
            enemySpawner.OnBossPhaseEnd += () => bossPhaseEnded = true;
            StartCoroutine(PhaseCycleRoutine());
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            SetMenuOpen(!isMenuOpen);
        UpdateScrollBar();
    }

    void UpdateScrollBar()
    {
        if (scrollBar == null || currentPhaseDuration <= 0f) return;
        float t = Mathf.Clamp01((Time.time - phaseStartTime) / currentPhaseDuration);
        Vector2 pos = scrollBar.anchoredPosition;
        pos.y = Mathf.Lerp(400f, -400f, t);
        scrollBar.anchoredPosition = pos;
    }

    public void SetMenuOpen(bool open)
    {
        isMenuOpen = open;
        settingCanvas.SetActive(open);
        if (open)
            globalVolumeAnimator.SetTrigger("open");
        else
            globalVolumeAnimator.SetTrigger("close");

        if (open) StartCoroutine(Pause());
        else Time.timeScale = 1f;

    }
    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.2f);
        Time.timeScale = 0f;
    }

    public void RegisterEnemy(HPController enemyHP, int scoreValue)
    {
        string enemyName = enemyHP.gameObject.name;
        enemyHP.OnDeath += () => OnEnemyDeath(enemyName, scoreValue);
    }

    IEnumerator PhaseCycleRoutine()
    {
        for (int i = 0; i < normalPhaseRepeat; i++)
        {
            phaseIndex = i;
            phaseStartTime = Time.time;
            currentPhaseDuration = normalPhaseDuration;
            enemySpawner.StartPhase(EnemySpawner.PhaseType.Normal, i);
            yield return new WaitForSeconds(normalPhaseDuration);
            enemySpawner.KillAllEnemies();
            yield return StartCoroutine(ShopPhaseRoutine());
        }

        phaseIndex = normalPhaseRepeat;
        currentPhaseDuration = 0f;
        bossPhaseEnded = false;
        enemySpawner.StartPhase(EnemySpawner.PhaseType.Boss);
        yield return new WaitUntil(() => bossPhaseEnded);
        enemySpawner.KillAllEnemies();
        yield return StartCoroutine(ShopPhaseRoutine());

        Debug.Log("ボスフェーズクリア");
    }

    public void CloseShop()
    {
        if (shopController == null) return;
        shopController.IsShopOpen = false;
        Time.timeScale = 1f;
        shopController.gameObject.SetActive(false);
    }

    IEnumerator ShopPhaseRoutine()
    {
        StartCoroutine(Pause());
        if (shopController == null) yield break;
        currentPhaseDuration = 0f;
        shopController.gameObject.SetActive(true);
        shopController.OpenShop();
        yield return new WaitUntil(() => !shopController.IsShopOpen);
    }

    public Vector3 ClampEnemyPosition(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, enemyMinX, enemyMaxX);
        pos.y = Mathf.Clamp(pos.y, enemyMinY, enemyMaxY);
        return pos;
    }

    public bool TrySpendStorage(int amount)
    {
        if (storage < amount) return false;
        storage -= amount;
        return true;
    }

    public void AddStorage(int amount)
    {
        storage += amount;
    }

    public void SetPhase(int index)
    {
        phaseIndex = index;
    }

    public void SetFieldStatus(FieldStatus status)
    {
        fieldStatus = status;
    }

    void OnEnemyDeath(string enemyName, int scoreValue)
    {
        string playerName = player != null ? player.gameObject.name : "Player";
        string log = $"{enemyName} killed by {playerName}";
        logs.Add(log);
        Debug.Log(log);
        score += scoreValue;
        storage += scoreValue;
    }

    void OnPlayerDeath()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            globalVolumeAnimator.SetTrigger("open");
        }

    }

    public void ActiveReplayButton()
    {
        replay_animator.SetTrigger("active");
        replay_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01.>replay";
        replay_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(0.9f, 0.9f, 1);
    }
    public void DeactiveReplayButton()
    {
        replay_animator.SetTrigger("deactive");
        replay_animator.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "01._replay";
        replay_animator.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1);
    }
    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }
}
