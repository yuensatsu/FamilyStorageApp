using System;
using UnityEngine;

/// <summary>
/// 制限時間を管理するクラスです。
/// Updateで残り時間を減らし、0になったらイベントで通知します。
/// </summary>
public class TimerManager : MonoBehaviour
{
    /// <summary>残り時間が変わった時に呼ばれます。UI更新に使えます。</summary>
    public event Action<float> OnTimeChanged;

    /// <summary>残り時間が0になった時に呼ばれます。</summary>
    public event Action OnTimeUp;

    [SerializeField]
    private float timeLimit = 60f;

    [SerializeField]
    private float remainingTime = 60f;

    private bool isRunning = false;

    private void Awake()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            StopTimer();
            Debug.Log("TimerManager: 制限時間になりました。");
            OnTimeChanged?.Invoke(remainingTime);
            OnTimeUp?.Invoke();
            return;
        }

        OnTimeChanged?.Invoke(remainingTime);
    }

    /// <summary>
    /// タイマーを開始します。
    /// </summary>
    public void StartTimer()
    {
        isRunning = true;
        Debug.Log("TimerManager: タイマーを開始しました。");
    }

    /// <summary>
    /// タイマーを停止します。
    /// </summary>
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("TimerManager: タイマーを停止しました。");
    }

    /// <summary>
    /// 残り時間を初期値に戻します。
    /// </summary>
    public void ResetTimer()
    {
        remainingTime = timeLimit;
        isRunning = false;
        Debug.Log($"TimerManager: タイマーをリセットしました。残り時間: {remainingTime}");
        OnTimeChanged?.Invoke(remainingTime);
    }

    /// <summary>
    /// 残り時間を取得します。
    /// </summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }

    /// <summary>
    /// 制限時間の初期値を取得します。
    /// </summary>
    public float GetTimeLimit()
    {
        return timeLimit;
    }

    /// <summary>
    /// 制限時間を変更したい時に使います。
    /// Inspectorから変更するだけなら通常は呼ばなくて大丈夫です。
    /// </summary>
    public void SetTimeLimit(float seconds)
    {
        if (seconds <= 0f)
        {
            Debug.LogWarning("TimerManager: 制限時間は0より大きい値にしてください。");
            return;
        }

        timeLimit = seconds;
        ResetTimer();
    }
}
