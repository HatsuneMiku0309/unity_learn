using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayMovementMouse : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask layerMask;

    bool isMoving;
    Vector3 targetPosition;

    float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            SetTargetPosition();
        }

        if (isMoving) {
            MoveTowardsTarget();
        }
    }

    void SetTargetPosition() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            CreateMarker(hit.point);
            targetPosition = hit.point;
            isMoving = true;
        }
    }
    void MoveTowardsTarget() {
        float floorHeight = checkPlayUnderGround();
        targetPosition.y = floorHeight + transform.localScale.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0; // 忽略垂直方向
        // 計算新的旋轉角度
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        // 平滑旋轉到目標角度
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            isMoving = false;
        }
    }

    float checkPlayUnderGround() {
        // 從角色的腳下向下發射射線
        Vector3 rayOrigin = transform.position + Vector3.up * 2f; // 稍微抬高射線起點，避免與地板交錯
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            float floorHeight = hit.point.y;
            // Debug.Log($"地板高度: {floorHeight}");
            return floorHeight;
        }
        else
        {
            // Debug.Log("角色腳下沒有檢測到地板！");
            return 0;
        }
    }

    void CreateMarker(Vector3 position)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.5f; // 縮小標記物體
        Destroy(marker, 2f); // 2秒後自動銷毀
    }
}
