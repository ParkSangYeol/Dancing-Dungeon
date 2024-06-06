using UnityEngine;

public class NoteHitSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag=="RightNote" || other.tag == "LeftNote")
        {
            if(Input.GetKey(KeyCode.Space))
            {
                Debug.Log(gameObject.tag);
                other.gameObject.SetActive(false);
            }
            other.gameObject.SetActive(false);
        }
        
    }
    
        
}

