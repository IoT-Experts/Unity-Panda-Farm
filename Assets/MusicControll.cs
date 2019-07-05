using UnityEngine;
using System.Collections;

public class MusicControll : MonoBehaviour
{

    public static MusicControll musicControll;
    public AudioClip horizontal;
    public AudioClip convertingSpecialFx;
    public AudioClip gameover;   
    public AudioClip matchSoundFx;
    public AudioClip dropSoundFx;
    public AudioClip switchSoundFx;
    public AudioClip badMoveSoundFx;  
    public AudioClip bombSoundFx;
    public AudioClip matchFiveSoundFx;
    public AudioClip specialMatchSoundFx;
    public AudioClip treasureCollectedFx;
    public AudioClip musicBackground;
    public AudioClip ButtonClick;
    public AudioClip AddBasket;
    public AudioClip winner;
    // Use this for initialization
    public bool isSoundOn = true;
    public bool isMusicOn = true;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        musicControll = this;
      
    }
   
    public void MakeSound(AudioClip originalClip)
    {
        if (originalClip != null)
        {
            if (isSoundOn)
            {
                AudioSource.PlayClipAtPoint(originalClip, transform.position);
            }
        }
    }
}
