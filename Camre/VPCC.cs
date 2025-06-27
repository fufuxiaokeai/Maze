using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VPCC : MonoBehaviour
{
    [Header("速度控制")]
    public float moveSpeed = 10f;
    public float lookSpeed = 3f;
    public float verticalSpeed = 13f;

    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        if(!PublicVar.IsOnlineEnd)
            transform.position = PublicVar.PlayerOrientation + new Vector3(0, 4f, 0);
        Vector3 currentRotation = transform.eulerAngles;
        _rotationX = currentRotation.y;
        _rotationY = currentRotation.x;
    }

    void Update()
    {
        if (PublicVar.IsOnlineEnd) return;
        // 鼠标控制相机朝向
        HandleRotation();

        // 键盘控制移动
        HandleMovement();
    }

    private void HandleRotation()
    {
        _rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        _rotationY -= Input.GetAxis("Mouse Y") * lookSpeed;
        _rotationY = Mathf.Clamp(_rotationY, -90f, 90f);

        transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;

        if (Input.GetKey(KeyCode.Space)) moveDirection += transform.up;
        if (Input.GetKey(KeyCode.LeftControl)) moveDirection -= transform.up;
        if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = verticalSpeed;
        else moveSpeed = 10f;

        // 应用移动
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

}