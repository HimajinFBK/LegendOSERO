using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetState(OthelloScript.spriteState spriteState)
    {
        var isActive = spriteState != OthelloScript.spriteState.None;
        gameObject.SetActive(isActive);

        if (spriteState == OthelloScript.spriteState.White)
        {
            gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);//白オセロを並べる
        }
        else if (spriteState == OthelloScript.spriteState.Black)
        {
            gameObject.transform.rotation = Quaternion.Euler(270, 0, 0);//黒オセロを並べる
        }
        else
        {
            gameObject.SetActive(false);//None
        }
    }
}
