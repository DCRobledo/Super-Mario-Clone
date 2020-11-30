using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
	private float transitionTimeLimit = 1.5f;
	private float transitionTimer;
    // Start is called before the first frame update
    void Start()
    {
        transitionTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
       transitionTimer += Time.deltaTime;
       if(transitionTimer >= transitionTimeLimit)
       		SceneManager.LoadScene("Level1");
    }
}
