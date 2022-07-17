using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public class GameManagerScript : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnPrefab
    {
        public GameObject enemyPrefab;
        [Range(1, 100)] public int randomSpawnWeight = 1;
    }

    [Serializable]
    public class FrustumCorners
    {
        public Transform corner00, corner01, corner11, corner10;
    }

    public EnemySpawnPrefab[] enemySpawnPrefabs;
    public float timeBetweenSpawns = 5.0f;
    public FrustumCorners frustumCorners;
    public VolumeProfile globalPostprocessing;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    public GameObject bossGameObject;
    public Transform bossGameObjectSpawnPosition;
    public float timeToWaitAfterBossSpawn = 15.0f;
    public int numOfEnemiesToSpawnBeforeBoss = 20;
    private int _numEnemiesSpawnedBeforeBoss = 0;
    
    [Header("Gamestate Stats")]
    public int xp = 0;
    public int xpForNextLevel = 150;
    public int currentLevel = 1;
    public UnityEngine.UI.Text uiXpShower;
    public GameObject[] spawnDices;
    public float chanceToSpawnDices = 0.25f;

    [Header("Player Ref")]
    public EntityHealthTracker playerStats;


    void Start()
    {
        ReportXPEarned(0, Vector3.zero, false);
        StartCoroutine(FadeInScreen());
        StartCoroutine(SpawnEnemies());
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Application.Quit();
        }
    }

    #if UNITY_EDITOR
    void OnApplicationQuit()
    {
        // Reset for the editor!
        ColorAdjustments ca;
        globalPostprocessing.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.value = 0.0f;
    }
    #endif

    IEnumerator FadeInScreen()
    {
        ColorAdjustments ca;
        globalPostprocessing.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.value = -5.0f;

        while (ca.postExposure.GetValue<float>() < 0.0f)
        {
            ca.postExposure.value += 1.0f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        ca.postExposure.value = 0.0f;
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        while (true)
        {
            if (_numEnemiesSpawnedBeforeBoss < numOfEnemiesToSpawnBeforeBoss)
            {
                // Pick spawn position
                float lerpValue = UnityEngine.Random.Range(0.0f, 1.0f);
                Vector3 spawnLocation = new Vector3();

                int whichCombo = (int)UnityEngine.Random.Range(0.0f, 3.999f);
                switch (whichCombo)
                {
                case 0:
                    spawnLocation = Vector3.Lerp(frustumCorners.corner00.position, frustumCorners.corner01.position, lerpValue);
                    break;

                case 1:
                    spawnLocation = Vector3.Lerp(frustumCorners.corner01.position, frustumCorners.corner11.position, lerpValue);
                    break;

                case 2:
                    spawnLocation = Vector3.Lerp(frustumCorners.corner11.position, frustumCorners.corner10.position, lerpValue);
                    break;

                case 3:
                    spawnLocation = Vector3.Lerp(frustumCorners.corner10.position, frustumCorners.corner00.position, lerpValue);
                    break;
                }
                spawnLocation.y = 1.0f;

                // Random spawn
                float totalRandomWeight = 0.0f;
                foreach (var spawnPrefab in enemySpawnPrefabs)
                {
                    totalRandomWeight += spawnPrefab.randomSpawnWeight;
                }
                float randomValue = UnityEngine.Random.Range(0.0f, totalRandomWeight);

                foreach (var spawnPrefab in enemySpawnPrefabs)
                {
                    randomValue -= (float)spawnPrefab.randomSpawnWeight;
                    if (randomValue <= 0.0f)
                    {
                        // Spawn this one
                        _spawnedEnemies.Add(Instantiate(spawnPrefab.enemyPrefab, spawnLocation, spawnPrefab.enemyPrefab.transform.rotation));
                        break;
                    }
                }

                _numEnemiesSpawnedBeforeBoss++;
                Debug.Log("Num Enemies spawned: " + _numEnemiesSpawnedBeforeBoss);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            else
            {
                _numEnemiesSpawnedBeforeBoss = 0;
                // Spawn boss at special position
                //EditorApplication.isPaused = true;
                var jojo = Instantiate(bossGameObject, bossGameObjectSpawnPosition.position, bossGameObject.transform.rotation);
                Debug.Log("HOPED POS: " + bossGameObjectSpawnPosition.position);
                Debug.Log("HOPED POS: " + jojo.transform.position);
                _spawnedEnemies.Add(jojo);
                yield return new WaitForSeconds(timeToWaitAfterBossSpawn);
            }
        }
    }

    public void ReportPlayerDied()
    {
        StartCoroutine(FadeOutScreenAndRestart());
    }

    IEnumerator FadeOutScreenAndRestart()
    {
        ColorAdjustments ca;
        globalPostprocessing.TryGet<ColorAdjustments>(out ca);
        ca.postExposure.value = 0.0f;

        while (ca.postExposure.GetValue<float>() > -5.0f)
        {
            ca.postExposure.value -= 1.0f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Restart this scene!
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReportXPEarned(int amount, Vector3 dyingPosition, bool maybeSpawnDice = true)
    {
        xp += amount;

        if (xp >= xpForNextLevel)
        {
            xpForNextLevel = 0;
            currentLevel = 0;
            while (xp >= xpForNextLevel)
            {
                currentLevel++;
                xpForNextLevel = (int)(Mathf.Pow(currentLevel, 1.25f) * 150.0f);
            }

            //
            // Level up those stats!
            //
            playerStats.maxHealth = 100 + 10 * (currentLevel - 1);
            playerStats.ReplenishHealthRelative(playerStats.maxHealth);
        }

        uiXpShower.text = $"XP: {xp} / {xpForNextLevel}\nLevel {currentLevel}";

        if (!maybeSpawnDice || UnityEngine.Random.value > chanceToSpawnDices)
            return;

        // Spawn dice
        float whichDiceToSpawn = UnityEngine.Random.value * 0.999f * (float)spawnDices.Length;
        var go = spawnDices[(int)whichDiceToSpawn];
        Instantiate(go, new Vector3(dyingPosition.x, go.transform.position.y, dyingPosition.z), go.transform.rotation);
    }

    public List<GameObject> GetListOfSpawnedEnemies()
    {
        for (int i = _spawnedEnemies.Count - 1; i >= 0; i--)
        {
            var spawnedEnemy = _spawnedEnemies[i];
            if (spawnedEnemy == null)
            {
                _spawnedEnemies.RemoveAt(i);
            }
        }

        return _spawnedEnemies;
    }
}
