using UnityEngine;
using System.Collections;

public class SpriteAnimation : MonoBehaviour {
    
    [SerializeField]
    private float speed= .1f;
    
    [SerializeField]
    private bool shouldLoop = false;

    [SerializeField]
    private Sprite[] sprites;


    int index = 0;
    Character parent;

    // Use this for initialization
    void Start () {
        parent = this.gameObject.GetComponentInParent<Character>();
	}
	
	public void Play()
    {
        StartCoroutine("Run");
    }

    private IEnumerator Run()
    {
        int len = sprites.Length;
        int ii = 0;
        float timer = 0;

        while (true)
        {
            parent.GetComponentInChildren<SpriteRenderer>().sprite = sprites[ii % len];
            timer += Time.deltaTime;
            if (timer > speed)
            {
                timer -= speed;
                ++ii;
            }
            yield return null;
        }
      
    }

    public bool Loops
    {
        get
        {
            return shouldLoop;
        }
    }

}
