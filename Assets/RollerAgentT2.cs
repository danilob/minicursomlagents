using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class RollerAgentT2 : Agent
{
	Rigidbody rBody;
	bool touchingGround;
	const string k_Ground = "k_Ground"; // Tag of ground object.
	public CanvasRenderer textInfo;
	public bool randomVel = false;
	public float speed;
	float speedRand;

	int test = 4;


	// 0: in->9; out->3 --> inserir mais um parâmetro, steps = 0, mudança na posição do target para assumir altura, y, maior que 0.5
	// 1: in->10; out->3 --> inserir a informação se a bola está no chão e adicionar recompensa negativa caso a bola fique muito tempo no ar,
	//    mudando assim o uso do AddReward ao invés de usar o SetReward.
	// 2: mesmo que o 1 mas agora alterando o max steps para 2500 (na interface)
	// 3: mesmo que o 1 mas agora alterando o max steps para 2500 (na interface) e colocando penalização para que ele chegue no objetivo mais rápido e penalização ao cair
    // 4: random velocity, inserção de mais um valor na entrada (in->11; out->3) e marcar na interface se vai usar o randomico ou fixo speed

	void Start () {
		rBody = GetComponent<Rigidbody>();
		speedRand = Random.value*speed;
	}

	public Transform Target;


	public override void OnEpisodeBegin()
	{
		if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
		{
		    // If the Agent fell, zero its momentum
		    this.rBody.angularVelocity = Vector3.zero;
		    this.rBody.velocity = Vector3.zero;
		    this.transform.localPosition = new Vector3( 0, 0.5f, 0);
			speedRand = Random.value*(speed-30.0f) + 30.0f; //minimo 30.0f
		}

		// Move the target to a new spot
		Target.localPosition = new Vector3(Random.value * 8 - 4,
				                   Random.value*2.0f + 0.5f,
				                   Random.value * 8 - 4);
		
		
	}

	public override void CollectObservations(VectorSensor sensor)
	{
	
		if (test == 0){
				// Target and Agent positions
				sensor.AddObservation(Target.localPosition);
	    		sensor.AddObservation(this.transform.localPosition);

	    		// Agent velocity
	    		sensor.AddObservation(rBody.velocity); 
				
		}else{
				// Check ground contact 
				sensor.AddObservation(touchingGround ? 1 : 0);
				// Target and Agent positions
				sensor.AddObservation(Target.localPosition);
	    		sensor.AddObservation(this.transform.localPosition);

	    		// Agent velocity
	    		sensor.AddObservation(rBody.velocity); 
				//consider speed max 
				if(test == 4 & randomVel) 
					sensor.AddObservation(speedRand/speed); 
				if(test == 4 & !randomVel) 
					sensor.AddObservation(speed/250.0f); 
			
		}

	}



	public override void OnActionReceived(float[] vectorAction)
	{
	
		//https://docs.unity3d.com/ScriptReference/Rigidbody.html

	    // Actions, size = 2
		//speed = Random.value*70.0f + 10;

		

	    Vector3 controlSignal = Vector3.zero;
	    controlSignal.x = vectorAction[0];
	    controlSignal.y = vectorAction[1];
		controlSignal.z = vectorAction[2];
		Vector3 force_app = (controlSignal * speed);

		if(test == 4 & randomVel){
			force_app = (controlSignal * speedRand);
		}


	    rBody.AddForce(force_app);

        
	    // Rewards
	    float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

		//if(!touchingGround)
		//	AddReward(-0.005f);

		//AddReward(-0.0001f);
	    // Reached target
	    


		switch (test)
		{
			case 0:{
				// Reached target
				if (distanceToTarget < 1.42f)
	    		{
					SetReward(1.0f);
					EndEpisode();
	    		}

				// Fell off platform
				if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 4)
				{
					EndEpisode();		
				}
				break;
			}
			case 1:{
				if(!touchingGround)
					AddReward(-0.005f);

				if (distanceToTarget < 1.42f)
				{
					AddReward(1.0f);
					EndEpisode();
				}

				// Fell off platform
				if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 4)
				{
					SetReward(-0.1f);
					EndEpisode();		
				}
				break;
			}
			case 2:{
				if(!touchingGround)
					AddReward(-0.005f);

				if (distanceToTarget < 1.42f)
				{
					AddReward(1.0f);
					EndEpisode();
				}

				// Fell off platform
				if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 4)
				{
					//SetReward(-0.1f);
					EndEpisode();		
				}
				break;
			}
			case 3:{
				AddReward(-0.0001f);
				if(!touchingGround)
					AddReward(-0.005f);

				if (distanceToTarget < 1.42f)
				{
					AddReward(1.0f);
					EndEpisode();
				}

				// Fell off platform
				if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 4)
				{
					//SetReward(-0.1f);
					EndEpisode();		
				}
				break;
			}

			case 4:{
				AddReward(-0.0001f);
				if(!touchingGround)
					AddReward(-0.005f);

				if (distanceToTarget < 1.42f)
				{
					AddReward(1.0f);
					EndEpisode();
				}

				// Fell off platform
				if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
				{
					SetReward(-0.5f);
					EndEpisode();		
				}
				break;
			}
			default: return;
		}

		

	}

	void Update() {
	    if(textInfo!=null){
			if(touchingGround)
	        	textInfo.GetComponent<Text>().text = "Is on the ground\n";
			else
				textInfo.GetComponent<Text>().text = "Is in the air\n";
			
			textInfo.GetComponent<Text>().text += "Max Speed: "+speed+"\nVelocity:   "+rBody.velocity.magnitude;
		}	
		//
		//Debug.Log("Vel: "+rBody.velocity.magnitude);
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.transform.CompareTag(k_Ground))
		{
			touchingGround = true;
		}
	}

	void OnCollisionExit(Collision other)
	{
		if (other.transform.CompareTag(k_Ground))
		{
			touchingGround = false;
		}
	}

}
