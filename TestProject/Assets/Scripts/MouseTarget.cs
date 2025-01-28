using System;
using UnityEngine;

public class MouseTarget : MonoBehaviour
{
    public Camera mainCamera;  // 主要攝影機
    public GameObject targetMarkerPrefab;  // 目標標記Prefab
    public float groundHeight = 0.5f;  // 地面高度 (通常是 0)
    [SerializeField] LayerMask groundLayer; // 對應的地面圖層，用於檢測射線

    private GameObject targetMarker;

    void LateUpdate()
    {
        // 捕捉滑鼠位置並轉換為世界座標
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 使用射線檢測來判斷滑鼠指向的地面位置
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 hitPosition = new Vector3((float) Math.Round(hit.point.x), (float) Math.Round(hit.point.y), (float) Math.Round(hit.point.z));
            // 只保留 X 和 Z 座標，將 Y 設置為 groundHeight (通常是 0)
            hitPosition.y = (float) Math.Round(hit.point.y) + groundHeight;

            // 如果尚未創建目標框，則創建它
            if (targetMarker == null)
            {
                targetMarker = Instantiate(targetMarkerPrefab, hitPosition, Quaternion.identity);
            }
            else
            {
                // 更新目標框位置
                targetMarker.transform.position = hitPosition;
            }

            // 確保框框始終平行於XZ平面，不會隨著物體旋轉
            targetMarker.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // 保持框框平行於XZ平面
        }
    }
}
