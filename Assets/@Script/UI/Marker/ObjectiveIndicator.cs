using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[System.Flags]
public enum HideReason
{
    None = 0,
    Hover = 1 << 0, // 상호 작용 대상 위에 커서
    Dialogue = 1 << 1, // 대화 중
    Cutscene = 1 << 2, // 컷신 중
}

public enum ObjectiveAction
{
    None,
    PickUp,
    Follow,
    Conversation,
    Push,
}
namespace Marker
{
    public class ObjectiveIndicator : MonoBehaviour
    {
        [SerializeField] private RectTransform CanvasRT;

        [SerializeField] private bool showWorldLabel;
        [SerializeField] private bool showEdgeLabel = true;

        [SerializeField] private UIScriptData scriptData;
        [SerializeField] private float edgePadding = 40f;
        [SerializeField] float worldMarkerYOffset = 40f;

        [SerializeField] CanvasGroup indicatorGroup;
        [SerializeField] private List<Slot> _slots;
        private Camera cam;
        
        [Header("Follow Delay")]
        [SerializeField] private float followShowDelay = 10f;
        
        [TabGroup("Action", "Type")] public SerializableDictionary<ObjectiveAction, Sprite> actionSprites;

        public void SetCamera(Camera c) => cam = c;
        
        [SerializeField] private HideReason hideMask = HideReason.None;
        private bool EffectiveVisible => _isVisible && hideMask == HideReason.None;
        private bool _isVisible = true;

        // ===== 멀티 슬롯 구조 =====
        [Serializable]
        private class Slot
        {
            public RectTransform worldMarker;
            public RectTransform edgeIndicator;

            public TextMeshProUGUI worldLabel;
            public RectTransform leftArrow;
            public RectTransform rightArrow;

            public Image worldActionIcon;
            public Image edgeActionIcon;

            public QuestTarget target;
            public ObjectiveAction action;
            
            public bool active;

            public string markerLabel;
            public bool waiting;
            public float showAt;
        }


        /// <summary> QuestUI처럼 전체 인디케이터 UI를 페이드 토글 </summary>
        public void SetHidden(HideReason reason, bool hidden, float duration = 0.1f)
        {
            var wasVisible = EffectiveVisible;

            if (hidden) hideMask |= reason;
            else hideMask &= ~reason;

            var nowVisible = EffectiveVisible;
            if (wasVisible == nowVisible) return; // 상태 변화 없으면 페이드 안 함

            FadeTo(nowVisible, duration);
        }

        private void FadeTo(bool visible, float duration)
        {
            if (!indicatorGroup) return;

            indicatorGroup.DOKill();
            indicatorGroup.DOFade(visible ? 1f : 0f, duration);
            indicatorGroup.interactable   = visible;
            indicatorGroup.blocksRaycasts = visible;
        }

        /// <summary> (호환) 단일 타깃을 슬롯0에 팔로우 </summary>
        public void Follow(QuestTarget t,ObjectiveAction action = ObjectiveAction.None, float? delayOverride = null) => FollowAt(0, t, action, delayOverride);

        /// <summary> 특정 슬롯에 타깃 지정 </summary>
        public void FollowAt(int slotIndex, QuestTarget t, ObjectiveAction action = ObjectiveAction.None, float? delayOverride = null)
        {
            if (!InRange(slotIndex))
                return;

            var s = _slots[slotIndex];
            s.target = t;
            s.active = (t is not null);
            
            s.action = action;
            float d = delayOverride ?? followShowDelay;
            if (s.active && d > 0f)
            {
                s.waiting = true;
                s.showAt = Time.time + d;
            }
            else
            {
                s.waiting = false;
                s.showAt = 0f;
            }
            // 초기엔 표시 결정을 Update에 맡김
            SafeSetActive(s.worldMarker, false);
            SafeSetActive(s.edgeIndicator, false);

            // 라벨 텍스트 반영
            ApplyLabelVisibilityAndText(s);
            ApplyActionSpriteOnly(s);
            if (!cam) cam = Camera.main;
        }

        private void ApplyActionSpriteOnly(Slot s)
        {
            Sprite sp = null;
            if (actionSprites != null)
                actionSprites.TryGetValue(s.action, out sp);
            if (s.worldActionIcon)
            {
                if (s.worldActionIcon.sprite != sp)
                    s.worldActionIcon.sprite = sp;
            }

            if (s.edgeActionIcon)
            {
                if (s.edgeActionIcon.sprite != sp)
                    s.edgeActionIcon.sprite = sp;    
            }
        }
        /// <summary> 슬롯별 현지화 키로 라벨 설정 </summary>
        public void SetMarkerByKeyAt(int slotIndex, string key)
        {
            if (!InRange(slotIndex)) return;
            string txt = string.IsNullOrEmpty(key) ? string.Empty
                : scriptData?.GetText(key, ScriptDataType.Quest) ?? string.Empty;

            _slots[slotIndex].markerLabel = txt;
            ApplyLabelVisibilityAndText(_slots[slotIndex]);
        }

        /// <summary> (호환) 슬롯0에 현지화 키로 라벨 설정 </summary>
        public void SetMarkerByKey(string key) => SetMarkerByKeyAt(0, key);

        /// <summary> 특정 슬롯 클리어 </summary>
        public void ClearAt(int slotIndex)
        {
            if (!InRange(slotIndex)) 
                return;
            var s = _slots[slotIndex];
            
            s.target = null;
            s.active = false;
            s.waiting = false;
            s.showAt = 0f;
            
            SafeSetActive(s.worldMarker, false);
            SafeSetActive(s.edgeIndicator, false);
            s.action  = ObjectiveAction.None;
            if (s.worldActionIcon) 
                s.worldActionIcon.gameObject.SetActive(false);
            if (s.edgeActionIcon)  
                s.edgeActionIcon.gameObject.SetActive(false);
        }

