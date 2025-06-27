using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMap : MonoBehaviour
{
    [Header("大地图操作")]
    public GameObject bigMapCamera;
    public GameObject middleMapCamera;
    public GameObject MainCamera;
    public GameObject Player;

    [Header("相机移动速度")]
    public float cameraMoveSpeed = 5f;
    public float cameraZoomSpeed = 10f;

    [Header("相机设置")]
    public float cameraHeight = 20f;
    public float borderOffset = 60f;
    public float minZoom = 5f;
    public float maxZoom = 30f;

    private bool isBigMapOpen = false;
    private GameObject BigCamera;
    private Vector3 _mapCenter; // 迷宫中心点
    private Vector3 _targetPosition; // 目标位置

    private void Start()
    {
        isBigMapOpen = false;

        _mapCenter = new Vector3(
            PublicVar.Wmaze * 2,
            0,
            PublicVar.Hmaze * 2
        );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isBigMapOpen = !isBigMapOpen;
        }
        if (isBigMapOpen)
        {
            OpenBigMap();
        }
        else
        {
            CloseBigMap();
        }
    }

    void OpenBigMap()
    {
        if (BigCamera == null)
        {
            BigCamera = Instantiate(bigMapCamera);
            middleMapCamera.gameObject.SetActive(false);
            MainCamera.gameObject.SetActive(false);

            Vector3 startPos = new Vector3(
                Player.transform.position.x,
                cameraHeight,
                Player.transform.position.z
            );

            BigCamera.transform.position = startPos;
            BigCamera.transform.rotation = Quaternion.Euler(90, 0, 0);

            _targetPosition = startPos;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            PublicVar.IsMapState = true;
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            _targetPosition.y -= scrollInput * cameraZoomSpeed * Time.deltaTime * 10f;
            _targetPosition.y = Mathf.Clamp(_targetPosition.y, minZoom, maxZoom);
        }

        if (Input.GetMouseButton(0))
        {
            _targetPosition += new Vector3(
                -Input.GetAxis("Mouse X") * cameraMoveSpeed * Time.deltaTime,
                0,
                -Input.GetAxis("Mouse Y") * cameraMoveSpeed * Time.deltaTime
            );

            float halfWidth = PublicVar.Wmaze * 2; // 迷宫半宽
            float halfHeight = PublicVar.Hmaze * 2; // 迷宫半高

            _targetPosition.x = Mathf.Clamp(
                _targetPosition.x,
                _mapCenter.x - halfWidth + borderOffset,
                _mapCenter.x + halfWidth + borderOffset
            );

            _targetPosition.z = Mathf.Clamp(
                _targetPosition.z,
                _mapCenter.z - halfHeight + borderOffset,
                _mapCenter.z + halfHeight + borderOffset
            );
        }

        BigCamera.transform.position = Vector3.Lerp(
            BigCamera.transform.position,
            _targetPosition,
            Time.deltaTime * cameraMoveSpeed * 2f
        );

    }

    void CloseBigMap()
    {
        if (BigCamera != null)
        {
            middleMapCamera.gameObject.SetActive(true);
            MainCamera.gameObject.SetActive(true);
            Destroy(BigCamera);
            PublicVar.IsMapState = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}