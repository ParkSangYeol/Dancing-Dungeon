using System.Collections;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "LeftNote" && this.tag == "RightNote")
        {
            gameManager.StartCoroutineFromManager(WaitForInputCoroutine());
            UiManager.instance.SetCombo(0,"Miss");
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            
        }
    }

    private IEnumerator WaitForInputCoroutine()
    {
        isWaitingForInput = true;
        float timer = 0f;

        while (timer < delay)
        {
            if (moveAction.triggered)
            {
                
                isWaitingForInput = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // if (isWaitingForInput)
        // {
        //     UiManager.instance.SetCombo(0);
        // } // 부딫히고나서 delaya시간안에 입력이 없다면 미스

        isWaitingForInput = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Add any collision logic here if needed
    }
}
