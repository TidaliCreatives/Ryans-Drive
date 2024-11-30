using UnityEngine;

public class IsolateLightController : MonoBehaviour
{
    [Header("Turn On Delay")]
    [SerializeField] bool doStartAfterDelay = false;
    [SerializeField] bool doFancyStart = false;
    [SerializeField] float startUpDelay = 0.5f;
    [SerializeField] float startUpTime = 0.5f;
    float startTime;
    float initialIntensity;
    bool isOn;
    bool hasPlayedSound = false;
    Light myLight;

    private void Start()
    {
        // Set the start time
        startTime = Time.time;

        // Get the initial intensity of the light
        if (TryGetComponent(out Light light))
        {
            myLight = light;
            initialIntensity = myLight.intensity;
        }
        else
        {
            // Debuglog error
            Debug.LogError("No light component found on " + gameObject.name);
        }

        // If the light is supposed to start after delay
        if (doStartAfterDelay)
        {
            myLight.intensity = 0;
        }

        // If the light is supposed to start fancy
        if (doFancyStart)
        {
            startUpDelay = Vector3.Distance(GameObject.FindGameObjectWithTag("FirstPersonController").transform.position, this.transform.position) / 20f;
        }
    }
    private void Update()
    {
        if (doStartAfterDelay && !isOn && Time.time - startTime > startUpDelay)
        {
            myLight.intensity = initialIntensity * Mathf.Clamp01((Time.time - startTime - startUpDelay) / startUpTime);

            if ((Time.time - startTime - startUpDelay) >= startUpTime)
            {
                isOn = true;
            }

            if (doStartAfterDelay && !hasPlayedSound && Time.time - startTime > startUpDelay)
            {
                GetComponent<AudioSource>().Play();
                hasPlayedSound = true;
            }
        }
    }
}