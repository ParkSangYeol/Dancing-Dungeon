using UnityEngine;

public class NoteMove : MonoBehaviour
{
    [SerializeField]
    public float speed;
    private RhythmGameManager gameManager;
    

    void Start()
    {
        gameManager = RhythmGameManager.Instance;
       
    }
    private void Update() {
        MoveNode();
    }

    

    private void MoveNode()
    {
        if (transform.tag == "LeftNote")
        {
            transform.localPosition += Vector3.right * Time.deltaTime * speed;
        }
        else if (transform.tag == "RightNote")
        {
            transform.localPosition += Vector3.left * Time.deltaTime * speed;
        }
    } 

    // void OnTriggerEnter2D(Collider2D other)
    // {

    //      if ((other.gameObject.tag == "LeftNote" && this.tag == "RightNote") ||  //서로 부딫힐때
    //         (other.gameObject.tag == "RightNote" && this.tag == "LeftNote"))
    //     {
    //         gameManager.DeregisterNoteTiming(this);
    //         gameManager.DeregisterNoteTiming(other.gameObject.GetComponent<NoteMove>());
    //         gameObject.SetActive(false);
    //         other.gameObject.SetActive(false);
        
            
    //     }
    //     if (other.CompareTag("LeftPerfect") || other.CompareTag("LeftGreat") || other.CompareTag("LeftBad") ||
    //         other.CompareTag("RightPerfect") || other.CompareTag("RightGreat") || other.CompareTag("RightBad"))
    //     {
    //         gameManager.RegisterNoteTiming(this, other.tag);
    //     }
       
        
    // }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LeftPerfect") || other.CompareTag("LeftGreat") || other.CompareTag("LeftBad") ||
            other.CompareTag("RightPerfect") || other.CompareTag("RightGreat") || other.CompareTag("RightBad"))
        {
            gameManager.DeregisterNoteTiming(this);
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.tag == "LeftNote" && this.tag == "RightNote") ||  //서로 부딫힐때
            (other.gameObject.tag == "RightNote" && this.tag == "LeftNote"))
        {
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
    
        }
    }
    

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
