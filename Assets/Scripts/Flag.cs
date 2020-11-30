using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public bool isActive = false;
    void Update()
    {
        if(isActive){
        	Vector3 pos = this.transform.localPosition;
        	if(pos.y<10f){
        		pos.y += Time.deltaTime * 6;
        		this.transform.localPosition = pos;
        	}
        	else if(pos.y >=10f)
        		isActive = false;
        }
    }
}
