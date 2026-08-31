using UnityEngine;

public class GameHud : MonoBehaviour
{
    GUIStyle label, button;
    readonly Color navy=new Color(.035f,.055f,.10f);
    readonly Color panel=new Color(.06f,.10f,.16f,.98f);
    readonly Color muted=new Color(.57f,.68f,.76f);

    void OnGUI()
    {
        if(label==null)
        {
            label=new GUIStyle(GUI.skin.label) {fontStyle=FontStyle.Bold};
            button=new GUIStyle(GUI.skin.button) {fontSize=19,fontStyle=FontStyle.Bold};
            button.normal.background=Texture2D.whiteTexture;
            button.hover.background=Texture2D.whiteTexture;
            button.active.background=Texture2D.whiteTexture;
            button.normal.textColor=navy; button.hover.textColor=navy; button.active.textColor=navy;
        }
        Matrix4x4 previous=GUI.matrix;
        float scale=Mathf.Min(Screen.width/1280f,Screen.height/720f);
        GUI.matrix=Matrix4x4.TRS(new Vector3((Screen.width-1280*scale)*.5f,(Screen.height-720*scale)*.5f,0),Quaternion.identity,Vector3.one*scale);
        GameSession s=GameSession.Instance;
        Box(new Rect(24,22,1232,86),panel);
        Box(new Rect(24,22,5,86),LevelBuilder.Teal);
        Text(new Rect(46,33,280,35),"LUMEN RUN",28,Color.white);
        Text(new Rect(47,73,300,22),"01  /  THE RELAY STATION",12,muted);
        Stat(380,"LIGHT CORES",s.Collected+" / 6",LevelBuilder.Gold);
        Stat(590,"SCORE",s.Score.ToString("0000"),Color.white);
        Stat(790,"TIME LEFT",Mathf.CeilToInt(s.Remaining)+"s",s.Remaining<20 ? new Color(1,.38f,.43f) : Color.white);
        Text(new Rect(1000,34,190,22),"SUIT ENERGY",12,muted);
        for(int i=0;i<5;i++) Box(new Rect(1000+i*34,64,27,16),i<s.Health ? LevelBuilder.Teal : new Color(.18f,.24f,.29f));
        if(s.State==RunState.Playing)
        {
            Box(new Rect(24,616,1232,80),panel);
            Text(new Rect(44,629,900,24),s.Hint,17,LevelBuilder.Gold);
            Text(new Rect(44,663,1020,22),"A / D  MOVE     SPACE  DOUBLE JUMP     F  PULSE ATTACK     ESC  PAUSE     M  SOUND",12,muted);
            if(Action(new Rect(1080,637,150,36),s.Sound.Muted ? "SOUND OFF" : "SOUND ON")) s.Sound.ToggleMute();
        }
        else
        {
            Box(new Rect(0,116,1280,590),new Color(.015f,.025f,.05f,.84f));
            if(s.State==RunState.Menu) Menu(s);
            else if(s.State==RunState.Paused) Pause(s);
            else Result(s);
        }
        GUI.matrix=previous;
    }

    void Menu(GameSession s)
    {
        Box(new Rect(74,168,1132,410),panel);
        Text(new Rect(110,194,620,25),"A SMALL MISSION. A BRIGHTER WORLD.",14,LevelBuilder.Teal);
        Text(new Rect(108,236,650,80),"Bring back the light.",44,Color.white);
        Text(new Rect(110,326,650,75),"Recover six light cores across the station.\nDodge the lasers. Disable the sentinels.\nReach the portal before the 120-second clock expires.",19,muted);
        if(Action(new Rect(110,456,295,62),"START MISSION  [ENTER]")) s.StartRun();
        Text(new Rect(110,535,610,22),"6 CORES    /    3 SENTINELS    /    1 WAY HOME",12,LevelBuilder.Gold);
        Box(new Rect(824,205,1,330),new Color(.18f,.28f,.35f));
        Text(new Rect(861,220,290,34),"FIELD GUIDE",21,Color.white);
        Text(new Rect(861,282,285,220),"A / D or arrows   Move\nSpace, then Space   Double jump\nF or left click   Pulse attack\nEsc   Pause / resume\nM   Toggle audio\n\nTwo hits disable a sentinel.\nPink floors cost suit energy.",16,muted);
        Text(new Rect(75,620,1120,25),"Built with Unity 6  •  Original procedural visuals and audio",13,muted,TextAnchor.MiddleCenter);
    }

    void Pause(GameSession s)
    {
        Box(new Rect(350,200,580,330),panel);
        Text(new Rect(395,235,490,65),"MISSION PAUSED",35,Color.white,TextAnchor.MiddleCenter);
        Text(new Rect(395,316,490,42),"Take a breath. The clock is stopped.",17,muted,TextAnchor.MiddleCenter);
        if(Action(new Rect(440,399,400,54),"RESUME  [ESC]")) s.TogglePause();
    }

    void Result(GameSession s)
    {
        bool won=s.State==RunState.Won;
        Color accent=won ? LevelBuilder.Teal : new Color(1,.38f,.43f);
        Box(new Rect(280,170,720,418),panel);
        Box(new Rect(280,170,720,5),accent);
        Text(new Rect(320,200,640,24),won ? "MISSION COMPLETE" : "MISSION ENDED",15,accent,TextAnchor.MiddleCenter);
        Text(new Rect(320,245,640,65),won ? "LIGHT RESTORED" : "GAME OVER",42,Color.white,TextAnchor.MiddleCenter);
        Text(new Rect(320,324,640,45),s.ResultReason,17,muted,TextAnchor.MiddleCenter);
        Text(new Rect(320,386,640,40),"FINAL SCORE  "+s.Score.ToString("0000")+"     |     CORES  "+s.Collected+" / 6",21,LevelBuilder.Gold,TextAnchor.MiddleCenter);
        if(Action(new Rect(440,473,400,58),"PLAY AGAIN  [R]")) s.Restart();
        Text(new Rect(320,546,640,22),won ? "Includes remaining time and suit-energy bonuses." : "Every new run restores all cores and suit energy.",12,muted,TextAnchor.MiddleCenter);
    }

    void Stat(float x,string name,string value,Color color)
    {
        Text(new Rect(x,34,200,22),name,12,muted);
        Text(new Rect(x,58,200,40),value,26,color);
    }
    void Box(Rect rect,Color color) { Color old=GUI.color; GUI.color=color; GUI.DrawTexture(rect,Texture2D.whiteTexture); GUI.color=old; }
    void Text(Rect rect,string value,int size,Color color,TextAnchor alignment=TextAnchor.UpperLeft)
    {
        label.fontSize=size; label.normal.textColor=color; label.alignment=alignment; label.wordWrap=true;
        GUI.Label(rect,value,label);
    }
    bool Action(Rect rect,string title)
    {
        Color old=GUI.backgroundColor; GUI.backgroundColor=LevelBuilder.Teal;
        bool clicked=GUI.Button(rect,title,button); GUI.backgroundColor=old; return clicked;
    }
}
