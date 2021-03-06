using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REE.Camera
{
    public class Camera : MonoBehaviour
    {
        private static Camera singleton;
        public static Camera Singleton => singleton;

        public event EventHandler<float> OnCameraZoomed;

        public Transform cameraTransform;
        public Transform cameraMoveRig;
        Vector3 cameraStartPosition;

        [Header("Speed Settings")]
        public AnimationCurve cameraCloseCurve = DefaultCameraCloseCurve();
        public float lerpSpeed = 5;
        public float moveSpeed = 2;
        public float rotateSpeed = 2;
        public float zoomSpeed = 4;
        public float mouseZoomMultiplier = 5;
        [Range(0, 1)]
        public float borderMovementEffectRange = 0.01f;
        float borderMovementThickness;
        Vector3 zoomVector;

        private float CameraCloseMultiplier
        {
            get
            {
                if (!evaluateCameraCloseMultiplier)
                    return 1;

                float zoom = (cameraTransform.position.y - minPosition.y) / (maxPosition.y - minPosition.y);
                return cameraCloseCurve.Evaluate(zoom);
            }
        }

        public Vector3 newPos;
        public Vector3 newZoom;

        public float horizontalAngel;
        public float verticalAngel;

        Vector3 rotStartPos;
        Vector3 rotCurrentPos;

        [Header("Camera Limits")]
        public Vector3 minPosition = new Vector3(-50, 10, -50);
        public Vector3 maxPosition = new Vector3(50, 100, 50);
        [Range(0, 360)]
        public float minVerticalAngle = 280;
        [Range(0, 360)]
        public float maxVerticalAngle = 358;
        [Range(-360, 360)]
        public float minHorizontalAngle = 0;
        [Range(-360, 360)]
        public float maxHorizontalAngle = 0;
        public bool limitVerticalRotation = true;
        public bool limitHorizontalRotation = false;


        [Header("Functionality Settings")]
        public bool useBorderMovement = true;
        public bool cameraCanMove = true;
        public bool cameraCanZoom = true;
        public bool cameraCanRotateHorizontally = true;
        public bool cameraCanRotateVertically = true;
        public bool evaluateCameraCloseMultiplier = true;

        public bool verticalRotationResetOnButtonRelease = false;
        public bool horizontalRotationResetOnButtonRelease = false;

        public bool ignoreAllInput = false;

        private void Awake()
        {
            if (!singleton)
            {
                singleton = this;
            }
            else return;
        }

        public void Start()
        {
            moveSpeed *= 20;
            rotateSpeed *= 100;
            zoomSpeed *= 20;
            mouseZoomMultiplier *= 20;

            cameraCloseCurve = DefaultCameraCloseCurve();
            cameraStartPosition = cameraMoveRig.localPosition;

            borderMovementThickness = Screen.height * borderMovementEffectRange;

            zoomVector = new Vector3(0, -zoomSpeed, 0);
            newZoom = cameraTransform.localPosition;
            newPos = cameraMoveRig.localPosition;

            horizontalAngel = cameraMoveRig.localRotation.eulerAngles.y;
            verticalAngel = transform.localRotation.eulerAngles.x;

            Map.MapManager.Instance.OnMapLoaded += HandleMapLoaded;
            Map.District.OnDoubleClicked += HandleDistrictDoubleClicked;
            Map.Property.OnDoubleClicked += HandlePropertyDoubleClicked;

            Console.UI.OnConsoleFocused += (sender, args) => { ignoreAllInput = true; };
            Console.UI.OnConsoleDefocused += (sender, args) => { ignoreAllInput = false; };
        }

        private void OnDestroy()
        {
            Map.MapManager.Instance.OnMapLoaded -= HandleMapLoaded;
            Map.District.OnDoubleClicked -= HandleDistrictDoubleClicked;
            Map.Property.OnDoubleClicked -= HandlePropertyDoubleClicked;
        }

        void Update()
        {
            if (!ignoreAllInput)
            {
                MouseInput();
                KeyboardInput();
            }
            ClampPosition();
        }

        void FixedUpdate()
        {
            Move();
            Rotate();
        }

        public void MouseInput()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (cameraCanZoom && Input.mouseScrollDelta.y != 0)
                ZoomInput(Input.mouseScrollDelta.y * zoomVector * mouseZoomMultiplier * CameraCloseMultiplier * UnityEngine.Time.deltaTime);

            if ((cameraCanRotateVertically || cameraCanRotateHorizontally) && Input.GetMouseButtonDown(2))
                rotStartPos = Input.mousePosition;

            if ((cameraCanRotateVertically || cameraCanRotateHorizontally) && Input.GetMouseButton(2))
            {
                rotCurrentPos = Input.mousePosition;
                Vector3 direction = rotCurrentPos - rotStartPos;
                rotStartPos = rotCurrentPos;

                if (cameraCanRotateVertically)
                    RotateInput(verticalAngel, Vector3.right, -direction.y * 20 * UnityEngine.Time.deltaTime);
                if (cameraCanRotateHorizontally)
                    RotateInput(horizontalAngel, Vector3.up, -direction.x * 20 * UnityEngine.Time.deltaTime);
            }

            if (useBorderMovement && cameraCanMove)
            {
                if (Input.mousePosition.y >= Screen.height - borderMovementThickness)
                    MoveInput(cameraMoveRig.forward * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.mousePosition.y <= borderMovementThickness)
                    MoveInput(-cameraMoveRig.forward * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.mousePosition.x >= Screen.width - borderMovementThickness)
                    MoveInput(cameraMoveRig.right * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.mousePosition.x <= borderMovementThickness)
                    MoveInput(-cameraMoveRig.right * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
            }
        }

        public void KeyboardInput()
        {
            if (cameraCanMove)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    MoveInput(cameraMoveRig.forward * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    MoveInput(-cameraMoveRig.forward * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    MoveInput(cameraMoveRig.right * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    MoveInput(-cameraMoveRig.right * moveSpeed * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
            }

            if (cameraCanRotateVertically)
            {
                if (Input.GetKey(KeyCode.T))
                    RotateInput(verticalAngel, Vector3.right, rotateSpeed * UnityEngine.Time.deltaTime);
                else if (Input.GetKey(KeyCode.G))
                    RotateInput(verticalAngel, Vector3.right, -rotateSpeed * UnityEngine.Time.deltaTime);
                else if (verticalRotationResetOnButtonRelease)
                    verticalAngel = minVerticalAngle;
                    
            }
            if (cameraCanRotateHorizontally)
            {
                if (Input.GetKey(KeyCode.Q))
                    RotateInput(horizontalAngel, Vector3.up, rotateSpeed * UnityEngine.Time.deltaTime);
                else if (Input.GetKey(KeyCode.E))
                    RotateInput(horizontalAngel, Vector3.up, -rotateSpeed * UnityEngine.Time.deltaTime);
                else if (horizontalRotationResetOnButtonRelease)
                    horizontalAngel = 0;
            }

            if (cameraCanZoom && Input.GetKey(KeyCode.R))
                ZoomInput(zoomVector * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
            if (cameraCanZoom && Input.GetKey(KeyCode.F))
                ZoomInput(-zoomVector * CameraCloseMultiplier * UnityEngine.Time.deltaTime);
        }

        public void ZoomInput(Vector3 value)
        {
            newZoom += value;
        }

        public void MoveInput(Vector3 value)
        {
            newPos += value;
        }

        public void RotateInput(float angle, Vector3 axis, float value)
        {
            if (axis != Vector3.up && axis != Vector3.right)
            {
                UnityEngine.Debug.LogError("Expected axis to be Vector3.up or Vector3.right but it was: " + axis);
                return;
            }

            float newAngle = value + angle;
            if (limitVerticalRotation || limitHorizontalRotation)
            {
                newAngle = LimitCameraAngle(axis, newAngle);
            }

            if (axis == Vector3.right)
                verticalAngel = newAngle;
            if (axis == Vector3.up)
                horizontalAngel = newAngle;
        }

        public float LimitCameraAngle(Vector3 axis, float angle)
        {
            if (axis == Vector3.right && limitVerticalRotation)
                angle = Mathf.Clamp(angle, minVerticalAngle, maxVerticalAngle);
            if (axis == Vector3.up && limitHorizontalRotation)
                angle = Mathf.Clamp(angle, minHorizontalAngle, maxHorizontalAngle);
            return angle;
        }

        public void ClampPosition()
        {
            if (cameraCanMove)
                newPos = new Vector3(
                    Mathf.Clamp(newPos.x, cameraStartPosition.x + minPosition.x, cameraStartPosition.x + maxPosition.x),
                    newPos.y,
                    Mathf.Clamp(newPos.z, cameraStartPosition.z + minPosition.z, cameraStartPosition.z + maxPosition.z));

            if (cameraCanZoom)
                newZoom = new Vector3(0, Mathf.Clamp(newZoom.y, minPosition.y, maxPosition.y), 0);
        }

        public void Move()
        {
            if (cameraCanMove)
                LerpMove(cameraMoveRig, newPos, UnityEngine.Time.fixedDeltaTime * lerpSpeed);
            if (cameraCanZoom)
            {
                LerpMove(cameraTransform, newZoom, UnityEngine.Time.fixedDeltaTime * lerpSpeed);

                float zoom = (cameraTransform.position.y - minPosition.y) / (maxPosition.y - minPosition.y);
                OnCameraZoomed?.Invoke(this, zoom);
            }
        }

        private void LerpMove(Transform transform, Vector3 pos, float lerpValue)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pos, lerpValue);
        }

        public void Rotate()
        {
            if (cameraCanRotateVertically)
                LerpRotate(transform, Quaternion.Euler(verticalAngel, 0, 0), UnityEngine.Time.fixedDeltaTime * lerpSpeed);
            if (cameraCanRotateHorizontally)
                LerpRotate(cameraMoveRig, Quaternion.Euler(0, horizontalAngel, 0), UnityEngine.Time.fixedDeltaTime * lerpSpeed);
        }

        private void LerpRotate(Transform transform, Quaternion quaternion, float lerpValue)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, quaternion, lerpValue);
        }

        private static AnimationCurve DefaultCameraCloseCurve()
        {
            float rad = Mathf.Deg2Rad * 82;
            AnimationCurve curve = new AnimationCurve();
            curve.AddKey(new Keyframe(0, 0.32f));
            curve.AddKey(new Keyframe(1, 1, rad, rad));
            return curve;
        }
    
        public void HandleMapLoaded(object sender, Transform transform)
        {
            Transform bounds = transform.Find("Bounds");
            if (bounds == null) {
                Console.Console.Run("log Camera couldn't set bounds.\n      HandleMapLoaded returned early.");
                return;
            }

            Vector3 size = bounds.localScale;

            minPosition = -size/2;
            maxPosition = size/2;

            minPosition.y = size.y/20f;
            maxPosition.y = size.y;
        }

        private void HandleDistrictDoubleClicked(object sender, Map.District district)
        {
            MoveInput(district.Center - newPos);
        }
    
        private void HandlePropertyDoubleClicked(object sender, Map.Property property)
        {
            MoveInput(property.transform.position - newPos);
            newZoom = new Vector3(0, minPosition.y*2, 0);
        }
    }
}
