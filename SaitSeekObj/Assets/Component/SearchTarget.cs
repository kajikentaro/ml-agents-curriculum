using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class SearchTarget: Agent
{
    Rigidbody rBody;
    RaycastHit raycastHit;
    Renderer myrenderer;
    public Transform Target;
    public override void Initialize(){
        rBody=GetComponent<Rigidbody>();
        myrenderer = GetComponent<Renderer>();
        myrenderer .material.color = Color.blue;
    }
    private bool continueThisStage()
    {
        int nextStageNumber = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("stage_number", -1.0f);
        if(nextStageNumber != MainMenu.nowStage)
        {
            MainMenu.aliveCount--;
            MainMenu.nextStage = nextStageNumber;
            DestroyImmediate(GetComponent<DecisionRequester>());
            DestroyImmediate(this.gameObject);
            //Academy academy = Academy.Instance;
            //academy.EnvironmentStep();
            //EpisodeInterrupted();
            return false;
        }
        else
        {
            return true;
        }
    }
    public float floorSize = 10f;
    public override void OnEpisodeBegin(){
        if (continueThisStage())
        {
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3( 4.0f, 0.75f, 0.0f);
            Vector3 targetNewPosition;
            do
            {
                targetNewPosition = new Vector3(Random.value * (floorSize - 1f) - (floorSize / 2), 1, Random.value * (floorSize - 1f) - (floorSize / 2));
            }
            while (Vector3.Distance(targetNewPosition, this.transform.localPosition) < (this.transform.localScale.x + Target.localScale.x) );
            Target.localPosition = targetNewPosition;
            startTime = Time.time;
            myrenderer .material.color = Color.blue;
        }
    }
    public override void CollectObservations(VectorSensor sensor){
        //sensor.AddObservation(this.transform.rotation.y);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(rBody.angularVelocity.y);
    }
    public float forceMultiplier = 4;
    float lookingTime = 0;
    float startTime = 0;
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 maxSpeed = new Vector3(5.0f, 0f, 5.0f);
        Vector3 minSpeed = new Vector3(-5.0f, 0f, -5.0f);

        Vector3 input = new Vector3(actionBuffers.ContinuousActions[2], 0, actionBuffers.ContinuousActions[1]);
        Vector3 frontForce = Quaternion.Euler(0, this.transform.eulerAngles.y, 0) * input * forceMultiplier;
        Vector3 newForce = frontForce + rBody.velocity;
        rBody.velocity = Vector3.Min(Vector3.Max(minSpeed, newForce), maxSpeed);

        rBody.angularVelocity = new Vector3(0, actionBuffers.ContinuousActions[0] * 20, 0);

        Physics.Raycast(this.transform.position, this.transform.forward, out raycastHit, 5);
        if(raycastHit.collider == null || raycastHit.collider.name != "Target")
        {
            myrenderer .material.color = Color.blue;
            lookingTime = Time.time;
            if(Time.time - startTime>= 30)
            {
                EndEpisode();
            }
        }
        else
        {
            myrenderer .material.color=Color.red;
            if(Time.time - lookingTime>= 3.0f)
            {
                SetReward(1.0f);
                EndEpisode();
            }
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.J))
        {
            continuousActionsOut[2] = -1;
        }
        if (Input.GetKey(KeyCode.K))
        {
            continuousActionsOut[2] = 1;
        }
    }
}
