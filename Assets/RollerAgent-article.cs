using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RollerAgentT2 : Agent
{
	Rigidbody rBody;
  public Transform Target;
  bool touchingGround;
	const string k_Ground = "k_Ground"; // Tag do objeto designado como chão.
	public float speed;


	void Start () {
		rBody = GetComponent<Rigidbody>();
	}

	public override void OnEpisodeBegin()
	{
		if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
		{
		    // Se o agente cair, o seu momento é zero.
		    this.rBody.angularVelocity = Vector3.zero;
		    this.rBody.velocity = Vector3.zero;
		    this.transform.localPosition = new Vector3( 0, 0.5f, 0);
		}

		// Move o alvo para um novo ponto.
		Target.localPosition = new Vector3(Random.value * 8 - 4,
				                   Random.value*2.0f + 0.5f,
				                   Random.value * 8 - 4);


	}

	public override void CollectObservations(VectorSensor sensor)
	{
    // Verifica o contato com o chão.
    sensor.AddObservation(touchingGround ? 1 : 0);

    // Posições do agente e do alvo
		sensor.AddObservation(Target.localPosition);
	  sensor.AddObservation(this.transform.localPosition);

	  // Velocidade do agente
	  sensor.AddObservation(rBody.velocity);

	}



	public override void OnActionReceived(float[] vectorAction)
	{
	  Vector3 controlSignal = Vector3.zero;
	  controlSignal.x = vectorAction[0];
	  controlSignal.y = vectorAction[1];
	  controlSignal.z = vectorAction[2];

	  rBody.AddForce(controlSignal * speed);


	  // Distância do agente para o alvo
	  float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

		AddReward(-0.0001f);
		if(!touchingGround)
		  AddReward(-0.005f);

		if (distanceToTarget < 1.42f)
			{
				AddReward(1.0f);
				EndEpisode();
			}

		// O agente caiu da plataforma
		if (this.transform.localPosition.y < 0 || this.transform.localPosition.y > 10)
			{
				SetReward(-0.5f);
				EndEpisode();		
			}
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
