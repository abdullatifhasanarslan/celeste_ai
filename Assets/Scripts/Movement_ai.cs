using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Movement_ai : MonoBehaviour
{
	private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    // private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float maxSpeed = 10;
    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
	[SerializeField] GameObject ai;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool canWallSlide;
    public bool wallGrab;
    public bool pushingWall;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;
    public bool isJumping;
    public bool isFalling;
    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("gen")]
    // SpriteRenderer m_SpriteRenderer;
    // Color m_NewColor;
    // float m_Red, m_Blue, m_Green;

    public int[] my_movements;
	public int movement_counter=-1;

    public int mutation_rate=5;			//percent
    public int crossing_over_rate=5;	//percent
    public int my_id;
    // public float best_score=100000;
    public float score=0;

    // [Space]
    // [Header("Polish")]
    // public ParticleSystem dashParticle;
    // public ParticleSystem jumpParticle;
    // public ParticleSystem wallJumpParticle;
    // public ParticleSystem slideParticle;


	void Awake() {
         Application.targetFrameRate = 30;
     }
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        my_movements=new int[150];
        int[,] best_movements=FindObjectOfType<recorder>().movement_input;
        // anim = GetComponentInChildren<AnimationScript>();
        
        my_id=FindObjectOfType<recorder>().alive++;
		if(my_id < FindObjectOfType<recorder>().survivors){
	        for(int i=0;i<150;i++){
				my_movements[i]=best_movements[my_id, i];
			}
			// m_SpriteRenderer = GetComponent<SpriteRenderer>();
			// m_SpriteRenderer.color = Color.blue;
		}else{
	        for(int i=0;i<150;i++){
	        	if(UnityEngine.Random.Range(0, 100) < mutation_rate){
	        		my_movements[i]=UnityEngine.Random.Range(0, 128);
	        	}else if(UnityEngine.Random.Range(0, 100) < crossing_over_rate){
	        		my_movements[i]=best_movements[UnityEngine.Random.Range(1, FindObjectOfType<recorder>().survivors), i];
	        	}else{
	        		my_movements[i]=best_movements[0, i];
	        	}
	        }
	    }
    }
    void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
    	if(movement_counter==149){
    		// movement_counter=-1;
    		// if(FindObjectOfType<recorder>().best_score>best_score){
    		// 	// Debug.Log(best_score);
    		// 	FindObjectOfType<recorder>().best_score=best_score;
    		// 	int[] best_movements=FindObjectOfType<recorder>().movement_input;
    		// 	for(int i=0;i<150;i++){
    		// 		best_movements[i]=my_movements[i];
    		// 	}
    		// }
    		// FindObjectOfType<recorder>().evaluate_score(best_score,my_movements);
    		FindObjectOfType<recorder>().evaluate_score(score,my_movements);
    		// FindObjectOfType<recorder>().alive-=1;
    		Destroy(gameObject);
    		return;
    	}
    	movement_counter+=1;


        float x = getX();
        float y = getY();
        float xRaw = x;
        float yRaw = y;
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        // anim.SetHorizontalMovement(x, y, rb.velocity.y);

        if (coll.onWall && getShift() && canMove)
        {
            // if(side != coll.wallSide)
            //     anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        if (getShiftUp() || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumpai>().enabled = true;
        }
        
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(rb.velocity.y < 0){
            isJumping = false;
            isFalling = true;
        }

        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab && isFalling)
            {
                //isFalling = false;
                wallSlide = true;
                WallSlide();
            }
        }

        //TERMINAL VELOCITY WHEN FALLING
        if(isFalling){
            if(rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
        // if (!coll.onWall || coll.onGround)
        //     wallSlide = false;

        if (getSpaceDown())
        {
            // anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();
        }

        if (getCtrlDown() && !hasDashed)
        {
            if(xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

    	score+=Vector3.Distance(transform.position, new Vector3(8.56f, 4.73f, 0f));
    	// float score=Vector3.Distance(transform.position, new Vector3(0f, 4.5f, 0f));
    	// if(best_score>score){
	    // 	// Debug.Log(score);
	    // 	// Debug.Log(transform.position);
	    // 	// Debug.Log(new Vector3(0f, 4.5f, 0f));
    	// 	best_score=score;
    	// }


        //WallParticle(y);

        if (wallGrab || wallSlide || !canMove)
            return;

        if(x > 0)
        {
            side = 1;
            // anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            // anim.Flip(side);
        }


    }

    float getX(){
    	int i=my_movements[movement_counter];
    	i=i%64;
    	if(i>=32) return -1;
    	i=i%16;
		if(i>=8) return 1;

		return 0;
    	// if(i==0 | i==3 | i==6 | i==9 | i==15 | i==17 | i==20 | i==24){
    	// 	return -1;
    	// } else if(i==2 | i==4 | i==8 | i==10 | i==16 | i==18 | i==22 | i==25){
    	// 	return 1;
    	// } else{
    	// 	return 0;
    	// }
    }

    float getY(){
    	int i=my_movements[movement_counter];
    	if(i>=64) return 1;
    	i=i%32;
		if(i>=16) return -1;

		return 0;
    
    	// if(i==0 | i==2 | i==5 | i==12 | i==15 | i==16 | i==19 | i==23){
    	// 	return 1;
    	// } else if(i==3 | i==4 | i==7 | i==13 | i==17 | i==18 | i==21){
    	// 	return -1;
    	// } else{
    	// 	return 0;
    	// }
    }

    bool getShift(){
    	int i=my_movements[movement_counter];
    	i=i%4;
    	if(i>=2){ 
    		return true;
    	}
    	
    	return false;
    	
    	// if(i==12 | i==13 | i==14){
    	// 	return true;
    	// } else{
    	// 	return false;
    	// }
    }

    bool getShiftUp(){
    	if(movement_counter==0){
    		return false;
    	}


    	int i=my_movements[movement_counter-1];
    	i=i%4;
    	if((i>=2) & !getShift()){
    		return true;
    	} else{
    		return false;
    	}	
    }

    bool getShiftDown(){
    	if(movement_counter==0){
    		return false;
    	}

    	int i=my_movements[movement_counter-1];
    	i=i%4;
    	if( !(i>=2) & getShift()){
    		return true;
    	} else{
    		return false;
    	}	
    }

    bool getSpace(){
    	int i=my_movements[movement_counter];
    	i=i%8;
    	if(i>=4){
    		return true;
    	} else{
    		return false;
    	}
    }

    bool getSpaceUp(){
    	if(movement_counter==0){
    		return false;
    	}

    	int i=my_movements[movement_counter-1];
    	i=i%8;
    	if(i>=4 & !getSpace()){
    		return true;
    	} else{
    		return false;
    	}
    }

    bool getSpaceDown(){
    	if(movement_counter==0){
    		return false;
    	}

    	int i=my_movements[movement_counter-1];
    	i=i%8;
    	if( !(i>=4) & getSpace()){
    		return true;
    	} else{
    		return false;
    	}
    }

    bool getCtrl(){
    	int i=my_movements[movement_counter];
    	i=i%2;
    	if(i>=1){
    		return true;
    	} else{
    		return false;
    	}
    }

    bool getCtrlUp(){
    	if(movement_counter==0){
    		return false;
    	}

    	int i=my_movements[movement_counter-1];
    	i=i%2;
    	if( i>=2 & !getCtrl()){
    		return true;
    	} else{
    		return false;
    	}
    }

    bool getCtrlDown(){
    	if(movement_counter==0){
    		return false;
    	}

    	int i=my_movements[movement_counter-1];
    	i=i%2;
    	if( !(i>=2) & getCtrl()){
    		return true;
    	} else{
    		return false;
    	}
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        isJumping = false;
        isFalling = false;
        wallSlide = false;

        // side = anim.sr.flipX ? -1 : 1;

        // jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        // Camera.main.transform.DOComplete();
        // Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        // FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

        isFalling = false;
        hasDashed = true;

        // anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(14, 0, .3f, RigidbodyDrag);

        // dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumpai>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        // dashParticle.Stop();
        rb.gravityScale = 3;
        GetComponent<BetterJumpai>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            // anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        // if(coll.wallSide != side)
        //  anim.Flip(side * -1);

        if (!canMove)
            return;

        pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove){
            return;
        }

        if (wallGrab)
            return;

        if (!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        isJumping = true;
        //slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        //ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        // particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    // void WallParticle(float vertical)
    // {
    //     var main = slideParticle.main;

    //     if (wallSlide || (wallGrab && vertical < 0))
    //     {
    //         slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
    //         main.startColor = Color.white;
    //     }
    //     else
    //     {
    //         main.startColor = Color.clear;
    //     }
    // }

    // int ParticleSide()
    // {
    //     int particleSide = coll.onRightWall ? 1 : -1;
    //     return particleSide;
    // }
}