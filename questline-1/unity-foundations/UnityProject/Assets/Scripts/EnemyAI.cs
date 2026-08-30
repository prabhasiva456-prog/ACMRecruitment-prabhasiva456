using UnityEngine;
public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed=2f, chaseSpeed=3.8f, detectionRange=7f; public int health=2;
    Rigidbody2D body; Transform player; float direction=1f, jumpTimer;
    void Start(){ body=GetComponent<Rigidbody2D>(); player=GameObject.FindGameObjectWithTag("Player")?.transform; }
    void FixedUpdate(){
        if(!player||GameManager.Instance.IsGameOver){body.linearVelocity=new Vector2(0,body.linearVelocity.y);return;}
        float distance=Vector2.Distance(transform.position,player.position);
        float dir=distance<detectionRange?Mathf.Sign(player.position.x-transform.position.x):direction;
        float speed=(distance<detectionRange?chaseSpeed:patrolSpeed)*GameManager.Instance.DifficultyMultiplier;
        body.linearVelocity=new Vector2(dir*speed,body.linearVelocity.y); transform.localScale=new Vector3(-dir*Mathf.Abs(transform.localScale.x),transform.localScale.y,1);
        bool floor=Physics2D.Raycast((Vector2)transform.position+Vector2.right*dir*.55f,Vector2.down,1f,GameBootstrap.GroundMask);
        bool wall=Physics2D.Raycast(transform.position,Vector2.right*dir,.75f,GameBootstrap.GroundMask);
        if(!floor&&distance>=detectionRange)direction*=-1;
        jumpTimer-=Time.fixedDeltaTime; if(wall&&jumpTimer<=0){body.linearVelocity=new Vector2(body.linearVelocity.x,8f);jumpTimer=.8f;}
    }
    void OnCollisionStay2D(Collision2D c){PlayerHealth h=c.gameObject.GetComponent<PlayerHealth>();if(h)h.Damage(1,transform.position);}
    public void TakeDamage(int amount,Vector2 source){health-=amount;body.linearVelocity=new Vector2(Mathf.Sign(transform.position.x-source.x)*7f,5f);if(health<=0){GameManager.Instance.AddScore(25);Destroy(gameObject);}}
}
