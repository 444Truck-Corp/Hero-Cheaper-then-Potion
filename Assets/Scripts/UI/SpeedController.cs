using UnityEngine;

public class SpeedController : MonoBehaviour
{
    [SerializeField] GameObject _imageFast;

    private bool _isFast;

    public void OnClickTogleSpeedButton()
    {
        _isFast = !_isFast;
        _imageFast.SetActive(_isFast);
        Time.timeScale = _isFast ? 2.0f : 1.0f;
    }
}