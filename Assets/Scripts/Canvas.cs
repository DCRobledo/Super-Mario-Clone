using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Canvas : MonoBehaviour
{
	public Text mario;
    public Text score;
    public Text coins;
    public Text x;
    public Text stage;
    public Text world;
    public Text time;
    public Text time_counter;
    public GameObject canvas_coin;

    public bool isActive = true;


    private float time_tool = 400f;
    private int timer = 0;
    private int score_points = 0;
    private int coins_amount = 0;
    // Start is called before the first frame update
    void Start()
    {
    	
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateScore();
    }

    void UpdateTime(){
        if(isActive){
        	timer++;
        	if(timer%50==0){
        		if(time_tool<=400 && time_tool>=0){
        		time_tool -= 0.5f;
        		time_tool = (int) time_tool;
        		time_counter.text = time_tool.ToString();
        		}
    		}
        }
    }

    public void CoinObtained(){
    	coins_amount++;
    	score_points += 200;
    	if(coins_amount<10){
    		coins.text = "0" + coins_amount.ToString();
    	}
    	else
    	coins.text = coins_amount.ToString();
    }
    void UpdateScore(){
    	if(score_points >= 0 && score_points < 10){
    		score.text = "00000" + score_points.ToString();
    	} else if(score_points>= 10 && score_points< 100){
    		score.text = "0000" + score_points.ToString();
    	} else if(score_points>= 100 && score_points< 1000){
    		score.text = "000" + score_points.ToString();
    	} else if(score_points>= 1000 && score_points< 10000){
    		score.text = "00" + score_points.ToString();
    	} else if(score_points>= 10000 && score_points< 100000){
    		score.text = "0" + score_points.ToString();
    	} else score.text = score_points.ToString();
    }
}
