using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class OverTheWallComponent : MonoBehaviour
{

    public Transform Player;
    public Transform Wall;

    AnimatorComponent m_AnimatorComponent;
    InputComponent m_InputComponent;
    NavMeshAgent m_NavMeshAgent;

    public bool isDrawGizmos = false;
    bool isJump = false;

    // Start is called before the first frame update
    void Start()
    {
        m_AnimatorComponent = GetComponent<AnimatorComponent>();
        DebugComponent.HandleErrorIfNullGetComponent<AnimatorComponent, OverTheWallComponent>(m_AnimatorComponent, this, gameObject);

        m_InputComponent = GetComponent<InputComponent>();
        DebugComponent.HandleErrorIfNullGetComponent<InputComponent, OverTheWallComponent>(m_InputComponent, this, gameObject);

        //m_NavMeshAgent = GetComponent<NavMeshAgent>();
        //DebugComponent.HandleErrorIfNullGetComponent<NavMeshAgent, OverTheWallComponent>(m_NavMeshAgent, this, gameObject);

        DebugComponent.HandleErrorIfNullFindObject<GameObject, OverTheWallComponent>(Wall, this);
        DebugComponent.HandleErrorIfNullFindObject<GameObject, OverTheWallComponent>(Player, this);

        //var size = Wall.GetComponent<Renderer>().bounds.size;
        //Debug.Log("size " + size);

    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetOver();
    }

    //ref : https://diego.assencio.com/?index=ec3d5dfdfc0b6a0d147a656f0af332bd#post_ec3d5dfdfc0b6a0d147a656f0af332bd_fig_1
    Vector3 Calc()
    {
        Vector3 x = Player.position;
        Vector3 p = Wall.transform.Find("StartPoint").position;
        Vector3 q = Wall.transform.Find("EndPoint").position;

        float k = Vector3.Dot((x - p), (q - p)) / Vector3.Dot((q - p), (q - p));

        Vector3 s = p + k * (q - p);
        return s;
    }

    void SetOver() {
        if (m_InputComponent.GetOverInputDown())
        {
            if (!isJump) {
                var newPos = Player.position + new Vector3(0, 0, 0.5f);

                if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 1f, 1))
                {
                    Player.DOJump(hit.position, 0.8f, 1, 0.867f, false);
                    m_AnimatorComponent.SetJump(true);
                    isJump = true;
                }
            }
        }
        else {
            isJump = false;
            m_AnimatorComponent.SetJump(false);
        }
    }

    private void OnDrawGizmos()
    {
        if(isDrawGizmos)
            Debug.DrawLine(Player.position, Calc(), Color.red);
    }
}
