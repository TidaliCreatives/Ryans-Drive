using UnityEngine;

public class TVLights : MonoBehaviour
{
    Light tvLight;
    [SerializeField] Color[] colors;
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float maxIntensity = 2.0f;
    [SerializeField] float minRange = 2.0f;
    [SerializeField] float maxRange = 5.0f;
    [SerializeField] float minFlickerSpeed = 1f;
    [SerializeField] float maxFlickerSpeed = 4f;
    float currentFlickerSpeed;

    private float timer;

    void Start()
    {
        if (tvLight == null)
        {
            tvLight = GetComponent<Light>();
        }
        currentFlickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentFlickerSpeed)
        {
            // Reset the timer
            timer -= currentFlickerSpeed;

            // Set intensity to a random value between minIntensity and maxIntensity
            tvLight.intensity = Random.Range(minIntensity, maxIntensity);

            // Set the color of the light to a random color from the colors array
            if (colors.Length > 0)
            {
                tvLight.color = colors[Random.Range(0, colors.Length)];
            }

            // Set the range of the light to a random value between minRange and maxRange
            tvLight.range = Random.Range(minRange, maxRange);

            // Set the flicker speed to a random value between minFlickerSpeed and maxFlickerSpeed
            currentFlickerSpeed = Random.Range(minFlickerSpeed, maxFlickerSpeed);
        }
    }
}