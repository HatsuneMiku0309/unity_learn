using UnityEngine;

public class PlayerMoveAndAnimation : MonoBehaviour
{

    float speed = 5f;
    CharacterController character;
    Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = transform.forward * vertical * speed * Time.deltaTime;
        if (direction != Vector3.zero) {
            animator.SetBool("isRun", true);
        } else {
            animator.SetBool("isRun", false);
        }
        character.Move(direction);

        transform.Rotate(Vector3.up, horizontal * speed * 0.5f);

        if (Input.GetButtonDown("Jump")) {
            animator.SetTrigger("jump");
        }
    }
}
