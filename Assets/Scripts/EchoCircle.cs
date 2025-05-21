using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoCircle : MonoBehaviour
{
    public CircleGenerator circle;
    public float expandSpeed;
    private GameManager gameManager;
    public Material redMat;
    public Material greenMat;
    public Material whiteMat;
    public LineRenderer circleLine;

    private void OnTriggerEnter(Collider other)
    {
        if(expandSpeed!=15)
        {
            if (other.tag == "Player" && PlayerController.onGround)
            {
                circleLine.material = redMat;
                gameManager.Hited();
                gameManager.combo = 0;
                GameManager.scoreJump = false;
            }
            if(other.tag == "Player" && !PlayerController.onGround)
            {
                circleLine.material = greenMat;
                gameManager.scroe += 1 + gameManager.combo;
                GameManager.scoreJump = true;
            }
        }
    }
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        circle.radius = 0;
    }

    
    void Update()
    {
        transform.localScale = new Vector3(circle.radius * 2 - 0.5f, transform.localScale.y, circle.radius * 2 - 0.5f);
        if (circle.radius<=20)
        {
            circle.radius += Time.deltaTime * expandSpeed;
        }
        else
        {
            //Destroy(circle.gameObject);
            gameManager.pool.ReturnToPool(transform.parent.gameObject);
            circle.radius = 0; transform.localScale = new Vector3(0, transform.localScale.y, 0);
            circleLine.material = whiteMat;
        }
    }
}
