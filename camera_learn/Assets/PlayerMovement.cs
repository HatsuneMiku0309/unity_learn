using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float rotateSpeed = 5f;
    CharacterController character;

    void Start() {
        character = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 distance = transform.forward * vertical * speed * Time.deltaTime;
        character.Move(distance);

        float rotate = horizontal * rotateSpeed;
        transform.Rotate(Vector3.up, rotate);
    }
}
