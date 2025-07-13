using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MusicZone : MonoBehaviour
{
    public AudioClip musicClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManager.Instance.PlayMusic(musicClip);
        }
    }
}
