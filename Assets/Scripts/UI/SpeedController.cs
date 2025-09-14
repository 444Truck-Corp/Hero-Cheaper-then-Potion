using UnityEngine;
using UnityEngine.UI;

public class SpeedController : MonoBehaviour
{
    [SerializeField] private Outline _imageFast;

    private bool _isFast;

    public void OnClickTogleSpeedButton()
    {
        _isFast = !_isFast;
        _imageFast.enabled = !_imageFast.enabled;
        Time.timeScale = _isFast ? 2.0f : 1.0f;
    }
}