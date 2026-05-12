using System;
using FamilyStorageApp.Common;
using UnityEngine;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// ゲーム全体の状態を管理するクラスです。
    /// まずはシンプルに、状態変更と基本操作だけを担当します。
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>状態が変わった時に呼ばれます。UIや各ゲーム専用Managerとの連携に使えます。</summary>
        public event Action<GameState> OnGameStateChanged;

        [SerializeField]
        private GameState currentState = GameState.Title;

        [SerializeField]
        private SceneLoadManager sceneLoadManager;

        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (sceneLoadManager == null)
            {
                sceneLoadManager = FindObjectOfType<SceneLoadManager>();
            }
        }

        /// <summary>
        /// ゲーム状態を変更します。
        /// 状態変更時はDebug.Logで流れが分かるようにしています。
        /// </summary>
        public void SetState(GameState nextState)
        {
            if (currentState == nextState)
            {
                return;
            }

            currentState = nextState;
            Debug.Log($"GameManager: 状態を変更しました。State: {currentState}");
            OnGameStateChanged?.Invoke(currentState);
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
        }

        public void EndGame()
        {
            SetState(GameState.Result);
        }

        public void PauseGame()
        {
            SetState(GameState.Pause);
            Time.timeScale = 0f;
            Debug.Log("GameManager: ポーズしました。");
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            SetState(GameState.Playing);
            Debug.Log("GameManager: ポーズを解除しました。");
        }

        public void RetryGame()
        {
            Time.timeScale = 1f;

            if (sceneLoadManager != null)
            {
                sceneLoadManager.RetryGoldfishScene();
            }
            else
            {
                Debug.LogWarning("GameManager: SceneLoadManagerが設定されていません。");
            }
        }
    }
}
