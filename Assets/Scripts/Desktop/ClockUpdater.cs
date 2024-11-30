using UnityEngine;
using TMPro;

public class ClockUpdater : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI time;
    [SerializeField] TextMeshProUGUI date;
    //

    float timer;
    //
    private void Start()
    {
        timer = 0f;
    }
    //
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer -= 1f;

            string dateTime = System.DateTime.Now.ToString();

            date.text = dateTime.Split(' ')[0];
            time.text = dateTime.Split(' ')[1].Remove(5);
        }
    }
}