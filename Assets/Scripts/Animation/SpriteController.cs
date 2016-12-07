using UnityEngine;
using System.Collections;

public class SpriteController : MonoBehaviour {
    SpriteAnimation[] animations;
    SpriteAnimation currentAnimation;
    void Awake()
    {
        animations = GetComponentsInChildren<SpriteAnimation>();
        //default to the first animation in the list
        currentAnimation = animations[0];
    }

    //starts a new animation
    public void PlayAnimation(string name)
    {
        for(int ii = 0; ii < animations.Length; ++ii)
        {
            if(animations[ii].gameObject.name == name)
            {
                animations[ii].Play();
                return;
            }
        }
        //there should always be an animation playing
        //this forces duplication of a string in code, which is bad
        //but this should make errors easy to find
        throw new System.Exception("Attempting to play animation that does not exist: " + name);
    }

    //private because there should never be
    //no animation is playing
    private void StopAnimation()
    {
        currentAnimation.StopAllCoroutines();
    }
}
