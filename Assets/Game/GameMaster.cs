using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    public UnityEvent<HPController> DeathEvent { get; } = new();

    public bool isMenuOpen { get; private set; }
    public int phaseIndex { get; private set; }
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
    }

    public void RegisterEnemy(HPController enemyHP)
    {
        string enemyName = enemyHP.gameObject.name;
        enemyHP.OnDeath += () => OnEnemyDeath(enemyName);
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
            gameOverCanvas.SetActive(true);
    }
}
