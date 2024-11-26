using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
public class BossAgent : Agent
{
    public bool useVectorObs;
    private Rigidbody m_BossRb;
    private EnvironmentParameters m_ResetParams;
    public override void Initialize()
    {
        m_BossRb = GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if(useVectorObs)
        {
        }
    }
    public override void OnActionReceived(ActionBuffers actions)
    {

    }

    public override void OnEpisodeBegin()
    {

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {

    }

    public void SetResetParameters()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
