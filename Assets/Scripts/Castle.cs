using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
	private bool isActive;

	private float castleTimeLimit = 5f;
	private float castleTimer = 0f;
	void Update(){
		if(isActive){
			castleTimer += Time.deltaTime;
			if(castleTimer>=castleTimeLimit)
				Application.LoadLevel("Win");
		}
	}
    void OnTriggerEnter2D (Collider2D other) {
    	if (other.tag == "Player"){
    		other.transform.GetComponent<Player>(). isWalkingToCastle = false;
    		other.transform.GetComponent<SpriteRenderer>(). enabled = false;
    		isActive = true;
    	}
    }
}
