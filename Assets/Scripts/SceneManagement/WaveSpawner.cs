using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveSpawner : MonoBehaviour
{
   public enum SpawnState
   {
      spawning,
      waiting,
      counting
   }

   [System.Serializable]
   public class Wave
   {
      public string waveName;
      public Transform enemy;
      public int count;
      public float spawnRate;
   }

   public Wave[] waves;
   private int nextWave = 0;

   public float timeBetweenWaves = 5f;
   public float waveCountdown;

   private float searchCountdown = 1f;

   public SpawnState state = SpawnState.counting;
   
   void Awake()
   {
      this.GameObject().SetActive(false);
   }

   public void TriggerSpawner()
   {
      this.GameObject().SetActive(true);
      waveCountdown = timeBetweenWaves;
   }

   void Update()
   {
      if (state == SpawnState.waiting)
      {
         if (!EnemyIsAlive())
         {
            WaveCompleted();
         }
         else
         {
            return;
         }
      }
      
      if (waveCountdown <= 0)
      {
         if (state != SpawnState.spawning)
         {
            StartCoroutine(SpawnWave(waves[nextWave]));
         }
      }
      else
      {
         waveCountdown -= Time.deltaTime;
      }
   }

   void WaveCompleted()
   {
      Debug.Log("Wave Completed!");

      state = SpawnState.counting;
      waveCountdown = timeBetweenWaves;

      if (nextWave + 1 > waves.Length - 1)
      {
         Debug.Log("ALL WAVES COMPLETE!");
      }
      else
      {
         nextWave++;  
      }
   }

   bool EnemyIsAlive()
   {
      searchCountdown -= Time.deltaTime;
      if (searchCountdown <= 0f)
      {
         searchCountdown = 1f;
         if (GameObject.FindGameObjectWithTag("Enemy") == null)
         {
            return false;
         }  
      }

      return true;
   }

   IEnumerator SpawnWave(Wave wave)
   {
      // Spawn enemy
      Debug.Log("Spawning wave: " + wave.waveName);
      state = SpawnState.spawning;

      for (int i = 0; i < wave.count; i++)
      {
         SpawnEnemy(wave.enemy);
         yield return new WaitForSeconds( 1f / wave.spawnRate );
      }

      state = SpawnState.waiting;
      
      yield break;
   }

   void SpawnEnemy(Transform enemy)
   {
      Debug.Log("Spawning Enemy: " + enemy.name);

      if (nextWave == 0)
      {
         Vector3 spawnPosition = new(transform.position.x, -19, transform.position.z);
         Instantiate(enemy, spawnPosition, transform.rotation);
      }
      else if (nextWave == 1)
      {
         Vector3 spawnPosition = new(transform.position.x, 30, transform.position.z);
         Instantiate(enemy, spawnPosition, transform.rotation);
      }
      else if (nextWave == 2)
      {
         Vector3 spawnPosition = new(transform.position.x, -19, transform.position.z);
         Instantiate(enemy, transform.position, transform.rotation);  
      }
   }
}