         public void ClearAll()
        {
            for (int i = 0; i < _slots.Count; i++)
                ClearAt(i);
        }

        private void Update()
        {
            if (!EffectiveVisible) 
                return;
            if (!cam)
            {
                cam = Camera.main;
                if (!cam) 
                    return;
            }

            for (int i = 0; i < _slots.Count; i++)
                TickSlot(_slots[i]);
        }
        private static void SafeSetActive(Component c, bool v)
        {
            if (!c) 
                return;

            if (c.TryGetComponent<CanvasGroup>(out var cg))
            {
                cg.alpha = v ? 1f : 0f;               
                cg.interactable = v;                  
                cg.blocksRaycasts = v;
            }
        }

        private static void SafeSetImageAcitve(Component c, bool v)
        {
            c.gameObject.SetActive(v);
        }

        private bool InRange(int i) => _slots != null && i >= 0 && i < _slots.Count && i < _slots.Count;

        private void ApplyLabelVisibilityAndText(Slot s)
        {
            bool has = !string.IsNullOrEmpty(s.markerLabel);

            if (s.worldLabel)
            {
                if (s.worldLabel.text != s.markerLabel)
                    s.worldLabel.text = s.markerLabel;
                s.worldLabel.gameObject.SetActive(showWorldLabel && has);
            }
        }

        private void TickSlot(Slot s)
        {
            if (!s.active || !s.target)
            {
                SafeSetActive(s.worldMarker, false);
                SafeSetActive(s.edgeIndicator, false);
                return;
            }
            
            var anchor = s.target.Anchor;
            if (!anchor)
            {
                // 타깃이 파괴되었으면 슬롯 정리
                s.target = null;
                s.active = false;
                s.waiting = false;
                SafeSetActive(s.worldMarker, false);
                SafeSetActive(s.edgeIndicator, false);
                return;
            }
            
            if (s.waiting)
            {
                if (Time.time < s.showAt)
                {
                    SafeSetActive(s.worldMarker, false);
                    SafeSetActive(s.edgeIndicator, false);
                    return;
                }
                // 지연 해제 → 이제부터 표시 허용
                s.waiting = false;
            }
            
            // 너무 멀면 숨김
            if (Vector3.Distance(cam.transform.position, s.target.Anchor.position) > s.target.ActivationDistance)
            {
                SafeSetActive(s.worldMarker, false);
                SafeSetActive(s.edgeIndicator, false);
                return;
            }

            Vector3 vp3 = cam.WorldToViewportPoint(s.target.Anchor.position);
            bool front = vp3.z > 0f;
            Vector2 p = new Vector2(vp3.x, vp3.y);
            if (!front) p = new Vector2(1f - p.x, 1f - p.y);

            bool onScreen = front && p.x >= 0f && p.x <= 1f && p.y >= 0f && p.y <= 1f;

            if (onScreen)
            {
                // 월드 마커
                Vector2 screenPt = cam.WorldToScreenPoint(s.target.Anchor.position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRT, screenPt, null, out var local);

                if (s.worldMarker)
                {
                    s.worldMarker.anchoredPosition = local + Vector2.up * s.target.yOffset;
                    s.worldMarker.localRotation = Quaternion.identity;
                    SafeSetActive(s.worldMarker, true);
                }

                SafeSetActive(s.edgeIndicator, false);

                // 라벨/회전
                if (s.worldLabel)
                {
                    s.worldLabel.rectTransform.localRotation = Quaternion.identity;
                    s.worldLabel.gameObject.SetActive(showWorldLabel);
                    s.worldLabel.text = s.markerLabel; // 최신화
                }
                if (s.worldActionIcon)
                {
                    bool hasSprite = s.worldActionIcon.sprite != null;
                    s.worldActionIcon.gameObject.SetActive(hasSprite);
                }
                if (s.edgeActionIcon)
                    s.edgeActionIcon.gameObject.SetActive(false);
                return;
            }

            // 오프스크린: 좌/우만
            SafeSetActive(s.worldMarker, false);
            SafeSetActive(s.edgeIndicator, true);

            bool goRight = p.x >= 0.5f;
            SafeSetImageAcitve(s.leftArrow, !goRight);
            SafeSetImageAcitve(s.rightArrow, goRight);

            float needW = LayoutUtility.GetPreferredSize(s.edgeIndicator, 0);
            float needH = LayoutUtility.GetPreferredSize(s.edgeIndicator, 1);
            Vector2 half = CanvasRT.rect.size * 0.5f - new Vector2(edgePadding, edgePadding);

            float x = goRight ? (half.x - needW * 0.5f) : (-half.x + needW * 0.5f);
            float yMin = -half.y + needH * 0.5f;
            float yMax = half.y - needH * 0.5f;
            float y = Mathf.Lerp(yMin, yMax, Mathf.Clamp01(p.y));

            s.edgeIndicator.anchoredPosition = new Vector2(x, y);
            s.edgeIndicator.localRotation = Quaternion.identity;
            if (s.edgeActionIcon)
            {
                bool hasSprite = s.edgeActionIcon.sprite != null;
                s.edgeActionIcon.gameObject.SetActive(hasSprite);
            }
            if (s.worldActionIcon)
                s.worldActionIcon.gameObject.SetActive(false);

        }
    }
}