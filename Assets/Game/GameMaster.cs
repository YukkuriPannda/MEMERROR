using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

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
    [SerializeField] float normalPhaseDuration = 60f;
    [SerializeField] int normalPhaseRepeat = 3;
    public Animator globalVolumeAnimator;
    public Animator replay_animator;

    public UnityEvent<HPController> DeathEvent { get; } = new();

    public bool isMenuOpen { get; private set; }
    public int phaseIndex { get; private set; }
    bool bossPhaseEnded;
    public FieldStatus fieldStatus { get; private set; }
    public List<string> logs { get; private set; } = new();
    public int score { get; private set; }

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

    public void RegisterEnemy(HPController enemyHP)
    {
        string enemyName = enemyHP.gameObject.name;
        enemyHP.OnDeath += () => OnEnemyDeath(enemyName);
    }

    IEnumerator PhaseCycleRoutine()
    {
        for (int i = 0; i < normalPhaseRepeat; i++)
        {
            phaseIndex = i;
            enemySpawner.StartPhase(EnemySpawner.PhaseType.Normal, i);
            yield return new WaitForSeconds(normalPhaseDuration);
        }

        phaseIndex = normalPhaseRepeat;
        bossPhaseEnded = false;
        enemySpawner.StartPhase(EnemySpawner.PhaseType.Boss);
        yield return new WaitUntil(() => bossPhaseEnded);

        Debug.Log("ボスフェーズクリア");
    }

    public void SetPhase(int index)
    {
        phaseIndex = index;
    }

    public void SetFieldStatus(FieldStatus status)
    {
        fieldStatus = status;
    }

    void OnEnemyDeath(string enemyName)
    {
        string playerName = player != null ? player.gameObject.name : "Player";
        string log = $"{enemyName} killed by {playerName}";
        logs.Add(log);
        Debug.Log(log);
        score += 100;
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
