using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBootstrap : MonoBehaviour
{
    public static int GroundMask => 1 << 0;
    static Sprite square;
    static readonly Color Navy=new Color(.025f,.035f,.09f), Cyan=new Color(.1f,.9f,1f), Pink=new Color(1f,.2f,.55f), Lime=new Color(.65f,1f,.25f), Purple=new Color(.35f,.18f,.65f);

    // This component is saved in Main.unity, so Awake also runs after every restart.
    void Awake()
    {
        square=MakeSprite(); Camera cam=CreateCamera();
        GameManager manager=new GameObject("Game Manager").AddComponent<GameManager>();
        CreateBackground(); CreateArena(); GameObject player=CreatePlayer();
        CreateEnemy(new Vector2(-6,-1.5f));CreateEnemy(new Vector2(5,2.1f));CreateEnemy(new Vector2(10,-1.5f));
        foreach(Vector2 p in new[]{new Vector2(-8,-.8f),new Vector2(-3,1.4f),new Vector2(1,-.7f),new Vector2(5,2.7f),new Vector2(9,-.7f),new Vector2(12,1.6f)})CreateCollectible(p);
        BindUI(manager); cam.GetComponent<CameraFollow>().target=player.transform;
    }

    static Sprite MakeSprite(){Texture2D t=new Texture2D(1,1);t.SetPixel(0,0,Color.white);t.Apply();return Sprite.Create(t,new Rect(0,0,1,1),new Vector2(.5f,.5f),1);}
    static GameObject Shape(string name,Vector2 pos,Vector2 size,Color color,int order=0)
    {GameObject g=new GameObject(name);g.transform.position=pos;g.transform.localScale=size;SpriteRenderer r=g.AddComponent<SpriteRenderer>();r.sprite=square;r.color=color;r.sortingOrder=order;return g;}
    static void Solid(string name,Vector2 pos,Vector2 size,Color color)
    {GameObject g=Shape(name,pos,size,color);g.AddComponent<BoxCollider2D>();}

    Camera CreateCamera(){GameObject g=new GameObject("Main Camera");g.tag="MainCamera";Camera c=g.AddComponent<Camera>();c.orthographic=true;c.orthographicSize=6;c.backgroundColor=Navy;g.transform.position=new Vector3(0,1,-10);g.AddComponent<AudioListener>();g.AddComponent<CameraFollow>();return c;}
    void CreateBackground(){for(int i=0;i<55;i++){float x=-18+(i*7%37);float y=-5+(i*13%19)*.6f;GameObject star=Shape("Star",new Vector2(x,y),Vector2.one*(i%3==0?.08f:.04f),i%4==0?Pink:Cyan,-10);star.transform.rotation=Quaternion.Euler(0,0,i*17);}}
    void CreateArena()
    {
        Solid("Floor",new Vector2(0,-3),new Vector2(30,1),Purple);Solid("Left Wall",new Vector2(-15,1),new Vector2(1,9),Purple);Solid("Right Wall",new Vector2(15,1),new Vector2(1,9),Purple);
        Solid("Platform A",new Vector2(-4,-.3f),new Vector2(5,.45f),new Color(.16f,.35f,.55f));Solid("Platform B",new Vector2(5,1.2f),new Vector2(4,.45f),new Color(.16f,.35f,.55f));Solid("Platform C",new Vector2(11,.1f),new Vector2(3,.45f),new Color(.16f,.35f,.55f));
        GameObject spikes=Shape("Energy Hazard",new Vector2(1,-2.35f),new Vector2(3,.3f),Pink);spikes.AddComponent<BoxCollider2D>();spikes.AddComponent<Hazard>();
    }
    GameObject CreatePlayer()
    {
        GameObject g=Shape("Player",new Vector2(-11,-1.5f),new Vector2(1,1.25f),Cyan,5);g.tag="Player";g.layer=6;
        Rigidbody2D rb=g.AddComponent<Rigidbody2D>();rb.freezeRotation=true;rb.gravityScale=2.5f;rb.interpolation=RigidbodyInterpolation2D.Interpolate;rb.collisionDetectionMode=CollisionDetectionMode2D.Continuous;g.AddComponent<BoxCollider2D>();g.AddComponent<PlayerController>();g.AddComponent<PlayerHealth>();
        GameObject eye=Shape("Visor",new Vector2(.18f,.15f),new Vector2(.48f,.18f),Navy,6);eye.transform.SetParent(g.transform,false);return g;
    }
    void CreateEnemy(Vector2 pos)
    {
        GameObject g=Shape("Hunter",pos,new Vector2(1.05f,.9f),Pink,4);g.layer=7;Rigidbody2D rb=g.AddComponent<Rigidbody2D>();rb.freezeRotation=true;rb.gravityScale=2.5f;g.AddComponent<BoxCollider2D>();g.AddComponent<EnemyAI>();
        GameObject eye=Shape("Eye",new Vector2(-.2f,.08f),new Vector2(.22f,.22f),Color.white,5);eye.transform.SetParent(g.transform,false);
    }
    void CreateCollectible(Vector2 pos){GameObject g=Shape("Energy Crystal",pos,new Vector2(.45f,.7f),Lime,3);g.layer=2;g.transform.rotation=Quaternion.Euler(0,0,45);BoxCollider2D c=g.AddComponent<BoxCollider2D>();c.isTrigger=true;g.AddComponent<Collectible>();}

    void BindUI(GameManager manager)
    {
        new GameObject("Event System", typeof(EventSystem), typeof(StandaloneInputModule));
        Canvas canvas=new GameObject("HUD").AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;canvas.gameObject.AddComponent<GraphicRaycaster>();
        Text score=Label(canvas.transform,"Score",new Vector2(30,-25),new Vector2(330,60),TextAnchor.UpperLeft,28,Cyan,"SCORE  0000");
        Text health=Label(canvas.transform,"Health",new Vector2(-30,-25),new Vector2(300,60),TextAnchor.UpperRight,24,Color.white,"HEALTH  5 / 5");health.rectTransform.anchorMin=health.rectTransform.anchorMax=new Vector2(1,1);health.rectTransform.pivot=new Vector2(1,1);
        Image bar=Panel(canvas.transform,"Health Bar",new Color(.15f,.05f,.14f,.9f));SetRect(bar.rectTransform,new Vector2(1,1),new Vector2(-30,-70),new Vector2(280,18),new Vector2(1,1));
        Image fill=Panel(bar.transform,"Fill",Pink);SetRect(fill.rectTransform,new Vector2(0,.5f),Vector2.zero,new Vector2(280,18),new Vector2(0,.5f));fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;
        Text hint=Label(canvas.transform,"Controls",new Vector2(0,18),new Vector2(800,35),TextAnchor.LowerCenter,17,new Color(.75f,.82f,1),"MOVE  A/D   •   JUMP  SPACE ×2   •   ATTACK  F");hint.rectTransform.anchorMin=hint.rectTransform.anchorMax=new Vector2(.5f,0);hint.rectTransform.pivot=new Vector2(.5f,0);
        Image over=Panel(canvas.transform,"Game Over",new Color(.015f,.02f,.06f,.94f));SetRect(over.rectTransform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(560,330),new Vector2(.5f,.5f));
        Text title=Label(over.transform,"Title",new Vector2(0,-40),new Vector2(520,75),TextAnchor.MiddleCenter,48,Pink,"SYSTEM FAILURE");title.rectTransform.anchorMin=title.rectTransform.anchorMax=new Vector2(.5f,1);title.rectTransform.pivot=new Vector2(.5f,1);
        Text final=Label(over.transform,"Final",new Vector2(0,20),new Vector2(500,50),TextAnchor.MiddleCenter,26,Color.white,"FINAL SCORE  0000");
        Button button=Panel(over.transform,"Restart Button",Cyan).gameObject.AddComponent<Button>();SetRect(button.GetComponent<RectTransform>(),new Vector2(.5f,0),new Vector2(0,38),new Vector2(250,58),new Vector2(.5f,0));button.onClick.AddListener(manager.Restart);
        Text bt=Label(button.transform,"Text",Vector2.zero,new Vector2(250,58),TextAnchor.MiddleCenter,24,Navy,"RESTART  [R]");SetRect(bt.rectTransform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(250,58),new Vector2(.5f,.5f));
        // Filled UI images need a sprite, and the final score belongs inside its panel.
        fill.sprite=square;
        SetRect(final.rectTransform,new Vector2(.5f,.5f),Vector2.zero,new Vector2(500,50),new Vector2(.5f,.5f));
        button.targetGraphic=button.GetComponent<Image>();
        manager.BindUI(score,health,fill,over.gameObject,final);
    }
    static Font Font()=>Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    static Text Label(Transform parent,string name,Vector2 pos,Vector2 size,TextAnchor align,int fontSize,Color color,string value){GameObject g=new GameObject(name);g.transform.SetParent(parent,false);Text t=g.AddComponent<Text>();t.font=Font();t.fontSize=fontSize;t.alignment=align;t.color=color;t.text=value;t.fontStyle=FontStyle.Bold;SetRect(t.rectTransform,new Vector2(0,1),pos,size,new Vector2(0,1));return t;}
    static Image Panel(Transform parent,string name,Color color){GameObject g=new GameObject(name);g.transform.SetParent(parent,false);Image i=g.AddComponent<Image>();i.color=color;return i;}
    static void SetRect(RectTransform r,Vector2 anchor,Vector2 pos,Vector2 size,Vector2 pivot){r.anchorMin=r.anchorMax=anchor;r.pivot=pivot;r.anchoredPosition=pos;r.sizeDelta=size;}
    public static void SpawnPulse(Vector2 position){GameObject g=Shape("Pulse",position,Vector2.one*.25f,new Color(.5f,1,1,.8f),8);g.AddComponent<Pulse>();}
}

public class Pulse:MonoBehaviour
{
    SpriteRenderer sprite;
    void Awake(){sprite=GetComponent<SpriteRenderer>();}
    void Update(){transform.localScale+=Vector3.one*4f*Time.deltaTime;Color c=sprite.color;c.a-=2.5f*Time.deltaTime;sprite.color=c;if(c.a<=0)Destroy(gameObject);}
}
