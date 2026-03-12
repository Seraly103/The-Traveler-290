using UnityEngine;
using UnityEngine.Video;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject world;
    public bool movementLocked;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private VideoPlayer footstepVideoPlayer;
    [SerializeField] private float footstepStartTimeSeconds = 2f;

    SpriteRenderer sr;

    bool IsWalking = false;

    Animator anim;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        IsWalking = false;
        anim = GetComponent<Animator>();

        if (footstepSource != null)
        {
            footstepSource.playOnAwake = false;
            footstepSource.loop = true;
        }

        if (footstepVideoPlayer != null)
        {
            footstepVideoPlayer.playOnAwake = false;
            footstepVideoPlayer.isLooping = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (movementLocked)
        {
            IsWalking = false;
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsWalkingLeft", false);
            anim.SetBool("IsWalkingRight", false);
            StopFootsteps();
            return;
        }

        bool moveLeft = Input.GetKey(KeyCode.A);
        bool moveRight = Input.GetKey(KeyCode.D);
        IsWalking = moveLeft || moveRight;

        anim.SetBool("IsWalking", IsWalking);

        if (moveLeft)
        {
            anim.SetBool("IsWalkingRight", true);
            anim.SetBool("IsWalkingLeft", false);
            world.transform.position += Vector3.right * speed * Time.deltaTime;
            sr.flipX = false;
        }

        if (moveRight)
        {
            anim.SetBool("IsWalkingLeft", true);
            anim.SetBool("IsWalkingRight", false);
            world.transform.position += Vector3.left * speed * Time.deltaTime;
            sr.flipX = true;
        }

        if (!IsWalking)
        {
            anim.SetBool("IsWalkingLeft", false);
            anim.SetBool("IsWalkingRight", false);
            StopFootsteps();
            return;
        }

        PlayFootsteps();
    }

    private void PlayFootsteps()
    {
        if (footstepVideoPlayer != null)
        {
            if (!footstepVideoPlayer.isPlaying)
            {
                footstepVideoPlayer.time = Mathf.Max(0f, footstepStartTimeSeconds);
                footstepVideoPlayer.Play();
            }
            return;
        }

        if (footstepSource == null || footstepSource.isPlaying)
        {
            return;
        }

        if (footstepSource.clip != null && footstepSource.clip.length > 0f)
        {
            float maxStart = Mathf.Max(0f, footstepSource.clip.length - 0.01f);
            footstepSource.time = Mathf.Clamp(footstepStartTimeSeconds, 0f, maxStart);
        }

        footstepSource.Play();
    }

    private void StopFootsteps()
    {
        if (footstepVideoPlayer != null && footstepVideoPlayer.isPlaying)
        {
            footstepVideoPlayer.Stop();
        }

        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }
}
