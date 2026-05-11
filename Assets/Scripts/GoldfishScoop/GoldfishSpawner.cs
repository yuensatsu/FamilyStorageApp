using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 金魚を一定数生成し、減ったら追加するクラスです。
/// 生成範囲はInspectorで調整できます。
/// </summary>
public class GoldfishSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    private Goldfish goldfishPrefab;

    [SerializeField]
    private Transform goldfishParent;

    [Header("Spawn Settings")]
    [SerializeField]
    private int targetGoldfishCount = 10;

    [SerializeField]
    private float respawnInterval = 1f;

    [SerializeField]
    private Vector3 spawnAreaCenter = Vector3.zero;

    [SerializeField]
    private Vector3 spawnAreaSize = new Vector3(8f, 0f, 8f);

    private readonly List<Goldfish> spawnedGoldfish = new List<Goldfish>();
    private float respawnTimer;

    private void Start()
    {
        SpawnInitialGoldfish();
    }

    private void Update()
    {
        RemoveDestroyedGoldfishFromList();

        if (spawnedGoldfish.Count >= targetGoldfishCount)
        {
            return;
        }

        respawnTimer += Time.deltaTime;
        if (respawnTimer >= respawnInterval)
        {
            respawnTimer = 0f;
            SpawnOneGoldfish();
        }
    }

    /// <summary>
    /// ゲーム開始時に目標数まで金魚を生成します。
    /// </summary>
    private void SpawnInitialGoldfish()
    {
        for (int i = spawnedGoldfish.Count; i < targetGoldfishCount; i++)
        {
            SpawnOneGoldfish();
        }
    }

    /// <summary>
    /// 金魚を1匹生成します。
    /// </summary>
    private void SpawnOneGoldfish()
    {
        if (goldfishPrefab == null)
        {
            Debug.LogWarning("GoldfishSpawner: goldfishPrefabが設定されていません。");
            return;
        }

        Vector3 spawnPosition = GetRandomPositionInSpawnArea();
        Transform parent = goldfishParent != null ? goldfishParent : transform;
        Goldfish goldfish = Instantiate(goldfishPrefab, spawnPosition, Quaternion.identity, parent);
        goldfish.SetSwimArea(spawnAreaCenter, spawnAreaSize);
        spawnedGoldfish.Add(goldfish);

        Debug.Log("GoldfishSpawner: 金魚を生成しました。");
    }

    /// <summary>
    /// すくわれてDestroyされた金魚をリストから取り除きます。
    /// </summary>
    private void RemoveDestroyedGoldfishFromList()
    {
        for (int i = spawnedGoldfish.Count - 1; i >= 0; i--)
        {
            if (spawnedGoldfish[i] == null)
            {
                spawnedGoldfish.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 生成範囲内のランダムな位置を返します。
    /// </summary>
    private Vector3 GetRandomPositionInSpawnArea()
    {
        float halfX = spawnAreaSize.x * 0.5f;
        float halfZ = spawnAreaSize.z * 0.5f;

        return new Vector3(
            Random.Range(spawnAreaCenter.x - halfX, spawnAreaCenter.x + halfX),
            spawnAreaCenter.y,
            Random.Range(spawnAreaCenter.z - halfZ, spawnAreaCenter.z + halfZ)
        );
    }

    /// <summary>
    /// Sceneビューで生成範囲を見やすくするための表示です。
    /// ゲーム本編には影響しません。
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}
