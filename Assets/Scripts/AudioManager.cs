using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip start;
    public List<AudioClip> bgmClips;
    public GameManager gameManager;

    public AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioPlaying;

    private float gapTime;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void GameStartSFX()
    {
        audioSource.Stop();
        audioSource2.Stop();
        i = 0;
        audioSource2.clip = start;
        audioSource2.GetComponent<AudioSource>().Play();
    }

    public void StartBGM()
    {
        i = 0;
        StopAllCoroutines();
        StartCoroutine(PlaySequence());
    }

    private int i = 0;
    IEnumerator PlaySequence()
    {
        audioSource.loop = false;
        audioSource.playOnAwake = false;

        while (true)
        {
            if (i < bgmClips.Count)
            {
                switch (i)
                {
                    case 0:
                        audioPlaying = audioSource;
                        gapTime = 8.5f;
                        break;
                    case 1:
                        audioPlaying = audioSource2;
                        gapTime = 6.65f;
                        break;
                    case 2:
                        audioPlaying = audioSource;
                        gapTime = 4.65f;
                        break;
                    case 3:
                        audioPlaying = audioSource2;
                        gapTime = 5f;
                        break;
                    default:
                        audioPlaying = audioSource;
                        break;
                }
                audioPlaying.clip = bgmClips[i];
                audioPlaying.Play();
                //yield return new WaitForSeconds(audioPlaying.clip.length - gapTime);
                yield return new WaitForSecondsRealtime(audioPlaying.clip.length - gapTime);
                i++;
            }
            else
            {
                //i = 0;
                if(gameManager.health<=0)
                {
                    StartBGM();
                }
                yield break;
            }
        }
    }
}
