using System;
using FamilyStorageApp.Common;
using UnityEngine;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// PlayerPrefsを使ってローカル保存を行うクラスです。
    /// 初学者にも分かりやすいように、保存キーは定数化しています。
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        // PlayerPrefsのキー名です。
        // gameIdを後ろに付けることで、今後ほかのミニゲームを追加しても記録を分けられます。
        private const string HighScoreKey = "HighScore";
        private const string BestTimeKey = "BestTime";
        private const string PlayCountKey = "PlayCount";
        private const string LastPlayedAtKey = "LastPlayedAt";

        /// <summary>
        /// 指定したゲームID用の保存キーを作ります。
        /// 例: "GoldfishScoop_HighScore"
        /// </summary>
        private string BuildKey(string gameId, string keyName)
        {
            return $"{gameId}_{keyName}";
        }

        /// <summary>
        /// 保存済みの記録を読み込みます。
        /// まだ保存されていない場合は初期値の記録を返します。
        /// </summary>
        public GameRecord LoadRecord(string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogWarning("SaveManager: gameIdが空なので記録を読み込めません。");
                return new GameRecord("Unknown");
            }

            GameRecord record = new GameRecord(gameId)
            {
                bestScore = PlayerPrefs.GetInt(BuildKey(gameId, HighScoreKey), 0),
                bestTime = PlayerPrefs.GetFloat(BuildKey(gameId, BestTimeKey), 0f),
                playCount = PlayerPrefs.GetInt(BuildKey(gameId, PlayCountKey), 0),
                lastPlayedAt = PlayerPrefs.GetString(BuildKey(gameId, LastPlayedAtKey), string.Empty)
            };

            Debug.Log($"SaveManager: 記録を読み込みました。GameId: {gameId}, BestScore: {record.bestScore}");
            return record;
        }

        /// <summary>
        /// GameRecordの内容をまとめて保存します。
        /// </summary>
        public void SaveRecord(GameRecord record)
        {
            if (record == null)
            {
                Debug.LogWarning("SaveManager: 保存する記録がnullです。");
                return;
            }

            if (string.IsNullOrEmpty(record.gameId))
            {
                Debug.LogWarning("SaveManager: gameIdが空なので記録を保存できません。");
                return;
            }

            PlayerPrefs.SetInt(BuildKey(record.gameId, HighScoreKey), record.bestScore);
            PlayerPrefs.SetFloat(BuildKey(record.gameId, BestTimeKey), record.bestTime);
            PlayerPrefs.SetInt(BuildKey(record.gameId, PlayCountKey), record.playCount);
            PlayerPrefs.SetString(BuildKey(record.gameId, LastPlayedAtKey), record.lastPlayedAt);
            PlayerPrefs.Save();

            Debug.Log($"SaveManager: 記録を保存しました。GameId: {record.gameId}");
        }

        /// <summary>
        /// 今回のスコアがハイスコアなら保存します。
        /// 戻り値がtrueなら新記録です。
        /// </summary>
        public bool SaveHighScoreIfNeeded(string gameId, int score)
        {
            GameRecord record = LoadRecord(gameId);

            if (score <= record.bestScore)
            {
                Debug.Log("SaveManager: ハイスコア更新はありません。");
                return false;
            }

            record.bestScore = score;
            SaveRecord(record);
            Debug.Log($"SaveManager: ハイスコアを更新しました。NewBestScore: {score}");
            return true;
        }

        /// <summary>
        /// 今回のタイムが最速なら保存します。
        /// bestTimeが0の場合は、まだ記録がないものとして保存します。
        /// </summary>
        public bool SaveBestTimeIfNeeded(string gameId, float time)
        {
            if (time <= 0f)
            {
                Debug.LogWarning("SaveManager: タイムは0より大きい値を指定してください。");
                return false;
            }

            GameRecord record = LoadRecord(gameId);

            if (record.bestTime > 0f && time >= record.bestTime)
            {
                Debug.Log("SaveManager: 最速タイム更新はありません。");
                return false;
            }

            record.bestTime = time;
            SaveRecord(record);
            Debug.Log($"SaveManager: 最速タイムを更新しました。NewBestTime: {time}");
            return true;
        }

        /// <summary>
        /// プレイ回数を1増やし、最終プレイ日時も更新します。
        /// </summary>
        public void AddPlayCount(string gameId)
        {
            GameRecord record = LoadRecord(gameId);
            record.playCount++;
            record.lastPlayedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SaveRecord(record);

            Debug.Log($"SaveManager: プレイ回数を更新しました。PlayCount: {record.playCount}");
        }

        /// <summary>
        /// 開発中に保存データを消したい時に使います。
        /// 本番ではデバッグメニューなどから呼ぶ想定です。
        /// </summary>
        public void DeleteRecord(string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogWarning("SaveManager: gameIdが空なので記録を削除できません。");
                return;
            }

            PlayerPrefs.DeleteKey(BuildKey(gameId, HighScoreKey));
            PlayerPrefs.DeleteKey(BuildKey(gameId, BestTimeKey));
            PlayerPrefs.DeleteKey(BuildKey(gameId, PlayCountKey));
            PlayerPrefs.DeleteKey(BuildKey(gameId, LastPlayedAtKey));
            PlayerPrefs.Save();

            Debug.Log($"SaveManager: 記録を削除しました。GameId: {gameId}");
        }
    }
}
