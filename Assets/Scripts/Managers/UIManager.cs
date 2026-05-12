using System;
using UnityEngine;
using UnityEngine.UI;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// ゲーム中とリザルトの簡単なUI表示を担当します。
    /// TextやButtonはInspectorで割り当ててください。
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField]
        private Text scoreText;

        [SerializeField]
        private Text timeText;

        [SerializeField]
        private Text poiDurabilityText;

        [SerializeField]
        private Text resultText;

        [SerializeField]
        private Text highScoreText;

        [Header("Panels")]
        [SerializeField]
        private GameObject resultPanel;

        [Header("Buttons")]
        [SerializeField]
        private Button retryButton;

        [SerializeField]
        private Button shareButton;

        private void Awake()
        {
            HideResult();
        }

        public void UpdateScore(int score)
        {
            if (scoreText == null)
            {
                return;
            }

            scoreText.text = $"Score: {score}";
        }

        public void UpdateTime(float remainingTime)
        {
            if (timeText == null)
            {
                return;
            }

            timeText.text = $"Time: {Mathf.CeilToInt(remainingTime)}";
        }

        public void UpdatePoiDurability(int durability)
        {
            if (poiDurabilityText == null)
            {
                return;
            }

            poiDurabilityText.text = $"Poi: {durability}";
        }

        public void UpdateHighScore(int bestScore)
        {
            if (highScoreText == null)
            {
                return;
            }

            highScoreText.text = $"High Score: {bestScore}";
        }

        public void ShowResult(int scoopedCount, int score, bool isNewHighScore)
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(true);
            }

            if (resultText != null)
            {
                string newRecordText = isNewHighScore ? "\nハイスコア更新！" : string.Empty;
                resultText.text = $"結果\nすくった数: {scoopedCount}匹\nスコア: {score}点{newRecordText}";
            }
        }

        public void HideResult()
        {
            if (resultPanel != null)
            {
                resultPanel.SetActive(false);
            }
        }

        /// <summary>
        /// リトライボタンの処理を登録します。
        /// Inspectorから設定してもよいですが、コードから登録すると設定漏れを減らせます。
        /// </summary>
        public void BindRetryButton(Action onRetry)
        {
            if (retryButton == null || onRetry == null)
            {
                return;
            }

            retryButton.onClick.AddListener(() => onRetry.Invoke());
        }

        /// <summary>
        /// SNS共有ボタンの処理を登録します。
        /// </summary>
        public void BindShareButton(Action onShare)
        {
            if (shareButton == null || onShare == null)
            {
                return;
            }

            shareButton.onClick.AddListener(() => onShare.Invoke());
        }
    }
}
