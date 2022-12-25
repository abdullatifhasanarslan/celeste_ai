using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{

    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    [Space]

    [Header("Collision")]

    public Vector2 collisionRadius = new Vector2(5, 5);
    public Vector2 collisionRadiusBOTTOM;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;
    Rigidbody2D PlayerRb;
    BoxCollider2D boxCollider;



    void Awake() {
         Application.targetFrameRate = 30;
     }

    // Start is called before the first frame update
    void Start()
    {
        PlayerRb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {  
        onGround = groundCheck();
        onWall =  wallCheck(Vector2.right) || wallCheck(Vector2.left);
        onRightWall = wallCheck(Vector2.right);
        onLeftWall = wallCheck(Vector2.left);
        wallSide = onRightWall ? -1 : 1;
    }

    private bool groundCheck(){
        RaycastHit2D Hitinfo = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return Hitinfo.collider != null;
    }

    private bool wallCheck(Vector2 direction){
        RaycastHit2D Hitinfo = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, direction, 0.1f, groundLayer);
        return Hitinfo.collider != null;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireCube((Vector3)transform.position  + (Vector3)bottomOffset, (Vector3)collisionRadiusBOTTOM);
        Gizmos.DrawWireCube((Vector3)transform.position + (Vector3)rightOffset, (Vector3)collisionRadius);
        Gizmos.DrawWireCube((Vector3)transform.position + (Vector3)leftOffset, (Vector3)collisionRadius);
    }
}
