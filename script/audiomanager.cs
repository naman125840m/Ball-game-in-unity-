using UnityEngine;

public class audiomanager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public AudioClip coin;
    public AudioClip obstacall;
    public AudioSource audioSource;
    public static audiomanager instance;
      private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }




public void onobstacall()
    {
        audioSource.clip = obstacall;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }

    public void oncoincollect()
    {
        audioSource.clip = coin;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }

}
