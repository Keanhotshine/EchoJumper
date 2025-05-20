using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerUI : MonoBehaviour ,IPointerDownHandler
{
    public PlayerController playerController;
    public GameManager gamemanager;
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if(PlayerController.onGround)
        {
            StartCoroutine( playerController.Jump());
            GameManager.scoreJump = false;
        }
        else
        {
            StartCoroutine(playerController.BackFlip());
        }
    }
    void Update()
    {
        /*if(gamemanager.health<=0)
        {
            return;
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (PlayerController.onGround)
                {
                    StartCoroutine(playerController.Jump());
                    GameManager.scoreJump = false;
                }
                else
                {
                    StartCoroutine(playerController.BackFlip());
                }
            }
        }*/
    }
}
