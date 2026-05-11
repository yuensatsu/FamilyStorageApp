using UnityEngine;

/// <summary>
/// SNS共有を担当するクラスです。
/// Android実機では共有Intentを開き、UnityエディタではDebug.Logに共有文を表示します。
/// </summary>
public class ShareManager : MonoBehaviour
{
    /// <summary>
    /// 金魚すくい用の共有文を作ります。
    /// </summary>
    public string CreateGoldfishShareText(int scoopedCount, int score)
    {
        return $"金魚すくい3Dで{scoopedCount}匹すくいました！\n" +
               $"スコア：{score}点\n" +
               "#お祭りミニゲーム #金魚すくい";
    }

    /// <summary>
    /// 共有を実行します。
    /// Android以外ではDebug.Logだけにして、エディタでも安全に動くようにしています。
    /// </summary>
    public void ShareText(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("ShareManager: 共有文が空です。");
            return;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
        using (AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent"))
        {
            string actionSend = intentClass.GetStatic<string>("ACTION_SEND");
            string extraText = intentClass.GetStatic<string>("EXTRA_TEXT");

            intentObject.Call<AndroidJavaObject>("setAction", actionSend);
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", extraText, message);

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "結果を共有"))
            {
                currentActivity.Call("startActivity", chooser);
            }
        }
#else
        Debug.Log($"ShareManager: エディタでは共有Intentを開かず、共有文を表示します。\n{message}");
#endif
    }
}
