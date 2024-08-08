using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    public class ScreenClickHandler : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] [InfoBox("터치를 확인하는 캔버스를 설정해 주세요!", InfoMessageType.Warning, "IsSetGraphicRaycaster")]
        private GraphicRaycaster graphicRaycaster;

        [SerializeField] 
        private SFXPlayer sfxPlayer;

        
        [Title("SFXs")] 
        [SerializeField] [AssetsOnly]
        private AudioClip uiClickAudioClip;
        
        [SerializeField] [AssetsOnly]
        private AudioClip missClickAudioClip;
        
        [Title("Variables")]
        [SerializeField] 
        private LayerMask raycastMask;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            InitComponent();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 클릭 위치를 가져옴
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;
                
                // 마우스 클릭시 실행
                if (GetOverlapUI(pointerEventData, out var overlapObject))
                {
                    // UI를 클릭한 경우
                    Debug.Log("[ScreenClickHandler] clicked object: " + overlapObject);
                    // SFX 출력. UI 종류에 따른 처리 진행
                    if (overlapObject.CompareTag("NegativeSFXUI"))
                    {
                        PlaySFX(uiClickAudioClip, new Vector2(0.9f, 0.95f));
                    }
                    else if (overlapObject.TryGetComponent<Toggle>(out var toggle))
                    {
                        if (toggle.isOn)
                        {
                            PlaySFX(uiClickAudioClip, new Vector2(0.9f, 0.95f));
                        }
                        else
                        { 
                            PlaySFX(uiClickAudioClip, new Vector2(1.05f, 1.1f));
                        }
                    }
                    else
                    {
                        PlaySFX(uiClickAudioClip, new Vector2(1.05f, 1.1f));
                    }
                }
                else
                {
                    // 빈 공간을 클릭한 경우
                    // SFX 출력
                    PlaySFX(uiClickAudioClip, new Vector2(0.9f, 0.95f));
                    // TODO 파티클 실행
                    
                }
            }
        }

        private bool GetOverlapUI(PointerEventData pointerEventData ,out GameObject overlapObject)
        {
            // 클릭한 위치에 해당하는 UI가 있는지 확인
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);
            
            // 레이어 마스크를 적용하여 결과 필터링
            results.RemoveAll(result => ((1 << result.gameObject.layer) & raycastMask) == 0);
            
            if (results.Count > 0)
            {
                // UI 요소를 클릭한 경우
                overlapObject = results[0].gameObject;
                return true;
            }

            // 빈 공간을 클릭한 경우
            overlapObject = null;
            return false;
        }
        
        private void PlaySFX(AudioClip audioClip, Vector2 pitchVec)
        {
            sfxPlayer.SetAudioClip(audioClip);
            sfxPlayer.PlayWithRandomPitch(pitchVec);
        }

        public void PlayClickSFX()
        {
            PlaySFX(uiClickAudioClip, new Vector2(1.05f, 1.1f));
        }
        
        #region Init
        
        private void InitComponent()
        {
            if (graphicRaycaster == null)
            {
                Debug.LogError("[ScreenClickHandler] GraphicRaycaster가 설정되어있지 않습니다!");
            }

            if (sfxPlayer == null)
            {
                sfxPlayer = GetComponentInChildren<SFXPlayer>();
            }
        }

        #endregion

        #region Odin

        private bool IsSetGraphicRaycaster()
        {
            return graphicRaycaster == null;
        }

        #endregion
    }
}