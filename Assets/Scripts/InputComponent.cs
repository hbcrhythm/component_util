using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
    [Header("Variable")]
    [Tooltip("Sensitivity multiplier for moving the camera around")]
    public float lookSensitivity = 1f;
    [Tooltip("Additional sensitivity multiplier for WebGL")]
    public float webglLookSensitivityMultiplier = 0.25f;
    [Tooltip("Limit to consider an input when using a trigger on a controller")]
    public float triggerAxisThreshold = 0.4f;
    [Tooltip("Used to flip the vertical input axis")]
    public bool invertYAxis = false;
    [Tooltip("Used to flip the horizontal input axis")]
    public bool invertXAxis = false;

    #region Input Constants

    [Header("Input Constants")]
    public string k_AxisNameVertical = "Vertical";
    public string k_AxisNameHorizontal = "Horizontal";
    public string k_MouseAxisNameVertical = "Mouse Y";
    public string k_MouseAxisNameHorizontal = "Mouse X";
    public string k_AxisNameJoystickLookVertical = "Look Y";
    public string k_AxisNameJoystickLookHorizontal = "Look X";
    public string k_ButtonNameJump = "Jump";
    public string k_ButtonNameFire = "Fire";
    public string k_ButtonNameGamepadFire = "Gamepad Fire";
    public string k_ButtonNameSprint = "Sprint";
    public string k_ButtonNameCrouch = "Crouch";
    public string k_ButtonNameAim = "Aim";
    public string k_ButtonNameGamepadAim = "Gamepad Aim";
    public string k_ButtonNameSwitchWeapon = "Mouse ScrollWheel";
    public string k_ButtonNameGamepadSwitchWeapon = "Gamepad Switch";
    public string k_ButtonNameNextWeapon = "NextWeapon";
    public string k_ButtonNamePauseMenu = "Pause Menu";
    public string k_ButtonNameSubmit = "Submit";
    public string k_ButtonNameCancel = "Cancel";
    public string k_ButtonNameOver = "Over";

    #endregion


    bool m_FireInputWasHeld;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        m_FireInputWasHeld = GetFireInputHeld();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("h " + Input.GetAxis("Mouse X") + " " + Input.GetAxis("Horizontal"));
    }


    public bool CanInput()
    {
        //return Cursor.lockState == CursorLockMode.Locked;
        return true;
    }

    public bool GetFireInputHeld()
    {
        if (CanInput())
        {
            bool isGamepad = Input.GetAxis(k_ButtonNameGamepadFire) != 0f;
            if (isGamepad)
            {
                return Input.GetAxis(k_ButtonNameGamepadFire) >= triggerAxisThreshold;
            }
            else
            {
                return Input.GetButton(k_ButtonNameFire);
            }
        }
        return false;
    }

    public bool GetSprintInputHeld()
    {
        if (CanInput())
        {
            return Input.GetButton(k_ButtonNameSprint);
        }
        return false;
    }
    public bool GetJumpInputDown()
    {
        if (CanInput())
        {
            return Input.GetButton(k_ButtonNameJump);
        }
        return false;
    }

    public Vector3 GetMoveInput()
    {
        if (CanInput())
        {
            Vector3 move = new Vector3(Input.GetAxisRaw(k_AxisNameHorizontal), 0f, Input.GetAxisRaw(k_AxisNameVertical));
            move = Vector3.ClampMagnitude(move, 1);
            return move;
        }
        return Vector3.zero;
    }


    public bool GetOverInputDown()
    {
        if (CanInput())
        {
            return Input.GetButton(k_ButtonNameOver);
        }
        return false;
    }

    public float GetLookInputsHorizontal()
    {
        return GetMouseOrStickLookAxis(k_MouseAxisNameHorizontal, k_AxisNameJoystickLookHorizontal);
    }

    public float GetLookInputsVertical()
    {
        return GetMouseOrStickLookAxis(k_MouseAxisNameVertical, k_AxisNameJoystickLookVertical);
    }

    float GetMouseOrStickLookAxis(string mouseInputName, string stickInputName) 
    {
        if (CanInput())
        {
            bool isGamepad = Input.GetAxis(stickInputName) != 0f; //判断输入是否来自手柄
            float i = isGamepad ? Input.GetAxis(stickInputName) : Input.GetAxisRaw(mouseInputName);

            if (invertYAxis)
                i *= -1f;

            i *= lookSensitivity;

            if (isGamepad)
            {
                i *= Time.deltaTime;
            }
            else
            {
                i *= 0.01f;
#if UNITY_WEBGL
            // Mouse tends to be even more sensitive in WebGL due to mouse acceleration, so reduce it even more
            i *= webglLookSensitivityMultiplier;
#endif
            }
            return i;
        }

        return 0f;
    }
}
