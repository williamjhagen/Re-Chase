using UnityEngine;
using System.Collections;
using System;

public class Kunai : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    public Character player;
    private Direction dir;
    public int speed = 10;

	// Use this for initialization
	void Start () {
        if (player == null) throw new Exception("set the player externally");
        spriteRenderer = GetComponent<SpriteRenderer>();
        dir = player.Direction;

        if (dir == Direction.Left)
        {
            speed *= -1;
            spriteRenderer.flipX = true;
        }
        
        transform.position = player.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
	    if(Math.Abs(transform.position.x - player.transform.position.x) < 15f)
        {
            Vector2 temp = transform.position;
            temp.x += speed * Time.deltaTime;
            transform.position = temp;
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}
