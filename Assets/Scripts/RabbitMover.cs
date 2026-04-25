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
    
    public bool destroyAtEnd = false;

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

            
            targetPos.y = currentPos.y;

            

            if (targetPos.x > currentPos.x)
                sr.flipX = true;   
            else
                sr.flipX = false;  

            transform.position = Vector3.MoveTowards(
                currentPos,
                targetPos,
                speed * Time.deltaTime
            );

            if (currentPoint >= points.Length - 1)
            {
                if (destroyAtEnd)
                {
                    Destroy(gameObject);
                }
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


        sr.flipX = !facesRightAtRest;

        isMoving = false;



    }

   
}
