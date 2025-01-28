using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayMovementKeyboard : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    [SerializeField] LayerMask layerMask;

    CharacterController character;

    float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponent<CharacterController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        MoveByKeyboard();
    }

    void MoveByKeyboard() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical * speed * Time.deltaTime;
        character.Move(direction);

        transform.Rotate(Vector3.up, horizontal * speed * 0.5f);
    }
}
