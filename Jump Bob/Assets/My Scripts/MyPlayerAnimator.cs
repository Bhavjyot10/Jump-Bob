using SupanthaPaul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupanthaPaul
{
    public class MyPlayerAnimator : MonoBehaviour
    {
        private Rigidbody2D m_rb;
        private MyPlayerController m_controller;
        private Animator m_anim;
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int JumpState = Animator.StringToHash("JumpState");
        private static readonly int IsJumping = Animator.StringToHash("IsJumping");
        private static readonly int IsDashing = Animator.StringToHash("IsDashing");

        private void Start()
        {
            m_anim = GetComponentInChildren<Animator>();
            m_controller = GetComponent<MyPlayerController>();
            m_rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // Idle & Running animation
            m_anim.SetFloat(Move, Mathf.Abs(m_rb.velocity.x));

            // Jump state (handles transitions to falling/jumping)
            float verticalVelocity = m_rb.velocity.y;
            m_anim.SetFloat(JumpState, verticalVelocity);

            // Jump animation
            if (!m_controller.isGrounded)
            {
                m_anim.SetBool(IsJumping, true);
            }
            else
            {
                m_anim.SetBool(IsJumping, false);
            }

            // dash animation
            m_anim.SetBool(IsDashing, m_controller.isDashing);
        }
    }
}
