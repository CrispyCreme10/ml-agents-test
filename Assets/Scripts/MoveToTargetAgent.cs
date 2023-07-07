using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToTargetAgent : Agent
{
    [Header("References")]
    [SerializeField] private Transform platform;
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer background;

    [Header("Display")]
    

    [Header("Bounds")]
    [SerializeField] private float left = -4.25f;
    [SerializeField] private float top = 4.25f;
    [SerializeField] private float right = -0.75f;
    [SerializeField] private float bottom = -4.25f;

    [Header("Modifiers")]
    [SerializeField] private float scale = 1f;
    [SerializeField] private float maxTime = 15f;
    [SerializeField] private float timeElapsed = 0f;

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= maxTime) 
        {
            Fail();
        }
    }

    public override void OnEpisodeBegin()
    {
        timeElapsed = 0;

        transform.localPosition = new Vector3(Random.Range(left * scale, right * scale), Random.Range(bottom * scale, top * scale));
        target.localPosition = new Vector3(Random.Range(-right * scale, -left * scale), Random.Range(bottom * scale, top * scale));

        platform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        transform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation((Vector2)transform.localPosition);
        sensor.AddObservation((Vector2)target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 5f;

        transform.localPosition += new Vector3(moveX, moveY) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Target")
        {
            Win();
        }
        else if (other.tag == "Wall")
        {
            Fail();
        }
    }

    private void Win()
    {
        AddReward(10f);
        background.color = Color.green;
        EndEpisode();
    }

    private void Fail()
    {
        AddReward(-2f);
        background.color = Color.red;
        EndEpisode();
    }
}
