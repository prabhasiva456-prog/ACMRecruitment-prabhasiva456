using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static readonly Color Teal = new Color(.22f,.94f,.80f);
    public static readonly Color Gold = new Color(1f,.76f,.29f);
    static Sprite square;
    Texture2D texture;

    public Runner Build()
    {
        texture = new Texture2D(1,1);
        texture.SetPixel(0,0,Color.white); texture.Apply();
        square = Sprite.Create(texture,new Rect(0,0,1,1),Vector2.one*.5f,1);
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = Mathf.Max(9.2f,17f / camera.aspect);
        camera.backgroundColor = new Color(.035f,.055f,.10f);
        cameraObject.transform.position = new Vector3(0,1.2f,-10);
        cameraObject.AddComponent<AudioListener>();

        for(int x=-17;x<=17;x+=2) Shape("Backdrop grid",new Vector2(x,1),new Vector2(.018f,20),new Color(.06f,.10f,.16f),-10);
        for(int y=-7;y<=11;y+=2) Shape("Backdrop grid",new Vector2(0,y),new Vector2(36,.018f),new Color(.06f,.10f,.16f),-10);
        for(int i=0;i<24;i++) Shape("Station light",new Vector2(-16+(i*7%33),-5+(i*3%15)),Vector2.one*.05f,new Color(.17f,.3f,.39f),-9);

        Platform("Station floor",new Vector2(0,-4),new Vector2(32,1));
        Platform("Left boundary",new Vector2(-16,0),new Vector2(.5f,8));
        Platform("Right boundary",new Vector2(16,0),new Vector2(.5f,8));
        Platform("01 / Arrival",new Vector2(-9,-1.5f),new Vector2(5,.45f));
        Platform("02 / Relay",new Vector2(-2,0),new Vector2(4,.45f));
        Platform("03 / Crossing",new Vector2(5,1.5f),new Vector2(4,.45f));
        Platform("04 / Lookout",new Vector2(11,2.7f),new Vector2(3.6f,.45f));
        MakeTrigger("Laser floor A",new Vector2(0,-3.35f),new Vector2(3,.25f),new Color(1f,.30f,.38f),TriggerKind.Hazard);
        MakeTrigger("Laser floor B",new Vector2(8.8f,-3.35f),new Vector2(2,.25f),new Color(1f,.30f,.38f),TriggerKind.Hazard);

        Vector2[] cores = {new Vector2(-12,-2.4f),new Vector2(-9,-.45f),new Vector2(-2,1.05f),new Vector2(5,2.55f),new Vector2(11,3.8f),new Vector2(13.8f,-2.3f)};
        foreach(Vector2 position in cores)
        {
            GameObject core=MakeTrigger("Light Core",position,new Vector2(.48f,.48f),Gold,TriggerKind.Core);
            core.transform.rotation=Quaternion.Euler(0,0,45);
        }
        MakeTrigger("Exit Portal",new Vector2(14.6f,-2.1f),new Vector2(.7f,2.7f),Teal,TriggerKind.Exit);
        Shape("Exit plinth",new Vector2(14.6f,-3.4f),new Vector2(1.8f,.15f),Teal,2);
        Enemy(new Vector2(-5,-2.9f),-7,-3);
        Enemy(new Vector2(5,2.3f),3.5f,6.5f);
        Enemy(new Vector2(6,-2.9f),4,8);

        GameObject player=Shape("Player",new Vector2(-14,-2.7f),new Vector2(.75f,1.2f),Teal,5);
        player.layer=6;
        Rigidbody2D body=player.AddComponent<Rigidbody2D>();
        body.freezeRotation=true; body.gravityScale=2.6f;
        body.interpolation=RigidbodyInterpolation2D.Interpolate;
        body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        player.AddComponent<CapsuleCollider2D>().size=Vector2.one;
        GameObject visor=Shape("Visor",Vector2.zero,new Vector2(.55f,.2f),new Color(.035f,.09f,.12f),6);
        visor.transform.SetParent(player.transform,false); visor.transform.localPosition=new Vector3(.14f,.16f,0);
        return player.AddComponent<Runner>();
    }

    static GameObject Shape(string name,Vector2 position,Vector2 size,Color color,int order=0)
    {
        GameObject obj=new GameObject(name);
        obj.transform.position=position; obj.transform.localScale=new Vector3(size.x,size.y,1);
        SpriteRenderer renderer=obj.AddComponent<SpriteRenderer>();
        renderer.sprite=square; renderer.color=color; renderer.sortingOrder=order;
        return obj;
    }

    static void Platform(string name,Vector2 position,Vector2 size)
    {
        GameObject obj=Shape(name,position,size,new Color(.10f,.19f,.25f));
        obj.layer=8; obj.AddComponent<BoxCollider2D>();
        Shape("Platform rim",position+Vector2.up*(size.y*.5f-.035f),new Vector2(size.x,.07f),new Color(.23f,.47f,.49f),1);
    }

    static GameObject MakeTrigger(string name,Vector2 position,Vector2 size,Color color,TriggerKind kind)
    {
        GameObject obj=Shape(name,position,size,color,3); obj.layer=2;
        obj.AddComponent<BoxCollider2D>().isTrigger=true;
        obj.AddComponent<LevelTrigger>().kind=kind;
        return obj;
    }

    static void Enemy(Vector2 position,float left,float right)
    {
        GameObject obj=Shape("Patrol Sentinel",position,new Vector2(.85f,.85f),new Color(1,.38f,.43f),4); obj.layer=7;
        Rigidbody2D body=obj.AddComponent<Rigidbody2D>(); body.freezeRotation=true; body.gravityScale=2.6f;
        obj.AddComponent<BoxCollider2D>();
        Sentinel enemy=obj.AddComponent<Sentinel>(); enemy.left=left; enemy.right=right;
        GameObject eye=Shape("Sentinel eye",Vector2.zero,new Vector2(.6f,.15f),Color.white,5);
        eye.transform.SetParent(obj.transform,false); eye.transform.localPosition=new Vector3(0,.12f,0);
    }

    public static void Pulse(Vector2 position)
    {
        Shape("Light pulse",position,Vector2.one*.25f,Teal,8).AddComponent<LightPulse>();
    }
    void OnDestroy() { if(square) Destroy(square); if(texture) Destroy(texture); }
}

public class LightPulse : MonoBehaviour
{
    float remaining=.25f;
    void Update()
    {
        remaining-=Time.deltaTime;
        transform.localScale+=Vector3.one*5f*Time.deltaTime;
        GetComponent<SpriteRenderer>().color=new Color(.22f,.94f,.80f,Mathf.Clamp01(remaining*4));
        if(remaining<=0) Destroy(gameObject);
    }
}
