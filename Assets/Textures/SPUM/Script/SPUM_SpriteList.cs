using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using Sirenix.OdinInspector;
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

    private string baseBodyString;
    private string basePantsString;
    private string baseHairString;
    private string previewBodyString;
    private string previewHairString;
    private string previewPantsString;

    private string[] inventoryhairpath = new string[6];
    private string[] inventoryclothpath = new string[6];
    private string[] inventorypantspath = new string[5];

    private int currentInventoryindex=0;
    private void Start() {
        baseBodyString = PlayerPrefs.GetString("PlayerBody",null);
        basePantsString = PlayerPrefs.GetString("PlayerPants",null);
        baseHairString = PlayerPrefs.GetString("PlayerHair",null);
        if (this.CompareTag("Player"))
        {
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
                
            }
        
    }

    public void Wear(string type)
    {
        switch(type)
        {
            case "Cloth":
                Debug.Log(currentInventoryindex);
                ReplacePathCloth_Pant(_clothListString, inventoryclothpath[currentInventoryindex-1]);
                SyncPath(_clothList, _clothListString);
                PlayerPrefs.GetString("WearingCloth", inventoryclothpath[currentInventoryindex - 1]);
                PlayerPrefs.SetString("WearingCloth", inventoryclothpath[currentInventoryindex - 1]);
                break;
            case "Hair":
                Debug.Log("currentindex는 "+currentInventoryindex+"이고 들어간 path는"+inventoryhairpath[currentInventoryindex-1]);
                _hairListString[0] = inventoryhairpath[currentInventoryindex-1];
                Debug.Log(_hairListString[0]);
                SyncPath(_hairList, _hairListString);
                PlayerPrefs.GetString("WearingHair", inventoryhairpath[currentInventoryindex - 1]);
                PlayerPrefs.SetString("WearingHair", inventoryhairpath[currentInventoryindex - 1]);
                break;
            case "Pants":
                Debug.Log(currentInventoryindex);
                ReplacePathCloth_Pant(_pantListString, inventorypantspath[currentInventoryindex-1]);
                SyncPath(_pantList, _pantListString);
                PlayerPrefs.GetString("WearingPants", inventorypantspath[currentInventoryindex - 1]);
                PlayerPrefs.SetString("WearingPants", inventorypantspath[currentInventoryindex - 1]);
                
                break;
        }
        PlayerPrefs.Save();
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
    public void ResetCostume()
    {
        LoadWear();

    }
    private void InitializeToReset()
    {
        SetClothPath(baseBodyString);
        ReplacePathCloth_Pant(_clothListString,previewBodyString);
        SyncPath(_clothList,_clothListString);
        
        SetPantsPath(basePantsString);
        ReplacePathCloth_Pant(_pantListString,previewPantsString);
        SyncPath(_pantList,_pantListString);
        
        _hairListString[0] = previewHairString;
        SyncPath(_hairList,_hairListString);
        

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
                Debug.Log("리스트에 들어간 패스 : "+ inventoryclothpath[index]);
                
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
            SyncPath(_hairList, _hairListString);
            
        }

        if (PlayerPrefs.HasKey("WearingCloth"))
        {
            string path = PlayerPrefs.GetString("WearingCloth");
            ReplacePathCloth_Pant(_clothListString, path);
            SyncPath(_clothList, _clothListString);
        }

        if (PlayerPrefs.HasKey("WearingPants"))
        {
            string path = PlayerPrefs.GetString("WearingPants");
            ReplacePathCloth_Pant(_pantListString, path);
            SyncPath(_pantList, _pantListString);
        }
    }
    
}
