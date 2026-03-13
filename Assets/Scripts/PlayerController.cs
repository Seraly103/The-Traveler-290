using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject world;
    public bool movementLocked = false;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;

    [SerializeField] private float footstepStartTimeSeconds = 2f;

    SpriteRenderer sr;

    bool IsWalking = false;

    Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        IsWalking = false;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movementLocked)
        {
            IsWalking = false;
            anim.SetBool("IsWalking", false);
            return;
        }

        bool moveLeft = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);
        IsWalking = moveLeft || moveRight;


        if(Input.GetKey(KeyCode.A))
        {
            anim.SetBool("IsWalkingRight", true);
            anim.SetBool("IsWalkingLeft", false);
            anim.SetBool("IsWalking", true);
            world.transform.position += Vector3.right * speed * Time.deltaTime;
            sr.flipX = false;
        }
        
        

        if(Input.GetKey(KeyCode.D))
        {
            anim.SetBool("IsWalkingLeft", false);
            anim.SetBool("IsWalkingRight", true);
            anim.SetBool("IsWalking", true);
            world.transform.position += Vector3.left * speed * Time.deltaTime;
            sr.flipX = true;
        }
        
        
    }
}
