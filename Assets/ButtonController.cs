using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public GameObject option;
    public GameObject main;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoNext()
    {
        SceneManager.LoadScene("NoteSystemDev2");
    }
    public void VisibleMain()
    {
        main.SetActive(true);
        option.SetActive(false);
    }
    public void VisibleOption()
    {
         main.SetActive(false);
         option.SetActive(true);
    }
}
