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
    public float[] waitTimeLvl2;
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
    public Camera mainCamera;
    private int lastWidth;
    private int lastHeight;

    private void Awake()
    {
        health = 5;
        gameOverUI.SetActive(false);
        /*double sum = 0.0;
        foreach (float value in waitTime)
        {
            sum += value;
            beatTimes.Add(sum);
        }*/
    }

    private void LevelShiftTo(float[] level)
    {
        beatTimes.Clear();
        double sum = 0.0;
        foreach (float value in level)
        {
            sum += value;
            beatTimes.Add(sum);
        }
        PosesQueue();
    }

    void Start()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        AdjustCamera();
    }

    public void GameStart()
    {
        LevelShiftTo(waitTime);
        songStarted = true;
        songStartDspTime = AudioSettings.dspTime;
        audioManager.GameStartSFX();
        audioManager.StartBGM();
        scroe = 0;
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

        if (songStarted)
        {
            DPSTimer();
        }

        comboUI.SetActive(combo > 0);

        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            AdjustCamera();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    public void CreateEchoCircle(Transform pos, float speed)
    {
        GameObject circlePrefab = pool.Get();
        EchoCircle _echoCircle = circlePrefab.GetComponentInChildren<EchoCircle>();
        circlePrefab.transform.position = pos.position;
        _echoCircle.expandSpeed = speed;
        //EchoCircle _echoCircle = Instantiate(echoCircle, pos.position, Quaternion.identity).GetComponentInChildren<EchoCircle>();
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
                if (noteIndex != 0)
                {
                    //CreateEchoCircle(poses[Random.Range(0, poses.Length)], 3f);
                    CreateEchoCircle(poses[poseQueue[noteIndex]], 3f);
                }
                noteIndex++;
            }
            restarted = false;
        }
        else
        {
            if (!restarted)
            {
                congratUI.SetActive(false);
                congratUI.SetActive(true);
                audioManager.StartBGM();
                Debug.Log("restart");
                songStartDspTime = AudioSettings.dspTime;
                noteIndex = 0;

                LevelShiftTo(waitTimeLvl2);
                beatTimes[0] = 1.25f;
                restarted = true;
            }
        }
    }
    void AdjustCamera()
    {
        if (Screen.height > Screen.width) // 竖屏
        {
            mainCamera.fieldOfView = 50f;
        }
        else
        {
            mainCamera.fieldOfView = 30f;
        }
    }

    public List<int> poseQueue; Queue<int> recentPoses = new Queue<int>();
    private void PosesQueue()
    {
        for (int i = 0; i < beatTimes.Count; i++)
        {
            // 构造一个候选池，排除最近的8个
            List<int> candidateIndices = new List<int>();
            for (int j = 0; j < poses.Length; j++)
            {
                if (!recentPoses.Contains(j))
                    candidateIndices.Add(j);
            }

            // 安全判断：如果候选项为空（poses太少），就允许所有
            if (candidateIndices.Count == 0)
            {
                for (int j = 0; j < poses.Length; j++)
                    candidateIndices.Add(j);
            }

            // 从候选中随机一个
            int selectedIndex = candidateIndices[Random.Range(0, candidateIndices.Count)];
            poseQueue.Add(selectedIndex);

            // 记录最近选过的 pose
            recentPoses.Enqueue(selectedIndex);
            if (recentPoses.Count > 8)
                recentPoses.Dequeue();
        }
    }
}
