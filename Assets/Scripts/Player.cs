using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{	
	public float jumpVelocity;
	public float bounceVelocity;
	public Vector2 velocity;
	public float gravity;

    public Sprite normal_idle;
    public Sprite fire_idle;

    public RuntimeAnimatorController normalAnimator;
    public RuntimeAnimatorController fireAnimator;
    public RuntimeAnimatorController starAnimator;
    public RuntimeAnimatorController miniAnimator;

	public LayerMask wallMask;
	public LayerMask floorMask;

    public AudioClip jump_Super;
    public AudioClip jump_Mini;
    public AudioClip item_Consumed;
    public AudioClip item_Lost;
    public AudioClip starModeMusic;
    public AudioClip stageClear;
    public AudioClip flagPole;
    public AudioClip marioDies;

    public bool isRecovering = false;

    public GameObject Canvas;

    private AudioSource audio;

    private bool canMove = true;
    private bool isStar = false;
    private bool crouch = false;
    private bool isDead = false;
    private bool isDeadAscending = false;
    private bool shoot = false;
    private bool isFlag = false;
    private bool isWaitingFlag=false;
    public bool isWalkingToCastle=false;
    private bool playingStageClear = false;


    private float flagTimeLimit = 1.8f;
    private float flagTimer = 0f;
    private float fireballTimeLimit = 0.2f;
    private float fireballTimer = 0f;
    private float starTimeLimit = 12f;
    private float starTimer = 0f;
    private float waitDeathTimer = 0f;
    private float waitDeathTimeLimit = 0.7f;

	public enum PlayerState{
		jumping,
		idle,
		walking,
		bouncing
	}

    private PlayerState playerState = PlayerState.idle;

    public enum PlayerMode {
        normal,
        mini,
        fire,
        star
    }

	public PlayerMode mode = PlayerMode.normal;

	public bool grounded = false;
	private bool bounce = false;

	private bool walk, walk_left, walk_right, jump;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        Fall();
        audio = transform.GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove){
            CheckPlayerInput();
            UpdatePlayerPosition(); 
            CheckCrouch(); 
        }
        if(mode == PlayerMode.fire)
        ShootFireball();

        StarMode();
        
        if(!isFlag && !isWaitingFlag && !isWalkingToCastle)
   		UpdateAnimationStates();

        WinAnimation();

        DeathAnimation();

   		
    }

    void UpdatePlayerPosition (){

    	Vector3 pos = transform.localPosition;
    	Vector3 scale = transform.localScale;

    	if(walk && !crouch){
    		if(walk_left){

    			pos.x -= velocity.x * Time.deltaTime;
    			scale.x = -1;

    		}
    		if(walk_right){
    			pos.x += velocity.x * Time.deltaTime;
    			scale.x = 1;

    		} 

    		pos = CheckWallRays(pos, scale.x);
    	}

    	if(jump && playerState != PlayerState.jumping){
    		playerState = PlayerState.jumping;
            if(this.mode == PlayerMode.mini)
                audio.PlayOneShot(jump_Mini);
            else
                audio.PlayOneShot (jump_Super);
    		velocity = new Vector2 (velocity.x, jumpVelocity);
    	}

    	if(playerState == PlayerState.jumping){
    		pos.y += velocity.y * Time.deltaTime;

    		velocity.y -= gravity * Time.deltaTime;
    	}

    	if(bounce && playerState != PlayerState.bouncing){
    		playerState = PlayerState.bouncing;

    		velocity = new Vector2 (velocity.x, bounceVelocity);
    	}

    	if(playerState == PlayerState.bouncing){
    		pos.y += velocity.y * Time.deltaTime;

    		velocity.y -= gravity * Time.deltaTime;
    	}

    	if(velocity.y<=0)
    		pos = CheckFloorRays(pos);

    		if(velocity.y>=0)
    		pos = CheckCeilingRays(pos);

    	transform.localPosition = pos;
    	transform.localScale = scale;
    }

    void UpdateAnimationStates(){
       

    	if(grounded && !walk && !bounce){
    		GetComponent<Animator>().SetBool("isJumping", false);
    		GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("Ground", true);
    	}
    	if(grounded && walk){
    		GetComponent<Animator>().SetBool("isJumping", false);
    		GetComponent<Animator>().SetBool("isRunning", true);
    	}
    	if(playerState == PlayerState.jumping){
    		GetComponent<Animator>().SetBool("isJumping", true);
    		GetComponent<Animator>().SetBool("isRunning", false);
    	}                    
    }

    void CheckPlayerInput (){
    	bool input_left = Input.GetKey(KeyCode.LeftArrow);
    	bool input_right = Input.GetKey(KeyCode.RightArrow);
    	bool input_space = Input.GetKeyDown(KeyCode.Space);
        bool input_shoot = Input.GetKeyDown(KeyCode.X);

        shoot = input_shoot;

    	walk = input_left || input_right;
    	walk_left = input_left && !input_right;
    	walk_right = !input_left && input_right;
    	jump = input_space;
        grounded = !input_space;
    }

    Vector3 CheckWallRays(Vector3 pos, float direction){
        float toolX = 0.4f;
        float toolY = 0.8f;

        if(this.mode ==  PlayerMode.mini){
            toolX = 0.2f;
            toolY = 0.2f;
        }

    	Vector2 originTop = new Vector2 (pos.x + direction * toolX, pos.y + toolY);
    	Vector2 originMiddle = new Vector2 (pos.x + direction * toolX, pos.y);
    	Vector2 originBottom = new Vector2 (pos.x + direction * toolX, pos.y - toolY);

    	RaycastHit2D wallTop = Physics2D.Raycast (originTop, new Vector2 (direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallMiddle = Physics2D.Raycast (originMiddle, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);
    	RaycastHit2D wallBottom = Physics2D.Raycast (originBottom, new Vector2(direction, 0), velocity.x * Time.deltaTime, wallMask);

    	if(wallTop.collider != null || wallMiddle.collider != null || wallBottom.collider != null){
            RaycastHit2D hitRay = wallMiddle;
            if(wallTop){
                hitRay = wallTop;
            } else if(wallMiddle){
                hitRay = wallMiddle;
            } else if (wallBottom) {
                hitRay = wallBottom;
            }

            if(hitRay.collider.tag == "Enemy"){
                if(this.mode == PlayerMode.fire)
                ChangeMode("normal");
                if(this.mode == PlayerMode.normal)
                ChangeMode("mini");
                if(this.mode == PlayerMode.mini)
                Death();
                }
            if(hitRay.collider.tag == "Flag"){
                GameObject.Find("flag_2").transform.GetComponent<Flag>().isActive = true;
                FlagAnimation();
            }
            if(hitRay.collider.tag != "Fireball" && hitRay.collider.tag != "Flag")
    		pos.x -= velocity.x * Time.deltaTime * direction;
    	}
    	return pos;
    }

    Vector3 CheckFloorRays(Vector3 pos){
        float toolX = 0.3f;
        float toolY = 1f;

        float toolGrounded = 1f;

        if(this.mode ==  PlayerMode.mini){
            toolX = 0.6f;
            toolY = 0.6f;

            toolGrounded = 0.5f;
        }


    	Vector2 originLeft = new Vector2 (pos.x - toolX, pos.y - toolY);
    	Vector2 originMiddle = new Vector2 (pos.x, pos.y -toolY);
    	Vector2 originRight = new Vector2 (pos.x +toolX, pos.y -toolY);


    	RaycastHit2D floorLeft = Physics2D.Raycast (originLeft, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D floorMiddle = Physics2D.Raycast (originMiddle, Vector2.down, velocity.y * Time.deltaTime, floorMask);
    	RaycastHit2D floorRight = Physics2D.Raycast (originRight, Vector2.down, velocity.y * Time.deltaTime, floorMask);

    	if(floorLeft.collider != null || floorMiddle.collider != null || floorRight.collider != null){
    		RaycastHit2D hitRay = floorRight;
    		if(floorLeft){
    			hitRay = floorLeft;
    		} else if(floorMiddle){
    			hitRay = floorMiddle;
    		} else if (floorRight) {
    			hitRay = floorRight;
    		}

    		if(hitRay.collider.tag == "Enemy"){
    			hitRay.collider.GetComponent<EnemyAI>().Crush();
    			bounce = true;
    		}
            if(hitRay.collider.tag == "Hole"){
                Death();
            }

            if(hitRay.collider.tag != "Fireball"){


    		playerState = PlayerState.idle;
    		
    		grounded = true;

    		velocity.y = 0;

    		pos.y = hitRay.collider.bounds.center.y + hitRay.collider.bounds.size.y / 2 + toolGrounded;
        }
    	} else {
    		if(playerState != PlayerState.jumping){
    			Fall();
    		}
    	}
    	return pos;
    }

    Vector3 CheckCeilingRays (Vector3 pos){
        float toolX = 0.3f;
        float toolY = 1f;

        float toolGrounded = 1f;

        if(this.mode == PlayerMode.mini){
            toolX = 0.15f;
            toolY = 0.15f;

            toolGrounded = 0.5f;
        }


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

    		if(hitRay.collider.tag == "QuestionBlock"){
    			hitRay.collider.GetComponent<QuestionBlock>().QuestionBlockBounce();
                if(!hitRay.collider.GetComponent<QuestionBlock>().isItem);
                Canvas.GetComponent<Canvas>(). CoinObtained();

    		}
            if(hitRay.collider.tag == "BrickBlock"){
                if(this.mode == PlayerMode.mini)
                    hitRay.collider.GetComponent<BrickBlock>(). BounceBrickBlock();
                else
                    hitRay.collider.GetComponent<BrickBlock>().BrickBlockExplode();
            }

            if(hitRay.collider.tag != "Fireball"){

        		pos.y = hitRay.collider.bounds.center.y - hitRay.collider.bounds.size.y / 2 - toolGrounded;
        		Fall();
            }
    	}
    	return pos;
    }

    void Fall(){
    	velocity.y = 0;
    	playerState = PlayerState.jumping;

    	grounded = false;
    	bounce = false;
    	
    }

    public void ChangeMode(string mode){
        if(mode.Equals("star")){
            if(this.mode == PlayerMode.fire)
            this.transform.GetComponent<Animator>().runtimeAnimatorController = normalAnimator;
            this.mode = PlayerMode.star;
            GetComponent<Animator>().SetBool("starMode", true);
            GameObject.Find("Game").transform.GetComponent<AudioSource>().Pause();
            isStar = true;
        }
        if(mode.Equals("fire")){
            if(this.mode == PlayerMode.star)
            this.transform.GetComponent<Animator>().runtimeAnimatorController = normalAnimator;
            this.mode = PlayerMode.fire;
            GetComponent<Animator>().SetBool("fireMode", true);
        }
        if(mode.Equals("normal") || mode.Equals("normal_from_mini")){
            GetComponent<Animator>().SetBool("isTransforming", true);
            isRecovering = true;
            this.mode = PlayerMode.normal;
        }
        if(mode.Equals("mini")){
        	GetComponent<Animator>().SetBool("miniMode", true);
            isRecovering = true;
            this.mode = PlayerMode.mini;
        }

        canMove = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in o){
            enemy.transform.GetComponent<EnemyAI>(). canMove = false;
        }
        if(mode.Equals("fire") || mode.Equals("star") || (mode.Equals("normal_from_mini")))
            transform.GetComponent<AudioSource> ().PlayOneShot (item_Consumed);

        if((mode.Equals("normal") || mode.Equals("mini")))
            transform.GetComponent<AudioSource> ().PlayOneShot (item_Lost);

        StartCoroutine (ProcessChangeModeAfter(1f, mode));
    }

    IEnumerator ProcessChangeModeAfter(float delay, string mode){
        yield return new WaitForSeconds(delay);

        canMove = true;

        if(mode.Equals("star")){
            GetComponent<Animator>().SetBool("starMode", false);
            this.transform.GetComponent<Animator>().runtimeAnimatorController = starAnimator;
            this.transform.GetComponent<AudioSource>().PlayOneShot(starModeMusic);
            
        }
        if(mode.Equals("fire")){
            GetComponent<Animator>().SetBool("fireMode", false);
            this.transform.GetComponent<Animator>().runtimeAnimatorController = fireAnimator;
        }

        if(mode.Equals("mini")){
        	GetComponent<Animator>().SetBool("miniMode", false);
            this.transform.GetComponent<Animator>().runtimeAnimatorController = miniAnimator;
        }
        
        if(mode.Equals("normal") || (mode.Equals("normal_from_mini"))){
            GetComponent<Animator>().SetBool("isTransforming", false);
            this.transform.GetComponent<Animator>().runtimeAnimatorController = normalAnimator;
        }

       isRecovering = false;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in o){
            enemy.transform.GetComponent<EnemyAI>(). canMove = true;
        }
    }

    void ShootFireball(){
        if(shoot){
            GameObject[] o = GameObject.FindGameObjectsWithTag("Fireball");
            foreach(GameObject fireball in o){
                if(fireball.transform.GetComponent<FireBall>().shouldShoot){
                    Vector3 scale = transform.localScale;
                    if(scale.x == -1)
                        fireball.transform.GetComponent<FireBall>().isWalkingLeft = true;
                    if(scale.x == 1)
                        fireball.transform.GetComponent<FireBall>().isWalkingLeft = false;
                    fireball.transform.GetComponent<FireBall>().InvokeFireBall();
                    break;
                }
            }

            

            GetComponent<Animator>().SetBool("isShooting", true);
            fireballTimer = 0f;
        }
        fireballTimer += Time.deltaTime;
        if(fireballTimer>=fireballTimeLimit){
            GetComponent<Animator>().SetBool("isShooting", false);
            fireballTimer = 0f;
        }
    }

    void StarMode(){
        if(isStar){
            starTimer += Time.deltaTime;
            if(starTimer>= starTimeLimit){
                isStar = false;
                transform.GetComponent<AudioSource> ().Stop();
                GameObject.Find("Game").transform.GetComponent<AudioSource>().UnPause();
                ChangeMode("normal");
            }
        }
    }

    void CheckCrouch(){
    	if(this.mode != PlayerMode.mini && (playerState == PlayerState.idle || playerState == PlayerState.walking)){
	    	if(Input.GetKey(KeyCode.DownArrow)){
	    		GetComponent<Animator>().SetBool("crouch", true);
	    		crouch = true;
	    	}
	    	else{
	    		GetComponent<Animator>().SetBool("crouch", false);
	    		crouch = false;
	    	}
	    }
    }

    void FlagAnimation(){
        Canvas.transform.GetComponent<Canvas>(). isActive = false;
        GetComponent<Animator>().SetBool("flag", true);
        Vector3 pos = this.transform.localPosition;
        pos.x = 210.45f;
        transform.localPosition = pos;
        canMove = false;
        isFlag = true;
        GameObject.Find("Game").transform.GetComponent<AudioSource>().Stop();
        this.transform.GetComponent<AudioSource>(). PlayOneShot(flagPole);
    }

    void WinAnimation(){
        if(isFlag){
            Vector3 pos = this.transform.localPosition;
            if(pos.y>3.5){
                pos.y -=  4 * Time.deltaTime;
                this.transform.localPosition = pos;
            }
            if(pos.y<=3.5){
                Vector3 scale = transform.localScale;
                scale.x = -1;
                this.transform.localScale = scale;
                pos.x = 211.45f;
                this.transform.localPosition = pos;
                isWaitingFlag=true;
                isFlag=false;
                audio.PlayOneShot (jump_Super);
                GetComponent<Animator>().SetBool("flag", false);
                this.GetComponent<Animator>().SetBool("isJumping", true);
            }
        }
        else if(isWaitingFlag){
            Vector3 pos = this.transform.localPosition;
            Vector3 scale = this.transform.localScale;
            if(pos.y > 2.5){
                pos.x += Time.deltaTime * 2f;
                pos.y += velocity.y * Time.deltaTime;

                velocity.y -= gravity * Time.deltaTime * 0.5f;

                this.transform.localPosition = pos;
            }
            else if(pos.y<=2.5){
                pos.y = 2.5f;
                this.transform.localPosition = pos;
                this.GetComponent<Animator>().SetBool("isJumping", false);
                isWaitingFlag = false;
                isWalkingToCastle = true;
            }
        }
        else if(isWalkingToCastle){
            if(flagTimer <= flagTimeLimit)
                flagTimer += Time.deltaTime;
            else if(flagTimer == flagTimeLimit){
                this.transform.GetComponent<AudioSource>(). PlayOneShot(stageClear);
                flagTimer += Time.deltaTime;
            }
            else{
                if(!playingStageClear){
                    playingStageClear = true;
                    this.transform.GetComponent<AudioSource>(). PlayOneShot(stageClear);
                }
                this.GetComponent<Animator>().SetBool("isRunning", true);
                Vector3 pos = this.transform.localPosition;
                Vector3 scale = this.transform.localScale;

                pos.x += velocity.x * Time.deltaTime * 0.5f;
                scale.x = 1;

                this.transform.localPosition = pos;
                this.transform.localScale = scale;
            }
        }
    }
    public void Death(){
        isDeadAscending = true;
        canMove = false;
        isDead = true;

        GameObject.Find("Game").transform.GetComponent<AudioSource>().Pause();
        this.transform.GetComponent<AudioSource>(). PlayOneShot(marioDies);

        GameObject[] o = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in o){
            enemy.transform.GetComponent<EnemyAI>(). canMove = false;
        }

        velocity = new Vector2 (velocity.x, bounceVelocity * 2.2f);

        this.transform.GetComponent<Animator>().runtimeAnimatorController = miniAnimator;
        this.transform.GetComponent<Animator>(). SetBool("isDead", true);
    }
    void DeathAnimation(){
        if(isDead){
            if(waitDeathTimer<waitDeathTimeLimit)
                waitDeathTimer += Time.deltaTime;
            else{
                Vector3 pos = this.transform.localPosition;
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
                    SceneManager.LoadScene("Transition");
                }
            }
        }
    }
}
