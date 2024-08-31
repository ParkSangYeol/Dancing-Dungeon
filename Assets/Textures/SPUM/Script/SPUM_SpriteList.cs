using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Net;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SPUM_SpriteList : MonoBehaviour
{
    public List<SpriteRenderer> _itemList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _eyeList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _hairList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _bodyList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _clothList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _armorList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _pantList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _weaponList = new List<SpriteRenderer>();
    public List<SpriteRenderer> _backList = new List<SpriteRenderer>();

    public SPUM_HorseSpriteList _spHorseSPList;
    public string _spHorseString;
    // Start is called before the first frame update

    public Texture2D _bodyTexture;
    public string _bodyString;

    public List<string> _hairListString = new List<string>();
    public List<string> _clothListString = new List<string>();
    public List<string> _armorListString = new List<string>();
    public List<string> _pantListString = new List<string>();
    public List<string> _weaponListString = new List<string>();
    public List<string> _backListString = new List<string>();
    

    
    private string previewBodyString;
    private string previewHairString;
    private string previewPantsString;
    private string previewRaceString;
    private string initialBodyString;
    private string initialHairString;
    private string initialPantsString;
    private string initialRaceString;

    private string[] inventoryhairpath = new string[6];
    private string[] inventoryclothpath = new string[6];
    private string[] inventorypantspath = new string[5];
    private string[] inventoryracepath = new string[5];

    

    private int currentInventoryindex=0;
  


    private void Start()
    {
        Debug.Log("처음 인종 패스 : "+_bodyString);
        
        if (!PlayerPrefs.HasKey("WearingHair"))
        {
            initialHairString = _hairListString[0];
            PlayerPrefs.SetString("WearingHair", initialHairString);
        }

        if (!PlayerPrefs.HasKey("WearingCloth"))
        {
            initialBodyString = _clothListString[0];
            PlayerPrefs.SetString("WearingCloth", initialBodyString);
        }

        if (!PlayerPrefs.HasKey("WearingPants"))
        {
            initialPantsString = _pantListString[0];
            PlayerPrefs.SetString("WearingPants",initialPantsString);
        }

        if (!PlayerPrefs.HasKey("WearingRace") /*|| _bodyString==null*/)
        {
            if (string.IsNullOrEmpty(_bodyString))
            {
                _bodyString ="Assets/Resources/SPUM/SPUM_Sprites/BodySource/Species/0_Human/Human_1.png";
            }
            initialRaceString = _bodyString;
            PlayerPrefs.SetString("WearingRace", initialRaceString);
        }
        PlayerPrefs.Save();

        
        if (CompareTag("Player"))
        {
            Debug.Log("처음 로드될때의 인종 패스 : " + PlayerPrefs.GetString("WearingRace"));
            Debug.Log("2번 인종 패스 : "+_bodyString);
            /*if (_bodyString.IsNullOrWhitespace())
            {
                Debug.Log(("넌 널이야"));
                _bodyString = "Assets/Resources/SPUM/SPUM_Sprites/BodySource/Species/0_Human/Human_1.png";
                PlayerPrefs.SetString("WearingRace",_bodyString);
                PlayerPrefs.Save();
            }*/
            Debug.Log("3번 인종 패스 : "+_bodyString);
            LoadWear();
            
        }

        
    }

    public void Reset()
    {
        for(var i = 0 ; i < _hairList.Count;i++)
        {
            if(_hairList[i]!=null) _hairList[i].sprite = null;
        }
        for(var i = 0 ; i < _clothList.Count;i++)
        {
            if(_clothList[i]!=null) _clothList[i].sprite = null;
        }
        for(var i = 0 ; i < _armorList.Count;i++)
        {
            if(_armorList[i]!=null) _armorList[i].sprite = null;
        }
        for(var i = 0 ; i < _pantList.Count;i++)
        {
            if(_pantList[i]!=null) _pantList[i].sprite = null;
        }
        for(var i = 0 ; i < _weaponList.Count;i++)
        {
            if(_weaponList[i]!=null) _weaponList[i].sprite = null;
        }
        for(var i = 0 ; i < _backList.Count;i++)
        {
            if(_backList[i]!=null) _backList[i].sprite = null;
        }
    }

    public void LoadSpriteSting()
    {

    }

    public void LoadSpriteStingProcess(List<SpriteRenderer> SpList, List<string> StringList)
    {
        for(var i = 0 ; i < StringList.Count ; i++)
        {
            if(StringList[i].Length > 1)
            {

                // Assets/SPUM/SPUM_Sprites/BodySource/Species/0_Human/Human_1.png
            }
        }
    }
    

    public void LoadSprite(SPUM_SpriteList data)
    {
        //스프라이트 데이터 연동
        SetSpriteList(_hairList,data._hairList);
        SetSpriteList(_bodyList,data._bodyList);
        SetSpriteList(_clothList,data._clothList);
        SetSpriteList(_armorList,data._armorList);
        SetSpriteList(_pantList,data._pantList);
        SetSpriteList(_weaponList,data._weaponList);
        SetSpriteList(_backList,data._backList);
        SetSpriteList(_eyeList,data._eyeList);
        
        if(data._spHorseSPList!=null)
        {
            SetSpriteList(_spHorseSPList._spList,data._spHorseSPList._spList);
            _spHorseSPList = data._spHorseSPList;
        }
        else
        {
            _spHorseSPList = null;
        }

        //색 데이터 연동.
        if(_eyeList.Count> 2 &&  data._eyeList.Count > 2 )
        {
            _eyeList[2].color = data._eyeList[2].color;
            _eyeList[3].color = data._eyeList[3].color;
        }

        _hairList[3].color = data._hairList[3].color;
        _hairList[0].color = data._hairList[0].color;
        //꺼져있는 오브젝트 데이터 연동.x
        _hairList[0].gameObject.SetActive(!data._hairList[0].gameObject.activeInHierarchy);
        _hairList[3].gameObject.SetActive(!data._hairList[3].gameObject.activeInHierarchy);

        _hairListString = data._hairListString;
        _clothListString = data._clothListString;
        _pantListString = data._pantListString;
        _armorListString = data._armorListString;
        _weaponListString = data._weaponListString;
        _backListString = data._backListString;
    }

    public void SetSpriteList(List<SpriteRenderer> tList, List<SpriteRenderer> tData)
    {
        for(var i = 0 ; i < tData.Count;i++)
        {
            if(tData[i]!=null) 
            {
                tList[i].sprite = tData[i].sprite;
                tList[i].color = tData[i].color;
            }
            else tList[i] = null;
        }
    }

    public void ResyncData()
    {
        SyncPath(_hairList,_hairListString);
        SyncPath(_clothList,_clothListString);
        SyncPath(_armorList,_armorListString);
        SyncPath(_pantList,_pantListString);
        SyncPath(_weaponList,_weaponListString);
        SyncPath(_backList,_backListString);
    }
    public void SyncPath_Race(List<SpriteRenderer> _objList, string _path)
    {
        if (_objList == null)
        {
            Debug.LogError("_objList is null");
            return;
        }
        if (string.IsNullOrEmpty(_path))
        {
            
            Debug.LogError(_objList);
            Debug.LogError("_path is null or empty");
            return;
        }

        // 경로를 조정
        string tPath = _path.Replace("Assets/Resources/", "").Replace(".png", "");

        // Sprite 리소스를 로드
        Sprite[] tSP = Resources.LoadAll<Sprite>(tPath);
        if (tSP.Length > 0)
        {
            // _objList의 각 SpriteRenderer에 Sprite를 순서대로 할당
            for (var i = 0; i < _objList.Count; i++)
            {
                if (_objList[i] != null)
                {
                    if (i < tSP.Length)
                    {
                        _objList[i].sprite = tSP[i];
                    }
                    else
                    {
                        Debug.LogWarning($"_objList at index {i} has no corresponding sprite in tSP");
                        _objList[i].sprite = null; // _objList가 tSP보다 길다면 null 할당
                    }
                }
                else
                {
                    Debug.LogWarning($"_objList at index {i} is null");
                }
            }
        }
        else
        {
            Debug.LogWarning($"No sprites found at path: {tPath}");
        }
    }

    
   

    public void SyncPath(List<SpriteRenderer> _objList, List<string> _pathList)
    {
        if (_objList == null)
        {
            Debug.LogError("_objList is null");
            return;
        }
        if (_pathList == null)
        {
            Debug.LogError("_pathList is null");
            return;
        }

        for (var i = 0; i < _pathList.Count; i++)
        {
            if (_pathList[i].Length > 1)
            {
                string tPath = _pathList[i];
                tPath = tPath.Replace("Assets/Resources/", "");
                tPath = tPath.Replace(".png", "");

                Sprite[] tSP = Resources.LoadAll<Sprite>(tPath);
                if (tSP.Length > 1)
                {
                    if (i < _objList.Count && _objList[i] != null)
                    {
                        _objList[i].sprite = tSP[i];
                    }
                    else
                    {
                        Debug.LogWarning($"_objList at index {i} is null or out of range");
                    }
                }
                else if (tSP.Length > 0)
                {
                    if (i < _objList.Count && _objList[i] != null)
                    {
                        _objList[i].sprite = tSP[0];
                    }
                    else
                    {
                        Debug.LogWarning($"_objList at index {i} is null or out of range");
                    }
                }
            }
            else
            {
                if (i < _objList.Count && _objList[i] != null)
                {
                    _objList[i].sprite = null;
                }
                else
                {
                    Debug.LogWarning($"_objList at index {i} is null or out of range");
                }
            }
        }
    }
    public void GetTypeList(string type)
    {
      
            switch(type)
            {
                case "Cloth" :
                    ReplacePathCloth_Pant(_clothListString,previewBodyString);
                    SyncPath(_clothList,_clothListString);
                    break;
                case "Hair" :
                    _hairListString[0] = previewHairString;
                    SyncPath(_hairList, _hairListString);
                    break;
                case "Pants" :
                    ReplacePathCloth_Pant(_pantListString,previewPantsString);
                    SyncPath(_pantList,_pantListString);
                    break;
               case "Race" :
                    _bodyString = previewRaceString;
                    SyncPath_Race(_bodyList, _bodyString);
                    break;
                    
                
            }
        
    }

    public void Wear(string type) //인벤에서 입어보기 
    {
        switch(type)
        {
            case "Cloth":
                Debug.Log(currentInventoryindex);
                ReplacePathCloth_Pant(_clothListString, inventoryclothpath[currentInventoryindex-1]);
                SyncPath(_clothList, _clothListString);
                previewBodyString = inventoryclothpath[currentInventoryindex-1];
                //PlayerPrefs.GetString("WearingCloth", inventoryclothpath[currentInventoryindex - 1]);
                //PlayerPrefs.SetString("WearingCloth", inventoryclothpath[currentInventoryindex - 1]);
                break;
            case "Hair":
                Debug.Log("currentindex는 "+currentInventoryindex+"이고 들어간 path는"+inventoryhairpath[currentInventoryindex-1]);
                _hairListString[0] = inventoryhairpath[currentInventoryindex-1];
                Debug.Log(_hairListString[0]);
                SyncPath(_hairList, _hairListString);
                previewHairString = inventoryhairpath[currentInventoryindex-1];
                //PlayerPrefs.GetString("WearingHair", inventoryhairpath[currentInventoryindex - 1]);
                //PlayerPrefs.SetString("WearingHair", inventoryhairpath[currentInventoryindex - 1]);
                break;
            case "Pants":
                Debug.Log(currentInventoryindex);
                ReplacePathCloth_Pant(_pantListString, inventorypantspath[currentInventoryindex-1]);
                SyncPath(_pantList, _pantListString);
                previewPantsString = inventorypantspath[currentInventoryindex - 1];
                //PlayerPrefs.GetString("WearingPants", inventorypantspath[currentInventoryindex - 1]);
                //PlayerPrefs.SetString("WearingPants", inventorypantspath[currentInventoryindex - 1]);
                break;
            case "Race":
                Debug.Log(currentInventoryindex);
                _bodyString = inventoryracepath[currentInventoryindex - 1];
                SyncPath_Race(_bodyList, _bodyString);
                previewRaceString = inventoryracepath[currentInventoryindex - 1];
                Debug.Log("선택하신 인종 path : " + previewRaceString);
                //PlayerPrefs.GetString("WearingRace", inventoryracepath[currentInventoryindex - 1]);
                //PlayerPrefs.SetString("WearingRace", inventoryracepath[currentInventoryindex - 1]);
                break;
        }
        //PlayerPrefs.Save();
    }
    
    public void ReplacePathCloth_Pant(List<string> pahtlist, string mypath)
    {
    
        for(int i=0;i<pahtlist.Count;i++)
        {
            pahtlist[i] = mypath;
        }
    }
    
    public void SetClothPath(string path)
    {
        previewBodyString = path;
    }
    public void SetHairPath(string path)
    {
        previewHairString = path;
    }
    public void SetPantsPath(string path)
    {
        previewPantsString = path;
    }

    public void SetRacePath(string path)
    {
        previewRaceString = path;
    }
    public void ResetCostume()
    {
        LoadWear();
    }
    

    public void InitializedPath(string type, string path, int index)
    {
        switch (type)
        {
            case "Hair":
                Debug.Log(index+"번째에 추가할"+"리스트에 추가할 PATH : "+ path);
                inventoryhairpath[index] = path;
                Debug.Log(index+"번째에 추가할"+"리스트에 들어간 패스 : "+ inventoryhairpath[index]);
                
                break;
            case "Cloth":
                Debug.Log(index+"번째에 추가할"+"리스트에 추가할 PATH : "+ path);
                inventoryclothpath[index] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventoryclothpath[index]);
                
                break;
            case "Pants":
                Debug.Log(index+"번째에 추가할"+"리스트에 추가할 PATH : "+ path);
                inventorypantspath[index] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventorypantspath[index]);
                break;
            case "Race":
                Debug.Log(index+"번째에 추가할"+"리스트에 추가할 PATH : "+ path);
                inventoryracepath[index] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventoryracepath[index]);
                break;
        }
    }
    

    public void SetPathInventory(string type,string path)
    {
        switch (type)
        {
            case "Hair":
                Debug.Log("리스트에 추가할 PATH : "+ path);
                inventoryhairpath[currentInventoryindex] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventoryhairpath[currentInventoryindex]);
                Debug.Log("선택한 버튼 번호 : "+currentInventoryindex);
                break;
            case "Cloth":
                Debug.Log("리스트에 추가할 PATH : "+ path);
                inventoryclothpath[currentInventoryindex] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventoryhairpath[currentInventoryindex]);
                Debug.Log("선택한 버튼 번호 : "+currentInventoryindex);
                break;
            case "Pants":
                Debug.Log("리스트에 추가할 PATH : "+ path);
                inventorypantspath[currentInventoryindex] = path;
                Debug.Log("리스트에 들어간 패스 : "+ inventoryhairpath[currentInventoryindex]);
                Debug.Log("선택한 버튼 번호 : "+currentInventoryindex);
                break;
            case "Race":
                inventoryracepath[currentInventoryindex] = path;
                break;
                
        }
    }

    public void SetCurrentInventoryindex(int index)
    {
        currentInventoryindex = index;
    }

    public void LoadWear()
    {
        if (PlayerPrefs.HasKey("WearingHair"))
        {
            string path = PlayerPrefs.GetString("WearingHair");
            _hairListString[0] = path;
            Debug.Log("머리되돌리기버튼 시행 : "+path);
            SyncPath(_hairList, _hairListString);
        }

        if (PlayerPrefs.HasKey("WearingCloth"))
        {
            string path = PlayerPrefs.GetString("WearingCloth");
            ReplacePathCloth_Pant(_clothListString, path);
            Debug.Log("옷되돌리기버튼 시행 : "+path);
            SyncPath(_clothList, _clothListString);
        }

        if (PlayerPrefs.HasKey("WearingPants"))
        {
            string path = PlayerPrefs.GetString("WearingPants");
            ReplacePathCloth_Pant(_pantListString, path);
            Debug.Log("바지되돌리기버튼 시행 : "+path);
            SyncPath(_pantList, _pantListString);
        }
        if (PlayerPrefs.HasKey("WearingRace"))
        {
            string path = PlayerPrefs.GetString("WearingRace");
            _bodyString = path;
            Debug.Log("몸되돌리기버튼 시행 : "+path);
            SyncPath_Race(_bodyList, path);
        }
        
        
    }

    public void SaveWearing()
    {
       
        if (PlayerPrefs.HasKey("WearingHair"))
        {
            if (previewHairString == null)
            {
                previewHairString = _hairListString[0];
            }
            string path = previewHairString; 
            PlayerPrefs.SetString("WearingHair",path);
            _hairListString[0] = path;
            SyncPath(_hairList, _hairListString);
        }
        if (PlayerPrefs.HasKey("WearingCloth"))
        {
            if (previewBodyString == null)
            {
                previewBodyString = _clothListString[0];
            }
            string path = previewBodyString;
            PlayerPrefs.SetString("WearingCloth",path);
            ReplacePathCloth_Pant(_clothListString, path);
            SyncPath(_clothList, _clothListString);
        }
        if (PlayerPrefs.HasKey("WearingPants"))
        {
            if (previewPantsString == null)
            {
                previewPantsString = _pantListString[0];
            }
            string path = previewPantsString; 
            PlayerPrefs.SetString("WearingPants",path);
            ReplacePathCloth_Pant(_pantListString, path);
            SyncPath(_pantList, _pantListString);
        }
        if (PlayerPrefs.HasKey("WearingRace"))
        {
            Debug.Log(_bodyString);
            if (previewRaceString == null)
            {
                previewRaceString = _bodyString;
            }
            string path = previewRaceString;
            PlayerPrefs.SetString("WearingRace",path);
            _bodyString = path;
            SyncPath_Race(_bodyList, _bodyString);
            Debug.Log("저장된 인종 패스 플레이 프리펩 : "+PlayerPrefs.GetString("WearingRace"));
        }
        PlayerPrefs.Save();
        
    }


    IEnumerator SaveWaiting()
    {
        yield return new WaitForSeconds(1.5f);
    }
}
