using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuestionBlock : MonoBehaviour
{

	public float bounceHeight = 0.5f;
	public float bounceSpeed = 4f;

	public float coinMoveSpeed = 8f;
	public float coinMoveHeight = 3f;
	public float coinFallDistance = 2f;

	public Sprite emptyBlockSprite;

    public Item item;

    public Text score_value;

    public AudioClip coin;

    public bool isItem;

	private Vector2 originalPosition;


	private bool canBounce = true;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
    }


    public void QuestionBlockBounce (){
    	if(canBounce){
            transform.GetComponent<SpriteRenderer>().enabled = true;
    		canBounce = false;

    		StartCoroutine(Bounce());
    	}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeSprite(){
    	GetComponent<Animator>().enabled = false;
    	GetComponent<SpriteRenderer>().sprite = emptyBlockSprite;
    }

    void PresentCoin (){
    	GameObject spinningCoin = (GameObject)Instantiate (Resources.Load("Prefabs/Spinning_Coin", typeof(GameObject)));
    	spinningCoin.transform.SetParent (this.transform.parent);

    	spinningCoin.transform.localPosition = new Vector2(originalPosition.x, originalPosition.y + 1);

        transform.GetComponent<AudioSource>().PlayOneShot(coin);

        //score_value.text = 200.ToString();

    	StartCoroutine (MoveCoin (spinningCoin));
    }

    IEnumerator Bounce (){

    	ChangeSprite();
        if(!isItem)
    	PresentCoin();



    	while(true){
    		transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + bounceSpeed * Time.deltaTime);
    		if(transform.localPosition.y >= originalPosition.y + bounceHeight)
    			break;
           // score_value.GetComponent<Text>().enabled = true;

    		yield return null;

    	}



    	while(true){
    		transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);
    		if(transform.localPosition.y <= originalPosition.y){
    			transform.localPosition = originalPosition;
    			break;
    		}
            //score_value.GetComponent<Text>().enabled = false;

    		yield return null;
    	}
        if(isItem)item.transform.GetComponent<Item>().PresentItem(originalPosition);
    }

    IEnumerator MoveCoin(GameObject coin){
    	while(true){
    		coin.transform.localPosition = new Vector2(coin.transform.localPosition.x, coin.transform.localPosition.y + coinMoveSpeed * Time.deltaTime);
    		if(coin.transform.localPosition.y >= originalPosition.y + 1 + coinMoveHeight)
    		break;

    		yield return null;

    	}
    	while(true){
    		coin.transform.localPosition = new Vector2(coin.transform.localPosition.x, coin.transform.localPosition.y - coinMoveSpeed * Time.deltaTime);
    		if(coin.transform.localPosition.y <= originalPosition.y + 1 + coinFallDistance){
    			Destroy(coin.gameObject);
    			break;
    		}
    		

    		yield return null;
    	}

    }

}
