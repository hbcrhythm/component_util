using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    InputComponent m_inputComponent;
    // Start is called before the first frame update
    void Start()
    {
        m_inputComponent = GetComponent<InputComponent>();
        DebugComponent.HandleErrorIfNullGetComponent<InputComponent, TestComponent>(m_inputComponent, this, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        Test_T();
    }

    void Test_T() {
        if (m_inputComponent.GetTInputDown())
        {
            Debug.Log("shake TestComponent");
            CameraShake.shake();
        }
    }

}
