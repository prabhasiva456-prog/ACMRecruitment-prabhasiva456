using UnityEngine;
public class Hazard:MonoBehaviour{void OnCollisionStay2D(Collision2D c){PlayerHealth h=c.gameObject.GetComponent<PlayerHealth>();if(h)h.Damage(1,transform.position);}}
