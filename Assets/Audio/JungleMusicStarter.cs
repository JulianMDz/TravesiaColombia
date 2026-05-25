using UnityEngine;

public class JungleMusicStarter : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayJungleMusic();
        }
    }
}