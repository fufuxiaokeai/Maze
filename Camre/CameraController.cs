using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public float rotateSpeed = 5f;
    public float minPitch = -30f;
    public float maxPitch = 70f;
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -5f);

    [Header("Collision")]
    public LayerMask collisionMask;
    public float cameraRadius = 0.9f;
    public float minCameraDistance = 0.5f;
    public float adjustmentSpeed = 5f;

    [Header("Zoom")] // 新增缩放参数
    public float zoomSpeed = 3f;
    public float minZoom = -10f;
    public float maxZoom = -2f;

    private Transform _target;
    private float _yaw;
    private float _pitch;
    private Vector3 _currentOffset;

    void Start()
    {
        _target = transform.parent;
        _currentOffset = cameraOffset;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (PublicVar.Isconnect || PublicVar.IsEnter || PublicVar.Isdetails || PublicVar.IsEnd || PublicVar.IsAchievement || PublicVar.IsChart || PublicVar.IsRanked) 
        {
            return; // 如果UI已打开，直接返回，不处理按键逻辑
        }
        else
        {
            HandleZoom();      // 新增缩放处理
            HandleRotation();
            HandleCollision();
            ApplyPosition();
        }
    }

    // 新增滚轮缩放方法
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cameraOffset.z += scroll * zoomSpeed;
        cameraOffset.z = Mathf.Clamp(cameraOffset.z, minZoom, maxZoom);
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
            return;

        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed;

        _yaw += mouseX;
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    void HandleCollision()
    {
        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 desiredPosition = _target.position + targetRotation * cameraOffset;

        Vector3 rayStart = _target.position + Vector3.up * cameraOffset.y;
        Vector3 rayDirection = desiredPosition - rayStart;
        float rayDistance = rayDirection.magnitude;

        if (Physics.SphereCast(
            rayStart,
            cameraRadius,
            rayDirection.normalized,
            out RaycastHit hit,
            rayDistance,
            collisionMask))
        {
            float safeDistance = hit.distance - cameraRadius;
            _currentOffset.z = -Mathf.Clamp(
                safeDistance,
                minCameraDistance,
                Mathf.Abs(cameraOffset.z) // 使用当前缩放值作为最大距离
            );
        }
        else
        {
            _currentOffset.z = Mathf.Lerp(
                _currentOffset.z,
                cameraOffset.z,
                adjustmentSpeed * Time.deltaTime
            );
        }
    }

    void ApplyPosition()
    {
        Quaternion targetRotation = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 targetPosition = _target.position + targetRotation * _currentOffset;

        transform.rotation = targetRotation;
        transform.position = targetPosition;
    }
}