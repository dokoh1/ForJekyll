using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class HomeAnimator : MonoBehaviour
    {
        private void OnEnable()
        {
            return;
            Animator[] animators = GetComponentsInChildren<Animator>(true);

            foreach (Animator animator in animators)
            {
                animator.enabled = false; // 먼저 끄고
                animator.enabled = true;  // 다시 켜서 강제 초기화

                animator.Update(0f); // 초기 상태 강제 반영
                animator.Play(0, 0, 1f);
                animator.Update(0f); // 끝 프레임 강제 반영
            }
        }
    }