using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float groundCheckDistance = 0.075f;

    public bool isGround = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsGround()) {
            isGround = true;
        } else {
            isGround = false;
        }
    }

    bool IsGround() {
        RaycastHit hit;
        bool g = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, layerMask);
        Debug.Log(g);
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance, 0), Color.red);

        return g;
    }

    // void OnTriggerEnter(Collider other) {
    //     Debug.Log(other.gameObject.name);
    // }
}
