using System;
using UnityEngine;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// 現在のスコアを管理するクラスです。
    /// スコア加算・リセット・取得だけを担当します。
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        /// <summary>
        /// スコアが変わった時に呼ばれるイベントです。
        /// UI更新を別クラスで行いたい時に使えます。
        /// </summary>
        public event Action<int> OnScoreChanged;

        [SerializeField]
        private int currentScore = 0;

        /// <summary>
        /// スコアを加算します。
        /// pointが0以下の場合は何もしません。
        /// </summary>
        public void AddScore(int point)
        {
            if (point <= 0)
            {
                Debug.LogWarning("ScoreManager: 0以下の点数は加算しません。");
                return;
            }

            currentScore += point;
            Debug.Log($"ScoreManager: {point}点追加しました。現在スコア: {currentScore}");
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// スコアを0に戻します。
        /// リトライやゲーム開始時に呼びます。
        /// </summary>
        public void ResetScore()
        {
            currentScore = 0;
            Debug.Log("ScoreManager: スコアをリセットしました。");
            OnScoreChanged?.Invoke(currentScore);
        }

        /// <summary>
        /// 現在のスコアを取得します。
        /// </summary>
        public int GetScore()
        {
            return currentScore;
        }
    }
}
