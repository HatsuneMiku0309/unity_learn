using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayMovementMouseNavMesh : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask layerMask;

    CharacterController character;
    NavMeshAgent navMeshAgent;

    bool isMoving;
    Vector3 targetPosition;

    float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        character = GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            SetTargetPosition();
        }

        if (isMoving) {
            MoveTowardsTargetNavMesh();
        }
    }

    void MoveByKeyboard() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical * speed * Time.deltaTime;
        character.Move(direction);

        transform.Rotate(Vector3.up, horizontal * speed * 0.5f);
    }

    void MoveTowardsTargetNavMesh() {
        navMeshAgent.SetDestination(targetPosition);
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

    void CreateMarker(Vector3 position)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = position;
        marker.transform.localScale = Vector3.one * 0.5f; // 縮小標記物體
        Destroy(marker, 2f); // 2秒後自動銷毀
    }
}
