using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class MoveComponent : MonoBehaviour
{
    NavMeshAgent m_Agent;
    //RaycastHit m_HitInfo = new RaycastHit();
    CharacterController m_Controller;

    public float capsuleHeightStanding = 1.8f;

    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 10f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;
    [Tooltip("Max movement speed when crouching")]
    [Range(0, 1)]
    public float maxSpeedCrouchedRatio = 0.5f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;
    [Tooltip("Max movement Horizontal speed when not grounded")]
    public float maxSpeedInAir = 10f;

    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;
    public float RotationMultiplier
    {
        get
        {
            return 1f;
        }
    }

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 9f;

    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;

    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;
    const float k_GroundCheckDistanceInAir = 0.07f;

    //地面法线
    Vector3 m_GroundNormal;

    //最后一次跳跃的时间
    float m_LastTimeJumped = 0f;

    //跳跃后多久可以开始检测落地
    const float k_JumpGroundingPreventionTime = 0.2f;

    InputComponent m_InputComponent;
    AnimatorComponent m_AnimatorComponent;

    public bool isCrouching { get; private set; }
    public Vector3 characterVelocity { get; set; }

    bool isGrounded = true;


    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        DebugComponent.HandleErrorIfNullGetComponent<CharacterController, MoveComponent>(m_Controller, this, gameObject);

        m_InputComponent = GetComponent<InputComponent>();
        DebugComponent.HandleErrorIfNullGetComponent<InputComponent, MoveComponent>(m_InputComponent, this, gameObject);

        m_AnimatorComponent = GetComponent<AnimatorComponent>();
        DebugComponent.HandleErrorIfNullGetComponent<AnimatorComponent, MoveComponent>(m_AnimatorComponent, this, gameObject);

        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.updatePosition = true;

        m_Controller.enableOverlapRecovery = true;
    }

    void Update()
    {
        //if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        //{

        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit m_HitInfo))
        //        m_Agent.destination = m_HitInfo.point;
        //}

        GroundCheck();
        HandleCharacterMovement();
    }

    void GroundCheck()
    {

        float chosenGroundCheckDistance = isGrounded ? (m_Controller.skinWidth + groundCheckDistance) : k_GroundCheckDistanceInAir;
        isGrounded = false;
        //m_Agent.updatePosition = false;

        m_GroundNormal = Vector3.up;
         
        if (Time.time >= m_LastTimeJumped + k_JumpGroundingPreventionTime) {
            
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(), m_Controller.radius, Vector3.down, out RaycastHit hitInfo, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore)) {

                m_GroundNormal = hitInfo.normal;
                //只有方向相同才是有效的命中地板
                if (Vector3.Dot(hitInfo.normal, transform.up) > 0f && isNormalUnderSlopeLimit(m_GroundNormal))
                {
                    isGrounded = true;
                    m_Agent.updatePosition = true;
                    Jump(false);
                    //if (hitInfo.distance > m_Controller.skinWidth)
                    //{
                    //    m_Controller.Move(Vector3.down * hitInfo.distance);
                    //}
                }
            }
        }
    }

    void HandleCharacterMovement()
    {

        //角色水平旋转
        {
            transform.Rotate(new Vector3(0f, (m_InputComponent.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self);
        }

        Vector3 worldspaceMoveInput = transform.TransformVector(m_InputComponent.GetMoveInput());

        bool isSprinting = m_InputComponent.GetSprintInputHeld();
        float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

        if (isGrounded)
        {
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;

            if (isCrouching)
            {
                targetVelocity *= maxSpeedCrouchedRatio;
            }

            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, m_GroundNormal) * targetVelocity.magnitude;

            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            if (isGrounded && m_InputComponent.GetJumpInputDown())
            {
                characterVelocity = new Vector3(characterVelocity.x, 0, characterVelocity.z);
                characterVelocity += Vector3.up * jumpForce;

                m_LastTimeJumped = Time.time;
                isGrounded = false;
                m_Agent.updatePosition = false;
                Jump(true);
            }
        }
        else 
        {
            characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);

            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        Move(characterVelocity * Time.deltaTime);
        State(worldspaceMoveInput);
        //RotateDir(transform.position + worldspaceMoveInput);
    }

    public void Jump(bool Bool)
    {
        m_AnimatorComponent.SetJump(Bool);
    }

    //public void RotateDir(Vector3 pos) {
    //    Debug.Log("moveInput.normalized " + pos.normalized);

    //    Quaternion lookAtRotation = Quaternion.LookRotation(pos);
    //    //Quaternion lookAtRotationOnly_Y = Quaternion.Euler(transform.rotation.eulerAngles.x, lookAtRotation.y, transform.rotation.eulerAngles.z);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 0.1f);
    //}

    public void State(Vector3 moveInput) {
        m_AnimatorComponent.SetVelocity(new Vector2(moveInput.x * 2, moveInput.z * 2).sqrMagnitude);
    }

    public void Move(Vector3 velocity)
    {
        m_Controller.Move(velocity);
    }

    //获取与给定斜率相切的方向
    public Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }


    bool isNormalUnderSlopeLimit(Vector3 normal) {
        return Vector3.Angle(transform.up, normal) < m_Controller.slopeLimit;
    }

    //获得CharacterControl底部半球的中心点
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * m_Controller.radius);
    }

    //获得CharacterControl顶部半球的中心点
    Vector3 GetCapsuleTopHemisphere()
    {
        return transform.position + (transform.up * (m_Controller.height - m_Controller.radius));
    }



}
