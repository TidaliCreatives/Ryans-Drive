using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugLightControl : MonoBehaviour
{
    Light myLight;
    float maxIntensity;
    float step = 0.2f;


    private void Start()
    {
        myLight = GetComponent<Light>();
        maxIntensity = myLight.intensity;
    }

    DefaultInputActions inputActions;

    public void OnEnable()
    {
        inputActions ??= new DefaultInputActions();

        inputActions.Debug.One.performed += DecreaseIntensity;
        inputActions.Debug.Two.performed += IncreaseIntensity;

        inputActions.Debug.Enable();
    }

    private void OnDisable()
    {
        inputActions.Debug.One.performed -= DecreaseIntensity;
        inputActions.Debug.Two.performed -= IncreaseIntensity;
        inputActions.Debug.Disable();
    }

    void DecreaseIntensity(InputAction.CallbackContext ctx)
    {
        if (myLight.intensity - step < 0f)
        {
            myLight.intensity = 0f;
        }
        else
        {
            myLight.intensity -= step;
        }
    }

    void IncreaseIntensity(InputAction.CallbackContext ctx)
    {
        if (myLight.intensity + step > maxIntensity)
        {
            myLight.intensity = maxIntensity;
        }
        else
        {
            myLight.intensity += step;
        }
    }
}