using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public enum EnemySpawnMode {afterTime, afterKills, overrideSpawnTime};

[System.Serializable]
public class Wave
{
	public string waveName;
	[FoldoutGroup("Details", false)] public int preWaveEnemyCount;
    [FoldoutGroup("Details", false)] public bool isPreWave;
    [FoldoutGroup("Details", false)] public bool autoStart;
    [FoldoutGroup("Details", false)] public WaveEnemy[] enemies;
}

[System.Serializable]
public class WaveEnemy
{
	public string waveEnemyName;
    [FoldoutGroup("Details", false)] public EnemySpawnMode spawnMode;
    [FoldoutGroup("Details", false)] public Transform enemy;
    [FoldoutGroup("Details", false)] public Transform spawnPoint;
    [FoldoutGroup("Details", false)] public float enemySpawnDelay = 0.3f;
    [FoldoutGroup("Details", false)] public float overridedSpawnTime = 0f;
    [FoldoutGroup("Details", false)] public int spawnAfterKills;

	[HideInInspector] public float spawnAfterTime;
}

public class AI_WaveManager : MonoBehaviour
{
    
	public Wave[] waves;

	[HideInInspector] public int killsInCurrentWave;
	[HideInInspector] public float timeInCurrentWave;

	private Wave currentWave;
	private int currentWaveIndex;
	private float timeAtWaveStart;

	#region Singleton
	public static AI_WaveManager Instance {get; private set;}
	private void Awake()
	{
		if(Instance == null) Instance = this;
    }
	#endregion

	private void Start()
	{
		//Start the first wave at game start.
		if(waves.Length > 0 && waves[0].autoStart) BeginWave(0);
	}

	public void BeginWave(int waveIndex)
	{
		//Update the current wave.
		currentWave = waves[waveIndex];
		currentWaveIndex = waveIndex;

		//Start updating the current wave.
		StopCoroutine(RunCurrentWave());
		StartCoroutine(RunCurrentWave());
	}

	private IEnumerator RunCurrentWave()
	{
		//Set the new wave.
		Wave wave = currentWave;

		//Reset the wave progress variables.
		timeAtWaveStart = Time.time;
		killsInCurrentWave = 0;
		timeInCurrentWave = 0;

		//Create sorting lists for different types of enemies.
		List<WaveEnemy> enemiesWithTimeSpawn = new List<WaveEnemy>();
		List<WaveEnemy> enemiesWithKillSpawn = new List<WaveEnemy>();

		float timeToSpawnNextEnemyInList = 1f;

		//Loop through the enemy list.
		foreach(WaveEnemy waveEnemy in wave.enemies)
		{
			//Sort enemy: Spawn After Time
			if(waveEnemy.spawnMode == EnemySpawnMode.afterTime)
			{
				timeToSpawnNextEnemyInList += waveEnemy.enemySpawnDelay;
				waveEnemy.spawnAfterTime = timeToSpawnNextEnemyInList;
				enemiesWithTimeSpawn.Add(waveEnemy);
			}

			//Sort enemy: Spawn After Kills
			else if(waveEnemy.spawnMode == EnemySpawnMode.afterKills) enemiesWithKillSpawn.Add(waveEnemy);

			//Override the spawn time of the remaining enemies.
			else if(waveEnemy.spawnMode == EnemySpawnMode.overrideSpawnTime) timeToSpawnNextEnemyInList = waveEnemy.overridedSpawnTime;
		}

		//Get the required kill amount before the next wave could start.
		int requiredKills = wave.isPreWave ? wave.preWaveEnemyCount : wave.enemies.Length;

		//Wave Tick: Every 0.1 sec.
		while(killsInCurrentWave < requiredKills)
		{
			//Update the elapsed time.
			timeInCurrentWave = Time.time - timeAtWaveStart;

			//Handler list for spawned enemies.
			List<WaveEnemy> initiatedEnemiesWithTimeSpawn = new List<WaveEnemy>();
			List<WaveEnemy> initiatedEnemiesWithKillSpawn = new List<WaveEnemy>();

			//Loop through sorted list: Time Spawn Enemies
			foreach(WaveEnemy waveEnemy in enemiesWithTimeSpawn)
			{
				//Check if the enemy should be spawned.
				if(timeInCurrentWave >= waveEnemy.spawnAfterTime)
				{
					//Spawn the enemy and add it to the corresponding handler list.
					Instantiate(waveEnemy.enemy, waveEnemy.spawnPoint.position, waveEnemy.spawnPoint.rotation);
					initiatedEnemiesWithTimeSpawn.Add(waveEnemy);
					break;
				}
			}

			//Loop through sorted list: Kill Spawn Enemies
			foreach(WaveEnemy waveEnemy in enemiesWithKillSpawn)
			{
				//Check if the enemy should be spawned.
				if(killsInCurrentWave >= waveEnemy.spawnAfterKills)
				{
					//Spawn the enemy and add it to the corresponding handler list.
					Instantiate(waveEnemy.enemy, waveEnemy.spawnPoint.position, waveEnemy.spawnPoint.rotation);
					initiatedEnemiesWithKillSpawn.Add(waveEnemy);
					break;
				}
			}

			//Update the remaining enemies.
			foreach(WaveEnemy waveEnemy in initiatedEnemiesWithTimeSpawn) enemiesWithTimeSpawn.Remove(waveEnemy);
			foreach(WaveEnemy waveEnemy in initiatedEnemiesWithKillSpawn) enemiesWithKillSpawn.Remove(waveEnemy);

			//Delay the tick.
			yield return new WaitForSeconds(0.1f);
		}

		//Get the next wave index and check if there is a wave left.
		int nextWaveIndex = currentWaveIndex + 1;
		if(nextWaveIndex < waves.Length)
		{
			//Start the next wave.
			Wave nextWave = waves[currentWaveIndex + 1];
			if(nextWave.autoStart) BeginWave(currentWaveIndex + 1);
		}

		//No waves left. End level.
		else ArenaManager.Instance.CompleteLevel();
	}
}
