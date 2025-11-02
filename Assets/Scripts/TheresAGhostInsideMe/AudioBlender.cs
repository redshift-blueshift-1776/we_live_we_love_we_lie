using UnityEngine;

public class AudioBlender : MonoBehaviour
{
    [SerializeField] public AudioSource song1;
    [SerializeField] public AudioSource song2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRatio(float r) {

        song1.volume = 2 * r - r * r;
        song2.volume = 1 - r * r;
    }
}
