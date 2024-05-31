using UnityEngine;

public class NoteLifeController : MonoBehaviour
{
    public float lifeTime = 4.0f; 
    private float timer;

    void OnEnable()
    {
        timer = lifeTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
