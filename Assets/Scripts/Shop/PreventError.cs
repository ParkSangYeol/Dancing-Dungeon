using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PreventError : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitForTest());
        if (PlayerPrefs.GetInt(gameObject.transform.parent.name,0) == 1)
        {
            PreventDoublePurchase();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PreventDoublePurchase()
    {
        if (PlayerPrefs.GetInt(gameObject.transform.parent.name) == 1)
        {
            gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "구매완료";
            gameObject.GetComponent<Button>().interactable = false;
        }
    }

    IEnumerator WaitForTest()
    {
        yield return new WaitForSeconds(3f);

    }
    
}
