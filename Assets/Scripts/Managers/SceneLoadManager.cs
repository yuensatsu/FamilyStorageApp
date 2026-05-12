using UnityEngine;
using UnityEngine.SceneManagement;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// シーン遷移を担当するクラスです。
    /// シーン名は定数化して、入力ミスを減らしています。
    /// </summary>
    public class SceneLoadManager : MonoBehaviour
    {
        private const string TitleSceneName = "TitleScene";
        private const string GoldfishSceneName = "GoldfishScene";
        private const string ResultSceneName = "ResultScene";

        public void LoadTitleScene()
        {
            LoadScene(TitleSceneName);
        }

        public void LoadGoldfishScene()
        {
            LoadScene(GoldfishSceneName);
        }

        public void LoadResultScene()
        {
            LoadScene(ResultSceneName);
        }

        public void RetryGoldfishScene()
        {
            LoadGoldfishScene();
        }

        private void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("SceneLoadManager: シーン名が空です。");
                return;
            }

            Debug.Log($"SceneLoadManager: シーンを読み込みます。Scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
    }
}
