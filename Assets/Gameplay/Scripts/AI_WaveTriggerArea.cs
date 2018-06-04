using UnityEngine;

public class AI_WaveTriggerArea : MonoBehaviour
{
	public int waveIndex;

	private void OnTriggerEnter(Collider col)
	{
		//Check if the entered object is the player.
		if(col.CompareTag("Player"))
		{
			//Start a wave based on the wave index.
			AI_WaveManager.Instance.BeginWave(waveIndex);
			Destroy(gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		//Draw the outline of the trigger area.
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireCube(transform.position, transform.localScale);
	}
}
