
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActorPortrait : MonoBehaviour {

    public string default_anim = "idle";
    public Vector2 offset;
    
    private string current_anim = "";

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PlayAnim(string anim_id, bool loop=true)
    {
        if (anim_id != "" && current_anim != anim_id)
        {
            current_anim = anim_id;

            //TO DO
            //Run your portrait animations from here

        }
    }

    public void PlayDefault()
    {
        PlayAnim(default_anim);
    }
}
