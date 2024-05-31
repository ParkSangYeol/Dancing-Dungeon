using DG.Tweening;
using UnityEngine;

public class HitScanByRay : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector2.left * 1000, Color.green, 2.0f);
        Debug.DrawRay(transform.position, Vector2.right * 1000, Color.red, 2.0f);

        CheckNote();
    }

    void CheckNote()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit2D lefthit = Physics2D.Raycast(transform.position, Vector2.left, Mathf.Infinity);
            RaycastHit2D righthit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity);

            if (lefthit.collider != null && righthit.collider != null) 
            {
                if (lefthit.collider.tag == "LeftNote" && righthit.collider.tag == "RightNote")
                {
                    float left_x = lefthit.collider.transform.position.x;
                    float right_x = righthit.collider.transform.position.x;
                    float xDifference = right_x - left_x;
                   
                    if(xDifference>0 && xDifference<=200)
                    {
                        Debug.Log("Perfect");
                    }
                    else if(xDifference>200 && xDifference<=500)
                    {
                        Debug.Log("Great");
                    }
                    else if(xDifference>500 && xDifference<1000)
                    {
                        Debug.Log("Bad");
                    }
                  
                }
            }
        }
    }
}
