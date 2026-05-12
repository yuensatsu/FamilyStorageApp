using UnityEngine;

namespace FamilyStorageApp.GoldfishScoop
{
    /// <summary>
    /// 金魚1匹の動きと、すくわれた時の処理を担当します。
    /// 見た目はSphereなどの仮オブジェクトでも動きます。
    /// </summary>
    public class Goldfish : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField]
        private int point = 10;

        [Header("Swimming")]
        [SerializeField]
        private float swimSpeed = 1.5f;

        [SerializeField]
        private float changeTargetInterval = 2f;

        [SerializeField]
        private Vector3 swimAreaCenter = Vector3.zero;

        [SerializeField]
        private Vector3 swimAreaSize = new Vector3(8f, 0f, 8f);

        [Header("References")]
        [SerializeField]
        private GoldfishGameManager gameManager;

        private Vector3 targetPosition;
        private float changeTargetTimer;
        private bool isScooped;

        /// <summary>
        /// 外部から点数を読むためのプロパティです。
        /// </summary>
        public int Point => point;

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GoldfishGameManager>();
            }

            ChooseNewTargetPosition();
        }

        private void Update()
        {
            if (isScooped)
            {
                return;
            }

            Swim();
        }

        /// <summary>
        /// 水槽内をゆっくりランダムに泳がせます。
        /// </summary>
        private void Swim()
        {
            changeTargetTimer += Time.deltaTime;

            if (changeTargetTimer >= changeTargetInterval)
            {
                ChooseNewTargetPosition();
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                swimSpeed * Time.deltaTime
            );

            Vector3 direction = targetPosition - transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized);
            }
        }

        /// <summary>
        /// 次に泳いでいく目標地点をランダムに決めます。
        /// </summary>
        private void ChooseNewTargetPosition()
        {
            changeTargetTimer = 0f;

            float halfX = swimAreaSize.x * 0.5f;
            float halfZ = swimAreaSize.z * 0.5f;

            targetPosition = new Vector3(
                Random.Range(swimAreaCenter.x - halfX, swimAreaCenter.x + halfX),
                transform.position.y,
                Random.Range(swimAreaCenter.z - halfZ, swimAreaCenter.z + halfZ)
            );
        }

        /// <summary>
        /// Spawnerから生成範囲を教えてもらうためのメソッドです。
        /// </summary>
        public void SetSwimArea(Vector3 center, Vector3 size)
        {
            swimAreaCenter = center;
            swimAreaSize = size;
            ChooseNewTargetPosition();
        }

        /// <summary>
        /// ポイですくわれた時に呼ばれます。
        /// </summary>
        public void Scoop()
        {
            if (isScooped)
            {
                return;
            }

            isScooped = true;
            Debug.Log($"Goldfish: {point}点の金魚がすくわれました。");

            if (gameManager != null)
            {
                gameManager.OnGoldfishScooped(this);
            }
            else
            {
                Debug.LogWarning("Goldfish: GoldfishGameManagerが見つかりません。");
            }

            Destroy(gameObject);
        }
    }
}
