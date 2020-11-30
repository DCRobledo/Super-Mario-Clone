using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
	public float gravity;
	public Vector2 velocity;
	public float walkSpeed;
	public bool isWalkingLeft = false;

	public float FireBallMoveSpeed = 4f;

	public Player player;

	public LayerMask floorMask;
	public LayerMask wallMask;

	public AudioClip sound;
	public AudioClip destroy;

	public bool shouldShoot = true;
	
	private float maximum_height;

	public bool grounded = false;
	private bool shouldMove = false;
	public bool ascending = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    	if(shouldMove)
     	UpdateFireballPosition();
    }
    public void InvokeFireBall(){

    		GetComponent<Animator>().SetBool("isAlive", true);

    		transform.GetComponent<AudioSource>().PlayOneShot(sound);

    		transform.GetComponent<SpriteRenderer>(). enabled = true;

    		Vector3 pos = this.transform.localPosition;
    		if(isWalkingLeft)
    			pos.x = this.player.transform.localPosition.x - 0.8f;
    		else
    			pos.x = this.player.transform.localPosition.x + 0.8f;

    		pos.y = this.player.transform.localPosition.y;

    		shouldShoot = false;
    		shouldMove = true;
    		maximum_height = player.transform.localPosition.y;

    		this.transform.localPosition = pos;
    }
    void UpdateFireballPosition(){
		Vector3 pos = transform.localPosition;
	    Vector3 scale = transform.localScale;

	    if(isWalkingLeft){
	    	pos.x -= velocity.x * Time.deltaTime;
	    	if(!ascending){
	    		pos.y += velocity.y * Time.deltaTime;
	    		if(velocity.y>-10)
	    			velocity.y -= gravity * Time.deltaTime;
	    	}
	    	else if (ascending) {
	    		pos.y += velocity.y * Time.deltaTime;
	    		if(velocity.y<10)
	    			velocity.y += gravity * Time.deltaTime;
	    	}
	    				
	    } 
	    else {
	    	pos.x += velocity.x * Time.deltaTime;
	    	if(!ascending){
	    		pos.y += velocity.y * Time.deltaTime;
	    		if(velocity.y>-10)
	    			velocity.y -= gravity * Time.deltaTime;
	    	}
	    	else if (ascending) {
	    		pos.y += velocity.y * Time.deltaTime;
	    		if(velocity.y<10)
	    			velocity.y += gravity * Time.deltaTime;
	    	}
	    			
	    }

	    float temporal_maximum_height = CheckGround(pos);
	    		
	 	if(pos.y <=1.8){
	    	velocity.y *= -1;
	    	ascending = true;
	    }
	    if(pos.y > 2.2){
	    	ascending = false;	
	    }


	   // if(grounded){
	   // 	ascending = true;
	   // }

	   // if(pos.y >= maximum_height || pos.y >= temporal_maximum_height)
	   // 	ascending = false;

	    CheckWalls(pos, scale.x);

	    transform.localPosition = pos;
	    transform.localScale = scale;
    	
    }
    float CheckGround (Vector3 pos){
    	Vector2 originLeft = new Vector2 (pos.x -0.075f, pos.y - .125f);
    	Vector2 originMiddle = new Vector2 (pos.x, pos.y - .125f);
    	Vector2 originRight = new Vector2 (pos.x + 0.075f, pos.y -.125f);

    	RaycastHit2D groundLeft = Physics2D.Raycast (originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D groundMiddle = Physics2D.Raycast (originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D groundRight = Physics2D.Raycast (originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

    	float temporal_maximum_height = maximum_height;

    	grounded = false;

    	if(groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null){
    		RaycastHit2D hitRay = groundLeft;
    		if(groundLeft){
    			hitRay = groundLeft;
    		} else if (groundMiddle){
    			hitRay = groundMiddle;
    		} else if (groundRight){
    			hitRay = groundRight;
    		}

    		temporal_maximum_height = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + 1;

    		grounded = true;
    	}

    	return temporal_maximum_height; 
    }

    void CheckWalls (Vector3 pos, float direction){
    	Vector2 originTop = new Vector2 (pos.x + direction * 0.4f, pos.y + 0.075f);
    	Vector2 originMiddle = new Vector2 (pos.x + direction * 0.4f, pos.y);
    	Vector2 originBottom = new Vector2 (pos.x + direction* 0.4f, pos.y - 0.075f);

    	RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);

    	if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null){
    		Reset();
    	}
    }

    public void Reset(){
    	GetComponent<Animator>().SetBool("isAlive", false);

		transform.GetComponent<AudioSource>().PlayOneShot(destroy);

    	shouldShoot = true;
    	shouldMove = false;

    	this.transform.GetComponent<SpriteRenderer>(). enabled = false;

    	Vector3 pos = this.transform.localPosition;
    		
    	pos.x = 2;		
    	pos.y = -3;
    	
    	this.transform.localPosition = pos;
    }
}
