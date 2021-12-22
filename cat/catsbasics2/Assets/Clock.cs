using UnityEngine;
public class Clock : MonoBehaviour
{
    [SerializeField]
    Transform hoursPivot, secondsPivot, minutesPivot;

    private void Awake()
    {
        Debug.Log(DateTime.Now.Hour);
        hoursPivot.localRotation = Quaternion.Euler(0, 0, -30);
    }
}
