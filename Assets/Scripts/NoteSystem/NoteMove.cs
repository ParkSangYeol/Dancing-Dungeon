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
    private CombatSceneUIManager combatSceneUIManager;

    private bool isWaitingForInput = false;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private int miss =0;

    void Start()
    {
        
        combatSceneUIManager=GameObject.Find("CombatUiManager").GetComponent<CombatSceneUIManager>();
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
            transform.localPosition += Vector3.right * (Time.deltaTime * speed);
        }
        else if (transform.tag == "RightNote")
        {
            transform.localPosition += Vector3.left * (Time.deltaTime * speed);
        }
    }
   
    void OnTriggerEnter2D(Collider2D other)
    {
         if (other.gameObject.CompareTag("LeftNote") && this.CompareTag("RightNote") || (other.gameObject.CompareTag("RightNote") && this.CompareTag("LeftNote")))
        {
            if (gameObject.activeInHierarchy && other.gameObject.activeInHierarchy) 
            {
                  StartCoroutine(WaitSetActive());
            }
            
        }
    }
   IEnumerator WaitSetActive()
    {
        yield return new WaitForSeconds(0.10f);
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
            if (this.gameObject.CompareTag("LeftNote"))
            {
                OnMiss.Invoke("Miss");
                combatSceneUIManager.SetCombo(miss,"Miss");
            }
        }
        
    }
   

    

    

    
}
