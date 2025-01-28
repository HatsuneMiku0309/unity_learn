using UnityEngine;

public class PlayLookAt : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    CharacterController character;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayMove();
        PlayLookAtMousePosition();
    }

    void PlayMove() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;

        character.Move(direction);
    }

    void PlayLookAtMousePosition() {
        Vector3 playerPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 point = Input.mousePosition - playerPosition;
        float angle = Mathf.Atan2(point.x, point.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
    }
}
