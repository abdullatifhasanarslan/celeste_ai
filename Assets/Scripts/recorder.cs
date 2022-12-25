using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class recorder : MonoBehaviour
{
    [Header("gen")]
	public int record_counter=-1;
	public int[,] movement_input;
	public bool recording=true;
	[SerializeField] GameObject ai;
	// [SerializeField] Text text;
	public float[] best_score;
	public int alive=0;
	public int max_population=100;
	public int survivors=5;


	void Awake() {
         Application.targetFrameRate = 30;
     }

    // Start is called before the first frame update
    void Start()
    {
    	movement_input = new int[survivors, 30*5];
    	best_score = new float[survivors];
    	for(int i=0;i<survivors;i++){
    		best_score[i]=10000000;
    	}
    }

    // Update is called once per frames
    void Update()
    {
    	if(recording){
	        record_counter+=1;
	        if(record_counter==150){
	        	recording=false;
	        	return;
	        }
	    
	        bool w=Input.GetKey(KeyCode.W);
	        bool a=Input.GetKey(KeyCode.A);
	        bool s=Input.GetKey(KeyCode.S);
	        bool d=Input.GetKey(KeyCode.D);
	        bool space=Input.GetKey(KeyCode.Space);
	        bool shift=Input.GetKey(KeyCode.LeftShift);
	        bool ctrl=Input.GetKey(KeyCode.LeftControl);
	        int result=0;
	        if(w) result+=64;
	        if(a) result+=32;
	        if(s) result+=16;
	        if(d) result+=8;
	        if(space) result+=4;
	        if(shift) result+=2;
	        if(ctrl) result+=1;
	        for(int i=0;i<survivors;i++){
	        	movement_input[i, record_counter]=result;
	        }
	        // if(space & ctrl & w & a){
		       //  movement_input[record_counter]=1;
	        // }else if(space & ctrl & w & d){
		       //  movement_input[record_counter]=2;
	        // }else if(space & ctrl & s & a){
		       //  movement_input[record_counter]=3;
	        // }else if(space & ctrl & s & d){
		       //  movement_input[record_counter]=4;
	        // }else if(space & ctrl & w){
		       //  movement_input[record_counter]=5;
	        // }else if(space & ctrl & a){
		       //  movement_input[record_counter]=6;
	        // }else if(space & ctrl & s){
		       //  movement_input[record_counter]=7;
	        // }else if(space & ctrl & d){
		       //  movement_input[record_counter]=8;
	        // }else if(space & a){
		       //  movement_input[record_counter]=9;
	        // }else if(space & d){
		       //  movement_input[record_counter]=10;
	        // }else if(space){
		       //  movement_input[record_counter]=11;
	        // }else if(shift & w){
		       //  movement_input[record_counter]=12;
	        // }else if(shift & s){
		       //  movement_input[record_counter]=13;
	        // }else if(shift & a){
		       //  movement_input[record_counter]=27;	//ez solution
	        // }else if(shift & d){
		       //  movement_input[record_counter]=28;	//ez solution
	        // }else if(shift){
		       //  movement_input[record_counter]=14;
	        // }else if(ctrl & w & a){
		       //  movement_input[record_counter]=15;
	        // }else if(ctrl & w & d){
		       //  movement_input[record_counter]=16;
	        // }else if(ctrl & s & a){
		       //  movement_input[record_counter]=17;
	        // }else if(ctrl & s & d){
		       //  movement_input[record_counter]=18;
	        // }else if(ctrl & w){
		       //  movement_input[record_counter]=19;
	        // }else if(ctrl & a){
		       //  movement_input[record_counter]=20;
	        // }else if(ctrl & s){
		       //  movement_input[record_counter]=21;
	        // }else if(ctrl & d){
		       //  movement_input[record_counter]=22;
	        // }else if(w){
		       //  movement_input[record_counter]=23;
	        // }else if(a){
		       //  movement_input[record_counter]=24;
	        // }else if(d){
		       //  movement_input[record_counter]=25;
	        // }else{
		       //  movement_input[record_counter]=26;
	        // }
	    }else{	    
	    	if(alive==0){	
    			// Debug.Log(best_score);
    			for(int i=0;i<max_population;i++){
		    		Instantiate(ai, new Vector2(-8.911f,-3.228f), Quaternion.identity);
		    	}
				Debug.Log(string.Join(",", best_score));
		    	// alive=max_population;
		    }
	    }
    }

    public void evaluate_score(float score, int[] moves){
    	for(int i=0;i<survivors;i++){
    		if(best_score[i]>score){
    			for(int j=survivors-1;j>i;j--){
    				for(int move=0;move<150;move++){
    					movement_input[j, move]=movement_input[j-1, move];
    				}
    				best_score[j]=best_score[j-1];
    			}

				for(int move=0;move<150;move++){
					movement_input[i, move]=moves[move];
				}
				best_score[i]=score;
    			break;
    		}
    	}
    	alive--;
    }
}
