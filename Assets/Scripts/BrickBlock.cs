using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BrickBlock : MonoBehaviour
{

	public AudioClip break_sound;
	public AudioClip bump;

	public float bounceHeight = 0.5f;
	public float bounceSpeed = 4f;

	private Vector2 originalPosition;


    public void BrickBlockExplode(){
    	transform.GetComponent<AudioSource>().PlayOneShot(break_sound);
    	Destroy(this.gameObject);
    }

    public void BounceBrickBlock (){
        transform.GetComponent<AudioSource>().PlayOneShot(bump);

        originalPosition = this.transform.localPosition;

    	StartCoroutine (Bounce());
    }

    IEnumerator Bounce (){
    	while(true){
    		transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y + bounceSpeed * Time.deltaTime);
    		if(transform.localPosition.y >= originalPosition.y + bounceHeight)
    			break;
    		yield return null;
    	}
    	while(true){
    		transform.localPosition = new Vector2 (transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);
    		if(transform.localPosition.y <= originalPosition.y){
    			transform.localPosition = originalPosition;
    			break;
    		}
    		yield return null;
    	}
    }
}
