using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MadNarrativeTargetType{
    Triggerer=0,
    GameObject=5,
    Texture=10,
    Sound=15,
    
}

[System.Serializable]
public class NarrativeTarget {

    public MadNarrativeTargetType type;
    public string data_id;
    public GameObject game_object;
    public Sprite texture;
    public AudioClip sound;
    
    public GameObject GetTargetObject(GameObject triggerer=null)
    {
        if (type == MadNarrativeTargetType.Triggerer)
        {
            return triggerer;
        }
        return game_object;
    }

    public Sprite GetTargetSprite()
    {
        return texture;
    }

    public AudioClip GetTargetSound()
    {
        return sound;
    }
}
