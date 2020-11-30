using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public float gravity;
	public Vector2 velocity;
	public float walkSpeed;
	public bool isWalkingLeft = false;

	public float itemMoveSpeed = 4f;

	public Player player;

	public LayerMask floorMask;
	public LayerMask wallMask;

	public AudioClip sound;
	

	private bool grounded = false;
	private bool shouldMove = false;
	private bool ascending = false;

	public enum ItemType{
		mushroom,
		flower,
		star,
		life
	}

	public ItemType type;

	private enum ItemState {
		walking,
		falling,
		dead
	}

	private ItemState state = ItemState.walking;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    	if(shouldMove)
     	UpdateItemPosition();   
    }

    public void PresentItem(Vector2 originalPosition){
    	transform.localPosition = originalPosition;
    	this.transform.GetComponent<SpriteRenderer>().enabled = true;
    	if(this.transform.GetComponent<Animator>() != null)
    		this.transform.GetComponent<Animator>().enabled = true;
    	transform.GetComponent<AudioSource>().PlayOneShot(sound);
    	StartCoroutine (MoveItem (this.gameObject, originalPosition));
    }

    IEnumerator MoveItem(GameObject item, Vector2 originalPosition){
    	while(true){
    		item.transform.localPosition = new Vector2(item.transform.localPosition.x, item.transform.localPosition.y + itemMoveSpeed * Time.deltaTime);
    		if(item.transform.localPosition.y >= originalPosition.y + 1){
    		shouldMove = true;	
    		break;
    		}

    		yield return null;

    	}
    }

    void UpdateItemPosition(){
    	if(type == ItemType.star){
    		if(state != ItemState.dead){
	    		Vector3 pos = transform.localPosition;
	    		Vector3 scale = transform.localScale;

	    		if(state == ItemState.walking){
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
	    				
	    			} else {
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
	    				pos.x += velocity.x * Time.deltaTime;
	    				
	    			}

	    		}

	    		if(pos.y <=2){
	    			velocity.y *= -1;
	    			ascending = true;
	    		}
	    		if(pos.y > 4){
	    			ascending = false;	
	    		}
	    		
	    		CheckWalls(pos, scale.x);

	    		transform.localPosition = pos;
	    		transform.localScale = scale;
    		}
    	}
    	else{
    		if(state != ItemState.dead){
	    		Vector3 pos = transform.localPosition;
	    		Vector3 scale = transform.localScale;

	    		if(state == ItemState.falling){
	    			pos.y += velocity.y * Time.deltaTime;
	    			velocity.y -= gravity * Time.deltaTime;
	    		}

	    		if(state == ItemState.walking){
	    			if(isWalkingLeft){
	    				pos.x -= velocity.x * Time.deltaTime;
	    				
	    			} else {
	    				pos.x += velocity.x * Time.deltaTime;
	    				
	    			}

	    		}

	    		if(velocity.y <= 0)
	    			pos = CheckGround(pos);
	    		
	    		CheckWalls(pos, scale.x);


	    		transform.localPosition = pos;
	    		transform.localScale = scale;
    		}
    	}
    }
    Vector3 CheckGround (Vector3 pos){
    	Vector2 originLeft = new Vector2 (pos.x -0.5f + 0.2f, pos.y - .5f);
    	Vector2 originMiddle = new Vector2 (pos.x, pos.y - .5f);
    	Vector2 originRight = new Vector2 (pos.x + 0.5f -0.2f, pos.y -.5f);

    	RaycastHit2D groundLeft = Physics2D.Raycast (originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D groundMiddle = Physics2D.Raycast (originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D groundRight = Physics2D.Raycast (originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

    	if(groundLeft.collider != null || groundMiddle.collider != null || groundRight.collider != null){
    		RaycastHit2D hitRay = groundLeft;
    		if(groundLeft){
    			hitRay = groundLeft;
    		} else if (groundMiddle){
    			hitRay = groundMiddle;
    		} else if (groundRight){
    			hitRay = groundRight;
    		}

    		if (hitRay.collider.tag == "Player"){
    			if(type == ItemType.flower && player.mode!=Player.PlayerMode.star)
    				player.ChangeMode("fire");
    			if(type == ItemType.star)
    				player.ChangeMode("star");
                if(type == ItemType.mushroom && player.mode==Player.PlayerMode.mini)
                    player.ChangeMode("normal_from_mini");
    				
    			Destroy (this.gameObject);
    		}

    		pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y /2 + .5f;
    		grounded = true;
    		velocity.y = 0;
    		state = ItemState.walking;
    	} else{
    		if(state != ItemState.falling)
    			Fall();
    	}
    	return pos;
    }

    void CheckWalls (Vector3 pos, float direction){
    	Vector2 originTop = new Vector2 (pos.x + direction * 0.4f, pos.y + .5f - 0.2f);
    	Vector2 originMiddle = new Vector2 (pos.x + direction * 0.4f, pos.y);
    	Vector2 originBottom = new Vector2 (pos.x + direction* 0.4f, pos.y - .5f + 0.2f);

    	RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);

    	if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null){
    		RaycastHit2D hitRay = wallTop;
    		if(wallTop){
    			hitRay = wallTop;
    		} else if(wallMiddle){
    			hitRay = wallMiddle;
    		} else if (wallBottom){
    			hitRay = wallBottom;
    		}
    		if (hitRay.collider.tag == "Player"){
    			if(type == ItemType.flower && player.mode!=Player.PlayerMode.star)
    				player.ChangeMode("fire");
    			if(type == ItemType.star)
    				player.ChangeMode("star");
    			if(type == ItemType.mushroom && player.mode==Player.PlayerMode.mini)
    				player.ChangeMode("normal_from_mini");	
    			Destroy (this.gameObject);
    		}

    		isWalkingLeft = !isWalkingLeft;
    	}
    }
    void Fall(){
    	velocity.y = 0;

    	state = ItemState.falling;

    	grounded = false;
    }
}
