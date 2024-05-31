using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private static UiManager instance;
    private GameObject optionPanel;
    private GameObject mainPanel;
    private AudioSource audioSource;
    private Slider volumeSlider; // 슬라이더 UI

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
        
        optionPanel = GameObject.Find("Canvas/OptionPannel");
        mainPanel = GameObject.Find("Canvas/MainPannel");
        volumeSlider = GameObject.Find("Canvas/OptionPannel/Sound/VolumeSlider").GetComponent<Slider>();
        if(optionPanel!=null && mainPanel!=null && volumeSlider!=null)
        {
            optionPanel.SetActive(false);
            mainPanel.SetActive(true);
            Debug.Log("존재");
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip bgmClip = Resources.Load<AudioClip>("MainBgm"); // Resources 폴더에서 BGM 로드
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true; // BGM 반복 재생 설정
            audioSource.Play(); // BGM 재생
        }
    }

    void Start()
    {
        
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
        else
        {
            Debug.LogError("볼륨 슬라이더가 없어~");
        }
    }

    void Update()
    {
        
    }

    public void VisibleOptionPanel()
    {
        optionPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void VisibleMainPanel()
    {
        optionPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("NoteSystemDev");
        AudioClip battleBgm = Resources.Load<AudioClip>("BattleBgm");
         if (battleBgm != null)
        {
            audioSource.clip = battleBgm;
            audioSource.loop = true; // BGM 반복 재생 설정
            audioSource.Play(); // BGM 재생
        }
        
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
