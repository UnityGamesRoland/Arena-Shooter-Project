using UnityEngine;

public class ArenaPortal : MonoBehaviour
{
    public Vector3 portPoint;

    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if(TP_Motor.Instance.dashParticle.isPlaying) TP_Motor.Instance.dashParticle.Stop();

            col.transform.position = new Vector3(portPoint.x, col.transform.position.y, portPoint.z);

            if(TP_Motor.Instance.states.enumeratingDash) TP_Motor.Instance.dashParticle.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(portPoint, 0.3f);
    }
}
