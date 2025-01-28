using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Camera mainCamera; // 主攝影機
    [SerializeField] LayerMask groundLayer; // 對應的地面圖層，用於檢測射線

    const float FIXED_Y_POSITION = 0.5f;

    Vector3 targetPosition; // 目標位置
    bool isMoving = false; // 是否正在移動
    // float gridSize = 1f;     // 每次移動的格子大小
    Animator animator;
    void Start()
    {
        Transform childTransform = transform.Find("body");
        animator = childTransform.GetComponent<Animator>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // 確保默認使用主攝影機
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // 當按下鼠標左鍵
        {
            SetTargetPosition();
        }

        if (isMoving)
        {
            MoveTowardsTarget();
        }
    }

    void SetTargetPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // 從鼠標生成射線
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // 設定目標位置為射線擊中點
            targetPosition = new Vector3((float) Math.Round(hit.point.x), hit.point.y + FIXED_Y_POSITION, (float) Math.Round(hit.point.z));
            // NavMeshHit nHit;
            // if (NavMesh.SamplePosition(hit.point, out nHit, 0.1f, NavMesh.AllAreas)) {
            //     Debug.Log(nHit.position);
            animator.SetTrigger("isWalk0");
            isMoving = true;
            // } else {
            //     Debug.Log("can't moving");
            //     isMoving = false;
            // }
        }
    }

    void MoveTowardsTarget()
    {
        double offsetX = Math.Round(targetPosition.x - transform.position.x);
        double offsetZ = Math.Round(targetPosition.z - transform.position.z);

        if (Math.Abs(offsetX) == Math.Abs(offsetZ)) { // 45度角允許直接斜走
            // 使用 Vector3.MoveTowards 讓物體以固定速度平滑移
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        } else if (Math.Abs(offsetX) >= Math.Abs(offsetZ)) {
            float nextX = transform.position.x + (int) Math.Ceiling(offsetX);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextX, targetPosition.y, transform.position.z), moveSpeed * Time.deltaTime);
        } else {
            float nextZ = transform.position.z + (int) Math.Ceiling(offsetZ);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetPosition.y, nextZ), moveSpeed * Time.deltaTime);
        }
        
        // 當物體接近目標位置時，停止移動
        if (Vector3.Distance(transform.position, targetPosition) < 0.000001f)
        {
            transform.position = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
            animator.ResetTrigger("isWalk0");
            animator.SetTrigger("isIdle0");
            isMoving = false;
        }
    }
}
