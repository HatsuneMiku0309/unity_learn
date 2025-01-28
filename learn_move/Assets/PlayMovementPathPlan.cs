using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;

public class PlayMovementPathPlan : MonoBehaviour
{
    float moveStep = 1f;
    float playerHeight;
    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask layerMask;
    Vector3 targetPosition;
    Queue<Vector3> pathPlan;
    bool isMove = false;
    [SerializeField] float speed = 4f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathPlan = new Queue<Vector3>();
        playerHeight = transform.localScale.y;   
    }

    // Update is called once per frame
    void Update()
    {
        setTargetPosition();
        PathPlan();
        StartCoroutine(MoveTowardsTarget());
    }

    void CreateMarker(Vector3 position)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.5f; // 縮小標記物體
        Destroy(marker, 2f); // 2秒後自動銷毀
    }

    void setTargetPosition() {
        if (Input.GetMouseButton(0)) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
                pathPlan.Clear();
                CreateMarker(hit.point);
                targetPosition = hit.point;
                targetPosition.y = playerHeight;
                targetPosition.x = math.round(targetPosition.x);
                targetPosition.z = math.round(targetPosition.z);
            }
        }
    }

    void PathPlan() {
        // 移動過程中不計算新路徑是為了避免小數點路徑

        // 依據RO的移動，一定是到targetPosition後才會移動下一個，因為setTargetPosition清空了pathPlan，所以當前移動的為最後的targetPosition
        // 移動完成後計算鼠標點擊的下個路徑的路徑規劃
        if (pathPlan.Count == 0 && targetPosition != Vector3.zero && !isMove) {
            Vector3 lastPath = transform.position;
            Vector3 diffPosition = targetPosition - transform.position;
            float distanceX = diffPosition.x;
            float distanceZ = diffPosition.z;
            int maxStep = (int) math.abs(math.round((math.abs(distanceX) >= math.abs(distanceZ) ? distanceX: distanceZ) / moveStep));
            for (int i = 0; i < maxStep ; i++) {    
                float directionX = Mathf.Sign(targetPosition.x - transform.position.x);
                float directionZ = Mathf.Sign(targetPosition.z - transform.position.z);
                float nextX = Mathf.Approximately(lastPath.x, targetPosition.x) ? targetPosition.x : lastPath.x + (directionX * moveStep);
                float nextZ = Mathf.Approximately(lastPath.z, targetPosition.z) ? targetPosition.z : lastPath.z + (directionZ * moveStep);
                pathPlan.Enqueue(new Vector3(nextX, playerHeight, nextZ));
                lastPath.x = nextX;
                lastPath.z = nextZ;
            }
            targetPosition = Vector3.zero;
        }
    }

    IEnumerator MoveTowardsTarget() {
        if (!isMove && pathPlan.Count != 0) {
            Vector3 nextPosition = pathPlan.Dequeue();
            isMove = true;
            Debug.Log(nextPosition);

            while (Vector3.Distance(transform.position, nextPosition) >= 0.1f) {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
                yield return null;
            }
            transform.position = nextPosition;

            isMove = false;
        }
    }

    
}
