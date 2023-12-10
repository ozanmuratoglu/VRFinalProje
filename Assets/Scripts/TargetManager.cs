using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TargetManager : MonoBehaviour
{
    public GameObject[] TargetsPrefabs;
    public Transform[] SpawnPoints;
    public float WaveTime;
    public TextMeshProUGUI timeText;
    public AudioSource boop;
    public AudioSource startMusic;
    public AudioSource gameMusic;
    
    //time shit
    private float timeSinceLastSpawn;
    private float spawnTime;
    private bool waveActive;
    private float waveStartTime;
    private void Awake()
    {
        timeSinceLastSpawn = 0;
        spawnTime = 0;
        waveStartTime = 0;
        waveActive = false;
    }

    public bool isWave()
    {
        return waveActive;
    }

    private void Update()
    {
        if (waveActive)
        {
            timeSinceLastSpawn += Time.deltaTime;
            
            if(timeSinceLastSpawn > spawnTime)
            {
                SpawnWave();
                timeSinceLastSpawn = 0;
                spawnTime = 5 - Random.Range(1,3) * ((Time.time - waveStartTime) / WaveTime);
            }

            timeText.fontSize += 2f * Time.deltaTime;
            int time = (int) Mathf.Floor(WaveTime - (Time.time - waveStartTime));
            timeText.text = time.ToString();
            if(time <= 0)
            {
                StartCoroutine(EndWave());
            }
        }
    }
    private void SpawnWave()
    {
        //aras�ndan se�ilebilecek hedef listelerini beyan eder
        int[] l1 = { 0, 0, 0, 0, 0 };
        int[] l2 = { 0, 0, 0,};
        int[] l3 = { 0, 1, 0, 0, 0, 1 };
        int[] l4 = { 0, 1, 0, 1, 0 };
        int[] l5 = { 0, 0, 0, 0, 1 };
        int[] l6 = { 0, 1, 0, 1, 0 };
        int[] l7 = { 1, 1, 0, 0 };
        int[] l8 = { 1, 0, 1, 0, 1, 0, 1 };
        int[] l9 = { 1,1,1 };
        int[] l10 = { 0, 0, 0, 0, 0 };
        int[] l11 = { 2 };
        int[] l12 = { 0,0,2,0,1 };
        List<int[]> targetSets = new List<int[]>();//bu listelerin dizisini bildirir
        targetSets.Add(l1); targetSets.Add(l2); targetSets.Add(l3); targetSets.Add(l4); targetSets.Add(l5); targetSets.Add(l6); targetSets.Add(l7);
        targetSets.Add(l8); targetSets.Add(l9); targetSets.Add(l10); targetSets.Add(l1); targetSets.Add(l1); targetSets.Add(l1); targetSets.Add(l11);
        targetSets.Add(l12);

        int spawnIndex = Random.Range(0, (int)SpawnPoints.Length / 2) * 2; //sadece �ift g�stergeleri kullan�r
        float spawnTime = Random.Range(0.5f, 1.5f);//hedeflerin birbirlerinden ayr� olarak ortaya ��kt�klar� rastgele aral��� bildirir
        float pathTime = Random.Range(2f, 4f);
        int[] tarList = targetSets[Random.Range(0, targetSets.Count)];
        for (int i = 0; i < tarList.Length; i++)
        {
            IEnumerator spawn = SpawnTarget(tarList[i],SpawnPoints[spawnIndex], SpawnPoints[spawnIndex+1],pathTime, i);//�u andan itibaren sabit bir s�re i�inde ortaya ��kma hedefi
            StartCoroutine(spawn);
        }
        
    }

    private IEnumerator SpawnTarget(int targetIndex, Transform startPos, Transform endpos, float pathTime, int i)
    {
        yield return new WaitForSeconds(2 * i);
        Target tar = Instantiate(TargetsPrefabs[targetIndex], startPos.position, startPos.rotation).GetComponent<Target>();
        tar.SetPath(startPos, endpos);
        tar.SetPathTime(pathTime);
    }


    public void StartWave() {//d��meye bas�larak �a�r�labilir
        StartCoroutine("ActualStartWave");
    }

    private IEnumerator ActualStartWave()
    {
        timeText.text = "3";
        boop.Play();
        yield return new WaitForSeconds(1);

        timeText.text = "2";
        boop.Play();
        yield return new WaitForSeconds(1);

        timeText.text = "1";
        boop.Play();
        yield return new WaitForSeconds(1);

        waveStartTime = Time.time;
        waveActive = true;
        boop.pitch += 1;
        boop.Play();
        boop.pitch -= 1;
        startMusic.Stop();
        gameMusic.Play();
    }

    private IEnumerator EndWave()
    {
        boop.Play();
        waveActive = false;
        timeText.text = "STOP";
        timeText.color = new Color(255, 0, 0);
        timeText.fontSize = 90;
        yield return new WaitForSeconds(3f);
        boop.Play();
        if (ScoreKeeper.current.CheckHighScore())
        {
            timeText.text = "NEW HIGH SCORE";
            timeText.fontSize = 70;
            timeText.color = new Color(80, 255, 0);
            SceneManager.LoadScene(2);
            yield return new WaitForSeconds(3f);
        }
        else
        {
            timeText.text = "GOOD WORK";
            timeText.fontSize = 90;
            yield return new WaitForSeconds(3f);
            boop.Play();
            timeText.text = "GOODBYE";
            timeText.fontSize = 90;
            timeText.color = new Color(80, 255, 0);
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(0);
        }
    }
}
