using UnityEngine;
public class CameraFollow:MonoBehaviour
{
    public Transform target;
    void LateUpdate(){if(!target)return;Vector3 p=new Vector3(Mathf.Clamp(target.position.x,-7f,7f),Mathf.Clamp(target.position.y+1f,1f,4f),-10f);transform.position=Vector3.Lerp(transform.position,p,Time.deltaTime*3f);}
}
