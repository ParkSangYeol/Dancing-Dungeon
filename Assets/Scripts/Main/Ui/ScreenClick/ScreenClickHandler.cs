using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Main.UI
{
    public class ScreenClickHandler : MonoBehaviour
    {
        #region Variables
        
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

        [SerializeField] [AssetsOnly] 
        private UIParticle clickVFX;
        
        private ScreenClickParticlePool particlePool;

        private int recentClick;
        
        #endregion
        
        void Start()
        {
            InitComponent();
            InitVariable();
        }

        void Update()
        {
            HandleMouseClick();
        }

        #region HandleMouseClick
        
        private void HandleMouseClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 클릭 위치를 가져옴
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Input.mousePosition;
                Vector3 ClickPosition = pointerEventData.position;
                
                // 클릭 위치에 파티클 생성
                // UIParticle을 사용하지 않고 ParticleSystem을 사용하는 경우
                // 아래의 주석 처리된 코드를 이용하여 좌표를 변환할 수 있음. 
                // Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(clickPosition);
                PlayScreenClickParticle(ClickPosition);
                
                // 마우스 클릭시 실행
                if (GetOverlapUI(pointerEventData, out var overlapObject))
                {
                    // UI를 클릭한 경우
                    WhenClickUI(overlapObject);
                }
                else
                {
                    // 빈 공간을 클릭한 경우
                    WhenClickBlank(pointerEventData.position);
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

        private void WhenClickUI(GameObject overlapObject)
        {
            // UI를 클릭한 경우
            Debug.Log("[ScreenClickHandler] clicked object: " + overlapObject);
            // SFX 출력. UI 종류에 따른 처리 진행
            if (overlapObject.CompareTag("NegativeSFXUI"))
            {
                PlayNegativeSFX();
            }
            else if (overlapObject.TryGetComponent<Toggle>(out var toggle))
            {
                // Toggle인 경우
                if (toggle.isOn)
                {
                    PlayNegativeSFX();
                }
                else
                { 
                    PlayPositiveSFX();
                }
            }
            else if (overlapObject.TryGetComponent<TMP_Dropdown>(out var dropdown))
            {
                // DropDown인 경우
                /*
                 * TMP_Dropdown의 isExpanded를 사용하는 경우 단순히 게임 오브젝트가 존재하는지 확인하고,
                 * 결과를 리턴하기 때문에 플레이어가 Dropdown을 연타하여 사라지는 도중에 다시 펼쳐지는 경우를
                 * 인식하지 못함. 이에 따라 일부 구현하여 처리
                 */
                if (dropdown.IsExpanded)
                {
                    Debug.Log("[ScreenClickHandler] Dropdown called. recent id is " + recentClick + " currentID is " + overlapObject.GetInstanceID());
                    // Dropdown List가 존재함. 이 경우 추가 확인 진행.
                    if (overlapObject.GetInstanceID() == recentClick)
                    {
                        Debug.Log("[ScreenClickHandler] Dropdown Expand & click double.");
                        // dropdown을 두 번 클릭한 경우로 dropdown이 열린 상태로 볼 수 있음.
                        PlayNegativeSFX();
                        recentClick = -1;
                    }
                    else
                    {
                        Debug.Log("[ScreenClickHandler] Dropdown Expand & click once.");
                        // dropdown이 처음 눌린 상태로 볼 수 있음.
                        PlayPositiveSFX();
                    }
                }
                else
                {
                    Debug.Log("[ScreenClickHandler] Dropdown UnExpand.");
                    // Dropdown List가 존재하지 않음. 확정적으로 닫힌 상태.
                    PlayPositiveSFX();
                }
            }
            else
            {
                PlayPositiveSFX();
            }

            recentClick = recentClick == -1? 0 : overlapObject.GetInstanceID();
        }


        private void WhenClickBlank(Vector3 clickPosition)
        {
            // SFX 출력
            PlayNegativeSFX();
        }

        #endregion
        
        #region About SFX

        private void PlayPositiveSFX()
        {
            PlaySFX(uiClickAudioClip, new Vector2(1.05f, 1.1f));
        }
        
        private void PlayNegativeSFX()
        {
            PlaySFX(uiClickAudioClip, new Vector2(0.9f, 0.95f));
        }
        
        private void PlaySFX(AudioClip audioClip, Vector2 pitchVec, bool dontPlayWhenPlaying = false)
        {
            sfxPlayer.SetAudioClip(audioClip);
            sfxPlayer.PlayWithRandomPitch(pitchVec, dontPlayWhenPlaying);
        }

        public void PlayClickSFX(bool dontPlayWhenPlaying = false)
        {
            PlaySFX(uiClickAudioClip, new Vector2(1.05f, 1.1f), dontPlayWhenPlaying);
        }
        
        #endregion

        #region About VFX

        private void PlayScreenClickParticle(Vector3 spawnPosition)
        {
            ScreenClickParticle particle = particlePool.GetScreenClickParticle();
            particle.transform.position = spawnPosition;
            particle.PlayParticle();
        }
        
        public UIParticle InstantiateParticle(UIParticle VFX)
        {
            return Instantiate(VFX, transform);
        }

        public void DestroyParticle(GameObject particle)
        {
            Destroy(particle);
        }
        
        #endregion
        
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

        private void InitVariable()
        {
            // Particle 생성
            if (clickVFX == null)
            {
                Debug.LogError("[ScreenClickHandler] ClickVFX가 설정되어 있지 않습니다.");
            }
            else
            {
                particlePool = new ScreenClickParticlePool(clickVFX, Vector3.zero, this, 3);
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