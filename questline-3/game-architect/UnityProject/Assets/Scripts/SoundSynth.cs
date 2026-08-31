using System.Collections.Generic;
using UnityEngine;

public enum Cue { Start, Jump, Collect, Attack, Hurt, Win, Lose }

public class SoundSynth : MonoBehaviour
{
    readonly Dictionary<Cue,AudioClip> clips=new Dictionary<Cue,AudioClip>();
    AudioSource effects, music;
    AudioClip musicClip;
    public bool Muted { get; private set; }

    void Awake()
    {
        effects=gameObject.AddComponent<AudioSource>(); effects.playOnAwake=false; effects.volume=.25f;
        music=gameObject.AddComponent<AudioSource>(); music.playOnAwake=false; music.volume=.07f; music.loop=true;
        clips[Cue.Start]=Tone("start",440,880,.25f);
        clips[Cue.Jump]=Tone("jump",260,650,.13f);
        clips[Cue.Collect]=Tone("core",660,1320,.18f);
        clips[Cue.Attack]=Tone("pulse",480,120,.1f);
        clips[Cue.Hurt]=Tone("damage",170,55,.22f);
        clips[Cue.Win]=Tone("restored",440,1320,.8f);
        clips[Cue.Lose]=Tone("failure",330,55,.65f);
        musicClip=Music(); music.clip=musicClip; music.Play();
    }

    public void Play(Cue cue) { if(!Muted) effects.PlayOneShot(clips[cue]); }
    public void ToggleMute() { Muted=!Muted; effects.mute=Muted; music.mute=Muted; }

    static AudioClip Tone(string name,float first,float last,float duration)
    {
        const int rate=22050;
        float[] samples=new float[Mathf.CeilToInt(rate*duration)];
        float phase=0;
        for(int i=0;i<samples.Length;i++)
        {
            float t=(float)i/samples.Length;
            phase+=2*Mathf.PI*Mathf.Lerp(first,last,t)/rate;
            float envelope=Mathf.Min(1,t*30)*Mathf.Pow(1-t,2);
            samples[i]=Mathf.Sin(phase)*envelope*.6f;
        }
        AudioClip clip=AudioClip.Create(name,samples.Length,1,rate,false); clip.SetData(samples,0); return clip;
    }

    static AudioClip Music()
    {
        const int rate=22050;
        float[] melody={220,329.63f,440,329.63f,196,293.66f,392,293.66f};
        float[] samples=new float[rate*8];
        for(int i=0;i<samples.Length;i++)
        {
            float t=(float)i/rate;
            int note=Mathf.FloorToInt(t);
            float local=t-note;
            float envelope=Mathf.Sin(local*Mathf.PI);
            samples[i]=envelope*(Mathf.Sin(2*Mathf.PI*melody[note]*local)*.4f+Mathf.Sin(2*Mathf.PI*melody[note]*.5f*local)*.15f);
        }
        AudioClip clip=AudioClip.Create("station ambience",samples.Length,1,rate,false); clip.SetData(samples,0); return clip;
    }
    void OnDestroy()
    {
        foreach(AudioClip clip in clips.Values) Destroy(clip);
        if(musicClip) Destroy(musicClip);
    }
}
