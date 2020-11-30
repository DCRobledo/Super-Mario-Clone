using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
	public GameObject cursor;
    
    private bool isOnePlayerGame;
	void Start(){
    	Screen.SetResolution(635,635, false);
    }
    // Update is called once per frame
    void Update()
    {
        UpdateCursor();
    }

    void UpdateCursor(){
    	if(Input.GetKey(KeyCode.DownArrow))
        	cursor.transform.localPosition = new Vector3(-313.75f, -312.64f, cursor.transform.localPosition.z);
        if(Input.GetKey(KeyCode.UpArrow))
        	cursor.transform.localPosition = new Vector3(-313.75f, -311.64f, cursor.transform.localPosition.z);

        if(Input.GetKey(KeyCode.Return))
        	SceneManager.LoadScene("Transition");
    }
}
