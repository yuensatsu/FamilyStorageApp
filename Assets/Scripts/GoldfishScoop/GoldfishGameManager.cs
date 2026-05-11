using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 金魚すくい専用のゲーム進行を管理するクラスです。
/// スコア、タイマー、ポイ耐久値、保存、簡単なUI更新をまとめて扱います。
/// </summary>
public class GoldfishGameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField]
    private string gameId = "GoldfishScoop";

    [SerializeField]
    private int initialPoiDurability = 10;

    [SerializeField]
    private bool startOnAwake = true;

    [Header("Managers")]
    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private TimerManager timerManager;

    [SerializeField]
    private SaveManager saveManager;

    [SerializeField]
    private ShareManager shareManager;

    [Header("Simple UI (Optional)")]
    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text timeText;

    [SerializeField]
    private Text durabilityText;

    [SerializeField]
    private Text resultText;

    [SerializeField]
    private Text highScoreText;

    [SerializeField]
    private GameObject resultPanel;

    private GameState currentState = GameState.Ready;
    private int currentPoiDurability;
    private int scoopedCount;
    private bool isGameEnded;

    private void Awake()
    {
        FindManagersIfNeeded();

        if (timerManager != null)
        {
            timerManager.OnTimeUp += EndGame;
            timerManager.OnTimeChanged += HandleTimeChanged;
        }
    }

    private void Start()
    {
        if (startOnAwake)
        {
            StartGame();
        }
        else
        {
            PrepareGame();
        }
    }

    private void OnDestroy()
    {
        if (timerManager != null)
        {
            timerManager.OnTimeUp -= EndGame;
            timerManager.OnTimeChanged -= HandleTimeChanged;
        }
    }

    /// <summary>
    /// InspectorでManagerを入れ忘れても動きやすいように、シーン内から探します。
    /// 初心者向けプロトタイプなので、まずはこれで十分です。
    /// </summary>
    private void FindManagersIfNeeded()
    {
        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreManager>();
        }

        if (timerManager == null)
        {
            timerManager = FindObjectOfType<TimerManager>();
        }

        if (saveManager == null)
        {
            saveManager = FindObjectOfType<SaveManager>();
        }

        if (shareManager == null)
        {
            shareManager = FindObjectOfType<ShareManager>();
        }
    }

    /// <summary>
    /// ゲーム開始前の値に戻します。
    /// </summary>
    public void PrepareGame()
    {
        currentState = GameState.Ready;
        currentPoiDurability = initialPoiDurability;
        scoopedCount = 0;
        isGameEnded = false;

        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        if (timerManager != null)
        {
            timerManager.ResetTimer();
        }

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        UpdateSimpleUI();
        UpdateHighScoreUI();
        Debug.Log("GoldfishGameManager: ゲーム準備が完了しました。");
    }

    /// <summary>
    /// 金魚すくいを開始します。
    /// </summary>
    public void StartGame()
    {
        PrepareGame();
        currentState = GameState.Playing;

        if (saveManager != null)
        {
            saveManager.AddPlayCount(gameId);
        }

        if (timerManager != null)
        {
            timerManager.StartTimer();
        }
        else
        {
            Debug.LogWarning("GoldfishGameManager: TimerManagerが設定されていません。");
        }

        Debug.Log("GoldfishGameManager: ゲームを開始しました。");
    }

    /// <summary>
    /// 金魚をすくった時にGoldfishから呼ばれます。
    /// </summary>
    public void OnGoldfishScooped(Goldfish goldfish)
    {
        if (currentState != GameState.Playing || isGameEnded)
        {
            return;
        }

        if (goldfish == null)
        {
            Debug.LogWarning("GoldfishGameManager: goldfishがnullです。");
            return;
        }

        scoopedCount++;

        if (scoreManager != null)
        {
            scoreManager.AddScore(goldfish.Point);
        }

        ReducePoiDurability(1);
        UpdateSimpleUI();

        Debug.Log($"GoldfishGameManager: 金魚をすくいました。合計: {scoopedCount}匹");
    }

    /// <summary>
    /// ポイの耐久値を減らします。
    /// 耐久値が0になったらゲーム終了です。
    /// </summary>
    public void ReducePoiDurability(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentPoiDurability -= amount;
        currentPoiDurability = Mathf.Max(0, currentPoiDurability);
        Debug.Log($"GoldfishGameManager: ポイ耐久値が減りました。残り: {currentPoiDurability}");

        if (currentPoiDurability <= 0)
        {
            Debug.Log("GoldfishGameManager: ポイが破れました。");
            EndGame();
        }
    }

    /// <summary>
    /// ゲームを終了し、リザルト表示と保存を行います。
    /// </summary>
    public void EndGame()
    {
        if (isGameEnded)
        {
            return;
        }

        isGameEnded = true;
        currentState = GameState.Result;

        if (timerManager != null)
        {
            timerManager.StopTimer();
        }

        int finalScore = scoreManager != null ? scoreManager.GetScore() : 0;
        float elapsedTime = timerManager != null ? timerManager.GetTimeLimit() - timerManager.GetRemainingTime() : 0f;

        bool isNewHighScore = false;
        if (saveManager != null)
        {
            isNewHighScore = saveManager.SaveHighScoreIfNeeded(gameId, finalScore);

            // 今後タイムアタック要素を足しやすいように、今回の終了までの時間も保存対象にしています。
            if (elapsedTime > 0f)
            {
                saveManager.SaveBestTimeIfNeeded(gameId, elapsedTime);
            }
        }

        ShowResult(finalScore, isNewHighScore);
        Debug.Log($"GoldfishGameManager: ゲーム終了。スコア: {finalScore}");
    }

    /// <summary>
    /// SNS共有ボタンから呼ぶためのメソッドです。
    /// </summary>
    public void ShareResult()
    {
        if (shareManager == null)
        {
            Debug.LogWarning("GoldfishGameManager: ShareManagerが設定されていません。");
            return;
        }

        int finalScore = scoreManager != null ? scoreManager.GetScore() : 0;
        string message = shareManager.CreateGoldfishShareText(scoopedCount, finalScore);
        shareManager.ShareText(message);
    }

    /// <summary>
    /// 現在のゲーム状態を取得します。
    /// </summary>
    public GameState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 現在のポイ耐久値を取得します。
    /// </summary>
    public int GetCurrentPoiDurability()
    {
        return currentPoiDurability;
    }

    /// <summary>
    /// 現在のすくった数を取得します。
    /// </summary>
    public int GetScoopedCount()
    {
        return scoopedCount;
    }

    private void ShowResult(int finalScore, bool isNewHighScore)
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (resultText != null)
        {
            string newRecordText = isNewHighScore ? "\nハイスコア更新！" : string.Empty;
            resultText.text = $"結果\nすくった数: {scoopedCount}匹\nスコア: {finalScore}点{newRecordText}";
        }

        UpdateSimpleUI();
        UpdateHighScoreUI();
    }

    private void HandleTimeChanged(float remainingTime)
    {
        UpdateSimpleUI();
    }

    private void UpdateSimpleUI()
    {
        int score = scoreManager != null ? scoreManager.GetScore() : 0;
        float remainingTime = timerManager != null ? timerManager.GetRemainingTime() : 0f;

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (timeText != null)
        {
            timeText.text = $"Time: {Mathf.CeilToInt(remainingTime)}";
        }

        if (durabilityText != null)
        {
            durabilityText.text = $"Poi: {currentPoiDurability}";
        }
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null && saveManager != null)
        {
            GameRecord record = saveManager.LoadRecord(gameId);
            highScoreText.text = $"High Score: {record.bestScore}";
        }
    }
}
