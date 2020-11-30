using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyAI : MonoBehaviour
{	
	public float gravity;
	public Vector2 velocity;
	public float walkSpeed;
	public bool isWalkingLeft = true;

    public Player player;

	public LayerMask floorMask;
	public LayerMask wallMask;

    public Text score_value;

    public bool canMove = true;

	private bool grounded = false;
	private bool ascending = false;
    private bool isDead = false;
    private bool isDeadAscending = false;

	private bool shouldDie = false;
	private bool shouldDieSmash = false;
	private float deathTimer = 0;

	public float timeBeforeDestroy = 1.0f;

    public AudioClip stomp;
    public AudioClip smash;

	private enum EnemyState {
		walking,
		falling,
		dead
	}

	private EnemyState state = EnemyState.falling;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;

        Fall();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove)
        UpdateEnemyPosition();
        CheckCrushed();
        CheckSmashed();
        DeathAnimation();   
    }

    public void Crush(){
    	state = EnemyState.dead;
        transform.GetComponent<AudioSource>().PlayOneShot(stomp);
    	GetComponent<Animator>().SetBool("isCrushed", true);
    	GetComponent<Collider2D>().enabled = false;
    	shouldDie = true;
    }

    public void Smash(){
    	state = EnemyState.dead;
    	GetComponent<Collider2D>().enabled = false;
    	shouldDieSmash = true;
    }

    void CheckSmashed(){
    	if(shouldDieSmash){

    		if (deathTimer <= timeBeforeDestroy){
    			deathTimer += Time.deltaTime;

    			Vector3 pos = transform.localPosition;
	    		Vector3 scale = transform.localScale;

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

    		} else{
    			shouldDieSmash = false;
    			Destroy (this.gameObject);
    		}
    	}
    }

    void CheckCrushed(){
    	if(shouldDie){

    		if (deathTimer <= timeBeforeDestroy){
    			deathTimer += Time.deltaTime;

    		} else{
    			shouldDie = false;
    			Destroy (this.gameObject);
    		}
    	}
    }

    void UpdateEnemyPosition(){
    	if(state != EnemyState.dead){
    		Vector3 pos = transform.localPosition;
    		Vector3 scale = transform.localScale;

    		if(state == EnemyState.falling){
    			pos.y += velocity.y * Time.deltaTime;
    			velocity.y -= gravity * Time.deltaTime;
    		}

    		if(state == EnemyState.walking){
    			if(isWalkingLeft){
    				pos.x -= velocity.x * Time.deltaTime;
    				scale.x = -1;
    			} else {
    				pos.x += velocity.x * Time.deltaTime;
    				scale.x = 1;
    			}

    		}

    		if(velocity.y <= 0)
    			pos = CheckGround(pos);
    		
    		CheckWalls(pos, scale.x);


    		transform.localPosition = pos;
    		transform.localScale = scale;
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

    		if (hitRay.collider.tag == "Player" && !player.isRecovering){
                if(player.mode == Player.PlayerMode.star){
                    this.transform.GetComponent<AudioSource>().PlayOneShot(smash);
                    Death();
                }
                else if(player.mode == Player.PlayerMode.fire){
                    player.ChangeMode("normal");
                }
                else if(player.mode == Player.PlayerMode.normal)
                    player.ChangeMode("mini");
                else if(player.mode==Player.PlayerMode.mini && !player.isRecovering)
    			player.transform.GetComponent<Player>(). Death();
    		}

            if(hitRay.collider.tag == "Hole"){
                Destroy(this.gameObject);
            }
            if(hitRay.collider.tag == "Fireball"){
                this.transform.GetComponent<AudioSource>().PlayOneShot(smash);
                Death();
                hitRay.collider.transform.GetComponent<FireBall>().Reset();
            }

    		pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y /2 + .5f;
    		grounded = true;
    		velocity.y = 0;
    		state = EnemyState.walking;
    	} else{
    		if(state != EnemyState.falling)
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
    		if (hitRay.collider.tag == "Player" && !player.isRecovering){
    			if(player.mode == Player.PlayerMode.star){
                    this.transform.GetComponent<AudioSource>().PlayOneShot(smash);
    				Death();
                }
                else if(player.mode == Player.PlayerMode.fire){
                    player.ChangeMode("normal");
                    
                }
                else if(player.mode == Player.PlayerMode.normal)
                	player.ChangeMode("mini");

                else if(player.mode == Player.PlayerMode.mini && !player.isRecovering)
                    player.transform.GetComponent<Player>(). Death();
            }
            if(hitRay.collider.tag == "Fireball"){
                this.transform.GetComponent<AudioSource>().PlayOneShot(smash);
                Death();
                hitRay.collider.transform.GetComponent<FireBall>().Reset();
            }

    		isWalkingLeft = !isWalkingLeft;
    	}
    }

    void CheckCeilingRays (Vector3 pos){
        float toolX = 0.3f;
        float toolY = 0.3f;

        Vector2 originLeft = new Vector2 (pos.x - toolX, pos.y + toolY);
        Vector2 originMiddle = new Vector2 (pos.x, pos.y + toolY);
        Vector2 originRight = new Vector2 (pos.x + toolX, pos.y + toolY);


        RaycastHit2D ceilLeft = Physics2D.Raycast (originLeft, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilMiddle = Physics2D.Raycast (originMiddle, Vector2.up, velocity.y * Time.deltaTime, floorMask);
        RaycastHit2D ceilRight = Physics2D.Raycast (originRight, Vector2.up, velocity.y * Time.deltaTime, floorMask);

        if(ceilLeft.collider != null || ceilMiddle.collider != null || ceilRight.collider != null){
            RaycastHit2D hitRay = ceilLeft;
            if(ceilLeft){
                hitRay = ceilLeft;
            } else if (ceilMiddle){
                hitRay = ceilMiddle;
            } else if(ceilRight){
                hitRay = ceilRight;
            }

            
            if(hitRay.collider.tag == "Fireball"){
                this.transform.GetComponent<AudioSource>().PlayOneShot(smash);
                Death();
                hitRay.collider.transform.GetComponent<FireBall>().Reset();
            }
        }
    }

    void OnBecameVisible(){
    	enabled = true;
    }
    void Fall(){
    	velocity.y = 0;

    	state = EnemyState.falling;

    	grounded = false;
    }
    public void Death(){
        isDeadAscending = true;
        canMove = false;
        isDead = true;

        GetComponent<Collider2D>().enabled = false;

        this.transform.GetComponent<Animator>(). SetBool("isDead", true);

        velocity = new Vector2 (velocity.x, 10f * 2.2f);

        Vector3 scale = this.transform.localScale;
        scale.y = -1;
        this.transform.localScale = scale;
    }
    void DeathAnimation(){
        if(isDead){
            Vector3 pos = this.transform.localPosition;
            pos.x += velocity.x * Time.deltaTime;
            if(isDeadAscending){
                pos.y += velocity.y * Time.deltaTime * 0.4f;
                velocity.y -= gravity * Time.deltaTime;
                this.transform.localPosition = pos;
                if(velocity.y <= 0)
                        isDeadAscending = false;
                }
                else if(!isDeadAscending){
                    pos.y -= velocity.y * Time.deltaTime * 0.4f;
                    velocity.y += gravity * Time.deltaTime;
                    this.transform.localPosition = pos;
                }
                if(pos.y <= -60){
                    Destroy(this.gameObject);
            }
        }
    }

}
