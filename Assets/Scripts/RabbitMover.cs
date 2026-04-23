using UnityEngine;
using System.Collections;

public class RabbitMover : MonoBehaviour
{
    public Transform[] points; 
    public float speed = 2f;

    public Transform targetPoint;
    private bool hasMoved = false;

    private int currentPoint = 0;
    private bool isMoving = false;
    private SpriteRenderer sr;
    public bool facesRightAtRest = false; 
    

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        
    }


    public void MoveOnce()
    {
        if (hasMoved) return; 

        if (currentPoint >= points.Length) return; 
        StartCoroutine(MoveToTarget(points[currentPoint]));
        currentPoint++;
    }

    IEnumerator MoveToTarget(Transform targetPoint)
    {
        isMoving = true;

        while (Mathf.Abs(transform.position.x - targetPoint.position.x) > 0.05f)
        {
            Vector3 currentPos = transform.position;
            Vector3 targetPos = targetPoint.position;

            // lock Y
            targetPos.y = currentPos.y;

            float direction = targetPos.x - currentPos.x;

            if (targetPos.x > currentPos.x)
                sr.flipX = true;   // facing RIGHT (because your sprite is reversed)
            else
                sr.flipX = false;  // facing LEFT

            transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                speed * Time.deltaTime
            );

            if (currentPoint >= points.Length - 1)
            {
                Destroy(gameObject);
            }

            yield return null;
        }

        transform.position = new Vector3(
            targetPoint.position.x,
            transform.position.y,
            transform.position.z
        );
        sr.flipX = !facesRightAtRest;
        isMoving = false;


        

    }

   
}
