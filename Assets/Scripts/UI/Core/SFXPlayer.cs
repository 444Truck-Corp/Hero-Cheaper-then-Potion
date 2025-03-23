using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public void Play(int index)
    {
        AudioManager.Instance.PlaySFX(index);
    }
}