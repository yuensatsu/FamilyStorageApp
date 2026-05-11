using UnityEngine;

namespace FamilyStorageApp.Managers
{
    /// <summary>
    /// タッチ入力とマウス入力を共通化するクラスです。
    /// PoiControllerなど、入力を使う側はこのクラスの値を見るだけで済みます。
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private Vector2 dragScreenPosition;
        private bool hasDragPosition;
        private bool isPointerDown;
        private bool isPointerHeld;
        private bool isPointerUp;

        public Vector2 DragScreenPosition => dragScreenPosition;
        public bool HasDragPosition => hasDragPosition;
        public bool IsPointerDown => isPointerDown;
        public bool IsPointerHeld => isPointerHeld;
        public bool IsPointerUp => isPointerUp;

        private void Update()
        {
            RefreshInput();
        }

        /// <summary>
        /// 現在フレームの入力状態を更新します。
        /// PoiController側からも呼べるようにして、Script Execution Orderに依存しにくくしています。
        /// </summary>
        public void RefreshInput()
        {
            ResetFrameInput();

            if (Input.touchCount > 0)
            {
                ReadTouchInput();
            }
            else
            {
                ReadMouseInput();
            }
        }

        private void ResetFrameInput()
        {
            hasDragPosition = false;
            isPointerDown = false;
            isPointerHeld = false;
            isPointerUp = false;
        }

        private void ReadTouchInput()
        {
            Touch touch = Input.GetTouch(0);

            dragScreenPosition = touch.position;
            hasDragPosition = true;

            isPointerDown = touch.phase == TouchPhase.Began;
            isPointerHeld = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            isPointerUp = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        }

        private void ReadMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragScreenPosition = Input.mousePosition;
                hasDragPosition = true;
                isPointerDown = true;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                dragScreenPosition = Input.mousePosition;
                hasDragPosition = true;
                isPointerHeld = true;
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                dragScreenPosition = Input.mousePosition;
                hasDragPosition = true;
                isPointerUp = true;
            }
        }
    }
}
