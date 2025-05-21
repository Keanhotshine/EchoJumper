using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject echoCircle;
    public GameObject startUI;
    public Transform[] poses;
    public float[] waitTime;
    public int health;
    public List<GameObject> healthUIs;
    public int scroe;

    public int combo;
    public static bool scoreJump;
    public GameObject comboUI;
    public Text comboText;

    public Text scoreText;
    public GameObject gameOverUI;
    public AudioManager audioManager;
    public GameObject congratUI;
    public double songStartDspTime;

    public List<double> beatTimes;
    public int noteIndex = 0;

    private bool songStarted = false;

    public ObjectPool pool;

    private void Awake()
    {
        health = 5;
        gameOverUI.SetActive(false);
        double sum = 0.0;
        foreach (float value in waitTime)
        {
            sum += value;
            beatTimes.Add(sum);
        }
    }

    public void GameStart()
    {
        songStarted = true;
        songStartDspTime = AudioSettings.dspTime;
        audioManager.GameStartSFX();
        audioManager.StartBGM();
        scroe = 0;
        //StartCoroutine(Spawn());
        startUI.SetActive(false);
        gameOverUI.SetActive(false);
        CreateEchoCircle(this.transform, 15f);
        foreach (GameObject heart in healthUIs)
        { heart.SetActive(true); }
        health = 5;


        noteIndex = 0;
    }
    void Update()
    {
        scoreText.text = "SCORE: " + scroe;
        comboText.text = combo.ToString();
        if (health == 0)
        {
            StopAllCoroutines();
            songStarted = false;
            gameOverUI.SetActive(true);
        }

        if(songStarted)
        {
            DPSTimer();
        }

        comboUI.SetActive(combo > 0);
    }

    public void CreateEchoCircle(Transform pos, float speed)
    {
        GameObject circlePrefab = pool.Get();
        EchoCircle _echoCircle = circlePrefab.GetComponentInChildren<EchoCircle>();
        circlePrefab.transform.position = pos.position;
        _echoCircle.expandSpeed = speed;
        //EchoCircle _echoCircle = Instantiate(echoCircle, pos.position, Quaternion.identity).GetComponentInChildren<EchoCircle>();
    }

    IEnumerator Spawn()
    {
        int num = 0;
        while (true)
        {
            if (num < waitTime.Length - 1)
            {
                num++;
                //Debug.Log(num);
                yield return new WaitForSeconds(waitTime[num]);
            }
            else
            {
                //Debug.Log("Cleared, Start random wave!");
                congratUI.SetActive(false);
                congratUI.SetActive(true);
                yield return new WaitForSeconds(1.25f);
                audioManager.StartBGM();
                StartCoroutine(Spawn());
                yield break;
                //yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.5f));
            }
            CreateEchoCircle(poses[Random.Range(0, poses.Length)], 3f);
        }
    }

    public void Hited()
    {
        if (health > 0)
        {
            healthUIs[health - 1].SetActive(false);
            health -= 1;
        }
    }

    private bool restarted;
    private void DPSTimer()
    {
        double currentDspTime = AudioSettings.dspTime;
        double timeSinceStart = currentDspTime - songStartDspTime;

        if (noteIndex < beatTimes.Count)
        {
            if (timeSinceStart >= beatTimes[noteIndex])
            {
                if(noteIndex != 0)
                {
                    CreateEchoCircle(poses[Random.Range(0, poses.Length)], 3f);
                }
                noteIndex++;
            }
            restarted = false;
        }
        else
        {
            if(!restarted)
            {
                congratUI.SetActive(false);
                congratUI.SetActive(true);
                audioManager.StartBGM();
                Debug.Log("restart");
                songStartDspTime = AudioSettings.dspTime;
                noteIndex = 0;
                beatTimes[0] = 1.25f;
                restarted = true;
            }
        }
    }
}
