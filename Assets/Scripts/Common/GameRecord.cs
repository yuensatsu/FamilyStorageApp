using System;

/// <summary>
/// ゲームの自己記録をまとめて持つためのデータクラスです。
/// PlayerPrefsへ保存する値を、分かりやすく1つの形にまとめています。
/// </summary>
[Serializable]
public class GameRecord
{
    /// <summary>ゲームを識別するIDです。例: "GoldfishScoop"</summary>
    public string gameId;

    /// <summary>これまでの最高スコアです。</summary>
    public int bestScore;

    /// <summary>これまでの最速タイムです。タイムアタック要素を入れる時に使います。</summary>
    public float bestTime;

    /// <summary>このゲームを遊んだ回数です。</summary>
    public int playCount;

    /// <summary>最後に遊んだ日時です。文字列にしておくとPlayerPrefsへ保存しやすいです。</summary>
    public string lastPlayedAt;

    public GameRecord(string gameId)
    {
        this.gameId = gameId;
        bestScore = 0;
        bestTime = 0f;
        playCount = 0;
        lastPlayedAt = string.Empty;
    }
}
