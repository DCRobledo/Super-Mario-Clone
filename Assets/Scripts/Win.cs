using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public GameObject cursor;
    
    private bool isReplay;

    // Update is called once per frame
    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor(){
    	if(Input.GetKey(KeyCode.DownArrow)){
    		isReplay= false;
        	cursor.transform.localPosition = new Vector3(-311.6f, -311.47f, cursor.transform.localPosition.z);
    	}
        if(Input.GetKey(KeyCode.UpArrow)){
        	isReplay = true;
        	cursor.transform.localPosition = new Vector3(-311.6f, -310.47f, cursor.transform.localPosition.z);
        }

        if(Input.GetKey(KeyCode.Return)){
        	if(isReplay)
        	SceneManager.LoadScene("Transition");
        	else
        		Application.Quit();
        }
    }
}
