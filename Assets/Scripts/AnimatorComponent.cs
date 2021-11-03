using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorComponent : MonoBehaviour
{

    Animator m_Animator;
    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        DebugComponent.HandleErrorIfNullGetComponent<Animator, MoveComponent>(m_Animator, this, gameObject);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetVelocity(float Speed) {
        m_Animator.SetFloat("Speed", Speed);
    }

    public void SetRun(float Speed) {
        m_Animator.SetInteger("State", 1);
        SetVelocity(Speed);
    }

    public void SetIdle() {
        m_Animator.SetInteger("State", 0);
    }

    public void SetOver(bool Bool) {
        //m_Animator.SetInteger("State", 2);
        m_Animator.SetBool("isOver", Bool);
    }

    public void SetJump(bool Bool) {
        m_Animator.SetBool("isJump", Bool);
    }

}
