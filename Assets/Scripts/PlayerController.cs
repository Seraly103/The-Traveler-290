using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject world;
    public bool movementLocked = false;

    bool isWalking = false;

    [Header("Footsteps")]
    
    AudioSource footSteps;

    

    SpriteRenderer sr;

    bool IsWalking = false;

    Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        anim = GetComponent<Animator>();
        footSteps = GetComponent<AudioSource>();
        footSteps.loop = true;
        footSteps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        

        bool moveLeft = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);
        isWalking = moveLeft || moveRight;


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

        if(!isWalking)
        {
            anim.SetBool("IsWalking", false);
        }

        // FOOTSTEP AUDIO
        if(isWalking)
        {
            if(!footSteps.isPlaying)
            {
                footSteps.Play();
            }
        }
        else
        {
            footSteps.Stop();
        }
        
    }
}
