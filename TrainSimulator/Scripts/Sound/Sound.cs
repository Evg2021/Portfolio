using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;

    private void OnValidate()
    {
        _source = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        _source.PlayOneShot(clip);
    }
}
