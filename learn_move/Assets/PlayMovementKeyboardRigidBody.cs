using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayMovementKeyboardRigidBody : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask layerMask;

    Rigidbody rb;
    GroundCheck g;

    [SerializeField] float speed = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        g = gameObject.GetComponentInChildren<GroundCheck>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update()
    {
        MoveByKeyboard();
    }

    void MoveByKeyboard() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical * speed;
        direction.y = rb.linearVelocity.y;
        rb.linearVelocity  = direction;  // 保持Y軸速度（重力效果）
        // character.Move(direction);

        transform.Rotate(Vector3.up, horizontal * speed * 0.5f);
        if (Input.GetButtonDown("Jump") && g.isGround) {
            // isGround = false;
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    // void OnCollisionEnter(Collision other) {
    //     if (other.gameObject.name == "Plane") {
    //      isGround = true;
    //     }
    // }
}
