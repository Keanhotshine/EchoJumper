using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;
    
    public float gravity;
    public float jumpHeight;
    public static bool onGround;public Transform groundCheck; public float groundDistance; public LayerMask groudMask; private bool hitGround;
    private Vector3 velocity;
    public Animator animator; public string currentAniName;
    public GameManager gameManager;

    void Start()
    {
        hitGround = true;
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        onGround = Physics.CheckSphere(groundCheck.position, groundDistance, groudMask);
        if (animator.GetCurrentAnimatorClipInfo(0).Length >= 1)
        {
            currentAniName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        }
        if(onGround)
        {
            if (!hitGround)
            {
                if(GameManager.scoreJump)
                {
                    gameManager.CreateEchoCircle(gameManager.transform, 15f);
                    gameManager.combo++;
                    gameManager.comboUI.SetActive(false);
                    gameManager.comboUI.SetActive(true);
                }
                else
                {
                    gameManager.combo = 0;
                }
                hitGround = true;
            }
        }
        else
        {
            hitGround = false;
        }
    }

    void FixedUpdate()
    {
        if (onGround)
        {
            if(velocity.y < 0)
            {
                velocity.y = 0f;
            }
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        animator.SetBool("Land", onGround);
    }

    public IEnumerator Jump()
    {
        animator.Rebind();// 强制退出当前状态
        animator.SetBool("Land", false);
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        animator.SetBool("Jump", true);
        yield return new WaitForEndOfFrame();
        animator.SetBool("Jump", false);
        yield break;
    }

    public IEnumerator BackFlip()
    {
        if (currentAniName == "Jump")
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("BackFlip", true);
            yield return new WaitForEndOfFrame();
            animator.SetBool("BackFlip", false);
        }
        yield break;
    }
}
