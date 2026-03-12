using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject world;

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
