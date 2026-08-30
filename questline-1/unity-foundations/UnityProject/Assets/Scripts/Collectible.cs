using UnityEngine;
public class Collectible : MonoBehaviour
{
    Vector3 origin;
    void Start(){origin=transform.position;}
    void Update(){transform.position=origin+Vector3.up*(Mathf.Sin(Time.time*3f+origin.x)*.18f);transform.Rotate(0,0,100f*Time.deltaTime);}
    void OnTriggerEnter2D(Collider2D other){if(other.CompareTag("Player")){GameManager.Instance.AddScore(10);GameBootstrap.SpawnPulse(transform.position);Destroy(gameObject);}}
}
