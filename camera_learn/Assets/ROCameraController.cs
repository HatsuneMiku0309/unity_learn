using Unity.Mathematics;
using UnityEngine;

public class ROCameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float distance = 10f;
    [SerializeField] float zoomSpeed = 5f;
    [SerializeField] float rotationSpeed = 1000f;
    [SerializeField] float smoothSpeed = 5f;
    [SerializeField] float minDistance = 2f;
    [SerializeField] float maxDistance = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    readonly float defaultYaw = 0f;
    [SerializeField] float yaw = 0f;

    readonly float defaultPitch = 60f;
    [SerializeField] float pitch = 60f;
    [SerializeField] float minPitch = 0f;
    [SerializeField] float maxPitch = 90f;

    float targetDistance;
    float doubleClickThreshold = 0.3f;
    float lastClickTime = -1f;
    void Start()
    {
        targetDistance = distance;
        pitch = defaultPitch;
        yaw = defaultYaw;
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift)) {
            RotateX();
            ResetRotateX();
        } else {
            RotateY();
            ResetRotateY();
            Scale();
        }

        updateCameraPosition();
    }

    void updateCameraPosition() {
        distance = Mathf.Lerp(distance, targetDistance, smoothSpeed * Time.deltaTime);
        Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(pitch, yaw, 0), smoothSpeed * Time.deltaTime);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    void RotateX() {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheel != 0f) {
            pitch += mouseScrollWheel * rotationSpeed * Time.deltaTime * 20;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
    }

    void ResetRotateX() {
        if (Input.GetMouseButtonDown(1)) {
            float currentTime = Time.time;
            if (currentTime - lastClickTime <= doubleClickThreshold){
                pitch = defaultPitch;
            }
            lastClickTime = currentTime;
        }
    }

    void RotateY() {
        if (Input.GetMouseButton(1)) {
            float mouseX = Input.GetAxis("Mouse X");
            yaw += mouseX * rotationSpeed * Time.deltaTime;
            // 避免一直轉數值越來越大
            yaw %= 360;
            
            // -180 ~ 180度
            float yawOffset = math.abs(yaw) - 180;
            yaw = yaw > 180 ? -(yaw - yawOffset) : yaw < -180 ? -(yaw + yawOffset) : yaw;
        }
    }

    void ResetRotateY() {
        if (Input.GetMouseButtonDown(1)) {
            float currentTime = Time.time;
            if (currentTime - lastClickTime <= doubleClickThreshold) {
                yaw = defaultYaw;
            }
            lastClickTime = currentTime;
        }
    }

    void Scale() {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScrollWheel != 0f) {
            targetDistance -= mouseScrollWheel * zoomSpeed;
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
    }
}
