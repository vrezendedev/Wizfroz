using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class AudioEventManager
{
    public static UnityAction<AudioClip, string> ChangeMusic;
    public static UnityAction ChangeAudioVolume;
    public static UnityAction<AudioClip> PlayUISound;
}
