using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJumpai : MonoBehaviour
{
    
    private Rigidbody2D rb;
    private Movement_ai movement;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Space]
    [Header("gen")]
    public int movement_counter=-1;


    void Awake() {
         Application.targetFrameRate = 30;
     }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement_ai>();
    }

    void Update()
    {
        if(movement_counter==149){
            // movement_counter=-1;
            return;
        }
        movement_counter+=1;

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }else if(rb.velocity.y > 0 && !getSpace())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    bool getSpace(){
        int i=movement.my_movements[movement_counter];
        i=i%8;
        if(i>=4){
            return true;
        } else{
            return false;
        }
    }
}
