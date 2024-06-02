using System.Collections;
using CombatScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class NoteMove : MonoBehaviour
{
    [SerializeField]
    public float speed;
    private RhythmGameManager gameManager;
    public UnityEvent<string> OnMiss;
    private NoteParticleSystem noteParticleSystem;
    public float delay = 0.3f;

    private bool isWaitingForInput = false;
    private PlayerInput playerInput;
    private InputAction moveAction;

    void Start()
    {
        gameManager = RhythmGameManager.Instance;

        noteParticleSystem = GameObject.Find("Canvas/UIParticle").GetComponent<NoteParticleSystem>();
        if (noteParticleSystem != null)
        {
            // Add the PlayParticle function to the OnMiss event
            OnMiss.AddListener(noteParticleSystem.PlayParticle);
        }
        else
        {
            Debug.LogError("NoteParticleSystem not found");
        }

        playerInput = GameObject.Find("Player").GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    private void Update()
    {
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
   
    void OnTriggerExit2D(Collider2D other)
    {
         if (other.gameObject.tag == "LeftNote" && this.tag == "RightNote")
        {
            // if (gameObject.activeInHierarchy && other.gameObject.activeInHierarchy)
            // {
            //     StartCoroutine(WaitSetActive(other));
            // }
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
    }
   IEnumerator WaitSetActive(Collider2D otherObject)
    {
        yield return new WaitForSeconds(0.3f);
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        if (otherObject != null && otherObject.gameObject.activeInHierarchy)
        {
            otherObject.gameObject.SetActive(false);
        }
    }
   

    

    

    
}
