using FamilyStorageApp.Common;
using FamilyStorageApp.Managers;
using UnityEngine;

namespace FamilyStorageApp.GoldfishScoop
{
    /// <summary>
    /// プレイヤーが操作するポイの移動と、すくう判定を担当します。
    /// 入力はInputManagerから受け取り、Androidタッチとエディタのマウスを共通化します。
    /// </summary>
    public class PoiController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private Camera mainCamera;

        [SerializeField]
        private float poiHeight = 0.2f;

        [SerializeField]
        private float moveSpeed = 20f;

        [Header("Scoop")]
        [SerializeField]
        private float scoopRadius = 0.7f;

        [SerializeField]
        private LayerMask goldfishLayer = ~0;

        [SerializeField]
        private bool scoopOnPress = false;

        [SerializeField]
        private bool scoopOnRelease = true;

        [SerializeField]
        private bool reduceDurabilityOnMiss = false;

        [Header("References")]
        [SerializeField]
        private GoldfishGameManager gameManager;

        [SerializeField]
        private InputManager inputManager;

        private bool isDragging;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GoldfishGameManager>();
            }

            if (inputManager == null)
            {
                inputManager = FindObjectOfType<InputManager>();

                if (inputManager == null)
                {
                    inputManager = gameObject.AddComponent<InputManager>();
                    Debug.LogWarning("PoiController: InputManagerが見つからないため、ポイに自動追加しました。");
                }
            }
        }

        private void Update()
        {
            if (gameManager != null && gameManager.GetCurrentState() != GameState.Playing)
            {
                return;
            }

            HandleInput();
        }

        /// <summary>
        /// InputManagerの入力状態を見て、ポイ移動とすくうタイミングを処理します。
        /// </summary>
        private void HandleInput()
        {
            if (inputManager == null)
            {
                Debug.LogWarning("PoiController: InputManagerが設定されていません。");
                return;
            }

            inputManager.RefreshInput();

            if (inputManager.HasDragPosition)
            {
                MovePoiToScreenPosition(inputManager.DragScreenPosition);
            }

            if (inputManager.IsPointerDown)
            {
                isDragging = true;

                if (scoopOnPress)
                {
                    TryScoop();
                }
            }

            if (inputManager.IsPointerHeld && isDragging)
            {
                MovePoiToScreenPosition(inputManager.DragScreenPosition);
            }

            if (inputManager.IsPointerUp)
            {
                isDragging = false;

                if (scoopOnRelease)
                {
                    TryScoop();
                }
            }
        }

        /// <summary>
        /// 画面座標をワールド座標に変換して、ポイを移動します。
        /// </summary>
        private void MovePoiToScreenPosition(Vector2 screenPosition)
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("PoiController: Cameraが設定されていません。");
                return;
            }

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            Plane movePlane = new Plane(Vector3.up, new Vector3(0f, poiHeight, 0f));

            if (!movePlane.Raycast(ray, out float distance))
            {
                return;
            }

            Vector3 targetPosition = ray.GetPoint(distance);
            targetPosition.y = poiHeight;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        /// <summary>
        /// ポイの近くにいる金魚を探し、見つかったらすくいます。
        /// まずは一番近い金魚を1匹だけすくうシンプルな仕様にしています。
        /// </summary>
        private void TryScoop()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, scoopRadius, goldfishLayer);

            Goldfish nearestGoldfish = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                Goldfish goldfish = hit.GetComponentInParent<Goldfish>();
                if (goldfish == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, goldfish.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestGoldfish = goldfish;
                }
            }

            if (nearestGoldfish == null)
            {
                Debug.Log("PoiController: すくえる金魚が近くにいません。");

                if (reduceDurabilityOnMiss && gameManager != null)
                {
                    gameManager.ReducePoiDurability(1);
                }

                return;
            }

            Debug.Log("PoiController: 金魚をすくいます。");
            nearestGoldfish.Scoop();
        }

        /// <summary>
        /// Sceneビューでポイのすくえる範囲を見やすくするための表示です。
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, scoopRadius);
        }
    }
}
