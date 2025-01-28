using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] Transform target;       // 目標物體
    [SerializeField] float distance = 10f;   // 相機距離
    [SerializeField] float zoomSpeed = 5f;   // 縮放速度
    [SerializeField] float rotationSpeed = 1000f; // 旋轉速度
    [SerializeField] float smoothSpeed = 5f; // 平滑速度（旋轉和縮放）
    [SerializeField] float minDistance = 2f; // 最小縮放距離
    [SerializeField] float maxDistance = 20f; // 最大縮放距離

    float yaw = 0f;        // Y 軸旋轉角度
    float pitch = 60f;     // X 軸旋轉角度（初始角度為 60 度）
    float targetDistance;  // 瞄準的縮放距離
    string currentAnim;

    [SerializeField] float doubleClickThreshold = 0.3f; // 雙擊的最大時間間隔（秒）
    float lastClickTime = -1f; // 上次點擊的時間

    void Start()
    {
        currentAnim = "isIdle0";
        // 初始化縮放距離
        targetDistance = distance;
        Debug.Log(new Vector3(10, 0, -10).normalized);

        StartCoroutine(WaitForTarget());
    }

    IEnumerator WaitForTarget()
    {
        // 等待直到 target 被加載並存在
        while (target == null)
        {
            yield return null; // 等待下一幀
        }

        // target 已經加載完成，執行接下來的邏輯
        InitializeCamera();
    }

    void InitializeCamera() {
        gameObject.SetActive(false);
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position);
        gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Input.GetKey(KeyCode.LeftShift)) {
            RotateX();
            if (Input.GetMouseButtonDown(1)) {
                float currentTime = Time.time;
                if (currentTime - lastClickTime <= doubleClickThreshold)
                {
                    ResetXAngle();
                }

                lastClickTime = currentTime;
            }
        } else {
            if (Input.GetMouseButtonDown(1)) {
                float currentTime = Time.time;
                if (currentTime - lastClickTime <= doubleClickThreshold)
                {
                    ResetYAngle();
                }

                lastClickTime = currentTime;
            }
            if (Input.GetMouseButton(1)) {
                RotateY();
            }
            Scale();
        }

        Vector3 offset = CalcOffset();
        UpdateCameraPosition(offset);
        UpdateSpriteXRotation();
        changeTargetAnimation();
    }

    Vector3 CalcOffset() {
        // 平滑變化距離變化
        distance = Mathf.Lerp(distance, targetDistance, smoothSpeed * Time.deltaTime);

        // 根據球坐標計算相機位置，並且平滑變化
        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(pitch, yaw, 0), smoothSpeed * Time.deltaTime); // 計算旋轉
        // 根據距離計算偏移量
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        return offset;
    }

    void UpdateSpriteXRotation() {
        Quaternion rotation = Quaternion.Lerp(target.rotation, Quaternion.Euler(pitch, yaw, 0), smoothSpeed * Time.deltaTime); // 計算旋轉
        target.rotation = rotation;
    }

    void UpdateCameraPosition(Vector3 offset) {
        

        // 更新相機位置和旋轉
        transform.position = target.position + offset; // 相機位置
        transform.LookAt(target.position);

        // // 使用 LookRotation 對準目標
        // Vector3 direction = target.position - transform.position; // 計算指向目標的方向
        // transform.rotation = Quaternion.LookRotation(direction, Vector3.up); // 自定義 up 向量
        // // transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothSpeed * Time.deltaTime);
    }

    void RotateX() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            pitch += Input.GetAxis("Mouse ScrollWheel") * rotationSpeed * Time.deltaTime * 20; // 垂直旋轉
            pitch = Mathf.Clamp(pitch, 15f, 89f); // 限制垂直旋轉角度
        }
    }

    void ResetXAngle() {
        pitch = 60f;
    }

    void Scale() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            targetDistance -= scroll * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
    }

    void RotateY() {
        float mouseX = Input.GetAxis("Mouse X");
        yaw += mouseX * rotationSpeed * Time.deltaTime; // 水平旋轉
        yaw %= 360;
        yaw = yaw >= 180 ? -yaw : yaw < -180 ? -yaw : yaw;
    }

    void ResetYAngle() {
        yaw = 0f;
    }

    void changeTargetAnimation() {
        float pitchRange = 360f / 8f;
        float halfPitchRange = pitchRange / 2f;
        Animator anim = target.Find("body").GetComponent<Animator>();
        Vector3 scale = target.localScale;
        anim.ResetTrigger("isIdle0");
        anim.ResetTrigger("isIdle1");
        anim.ResetTrigger("isIdle2");
        anim.ResetTrigger("isIdle3");
        anim.ResetTrigger("isIdle4");
        if (yaw >= -halfPitchRange && yaw <= halfPitchRange && currentAnim != "isIdle0") {
            Debug.Log("isIdle0");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle0";
            anim.SetTrigger("isIdle0");
        } else if (yaw > halfPitchRange && yaw < halfPitchRange + pitchRange && currentAnim != "isIdle1") {
            Debug.Log("isIdle1");
            scale.x = scale.x > 0 ? -scale.x : scale.x;
            currentAnim = "isIdle1";
            anim.SetTrigger("isIdle1");
        } else if (yaw >= halfPitchRange + pitchRange && yaw <= halfPitchRange + (2 * pitchRange) && currentAnim != "isIdle2") {
            Debug.Log("isIdle2");
            scale.x = scale.x > 0 ? -scale.x : scale.x;
            currentAnim = "isIdle2";
            anim.SetTrigger("isIdle2");
        } else if (yaw > halfPitchRange + (2 * pitchRange) && yaw < halfPitchRange + (3 * pitchRange) && currentAnim != "isIdle3") {
            Debug.Log("isIdle3");
            scale.x = scale.x > 0 ? -scale.x : scale.x;
            currentAnim = "isIdle3";
            anim.SetTrigger("isIdle3");
        } else if (yaw >= halfPitchRange + (3 * pitchRange) && yaw <= halfPitchRange + (4 * pitchRange) && currentAnim != "isIdle4") {
            Debug.Log("isIdle4");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle4";
            anim.SetTrigger("isIdle4");
        } else if (yaw < -halfPitchRange && yaw > -halfPitchRange - pitchRange && currentAnim != "isIdle1") {
            Debug.Log("-isIdle1");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle1";
            anim.SetTrigger("isIdle1");
        } else if (yaw <= -halfPitchRange - pitchRange && yaw >= -halfPitchRange - (2 * pitchRange) && currentAnim != "isIdle2") {
            Debug.Log("-isIdle2");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle2";
            anim.SetTrigger("isIdle2");
        } else if (yaw < -halfPitchRange - (2 * pitchRange) && yaw > -halfPitchRange - (3 * pitchRange) && currentAnim != "isIdle3") {
            Debug.Log("-isIdle3");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle3";
            anim.SetTrigger("isIdle3");
        } else if (yaw <= -halfPitchRange - (3 * pitchRange) && yaw >= -halfPitchRange - (4 * pitchRange) && currentAnim != "isIdle4") {
            Debug.Log("isIdle4");
            scale.x = scale.x > 0 ? scale.x : -scale.x;
            currentAnim = "isIdle4";
            anim.SetTrigger("isIdle4");
        }

        target.localScale = scale;
    }
}
