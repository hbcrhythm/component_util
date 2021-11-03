using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentComponent : MonoBehaviour
{

    public Transform target;
    public bool isMoveToTarget;

    NavMeshAgent m_Agent;

    NavMeshPath m_Path;

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        DebugComponent.HandleErrorIfNullGetComponent<NavMeshAgent, MoveComponent>(m_Agent, this, gameObject);

        m_Path = new NavMeshPath();
        m_Agent.updatePosition = false;

    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        //{

        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit m_HitInfo))
        //        m_Agent.destination = m_HitInfo.point;
        //}


        Debug.Log("NavMeshAgent.isOnOffMeshLink " + m_Agent.isOnOffMeshLink);
        if (isMoveToTarget)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 1.0f)
            {
                elapsed -= 1.0f;
                FindPath();
            }

            OnDraw();


            //Debug.Log("updatePosition " + m_Agent.updatePosition);
            //m_Agent.updatePosition = true;
            //m_Agent.destination = target.position;
        }
    }

    private float elapsed = 0.0f;
    void FindPath()
    {
        NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, m_Path);
    }

    private void OnDraw()
    {
        if (m_Path != null)
        {
            m_Agent.SetDestination(target.position);
            for (int i = 0; i < m_Path.corners.Length - 1; i++)
            {
                Debug.DrawLine(m_Path.corners[i], m_Path.corners[i + 1], Color.red);
            }
        }
    }

}
