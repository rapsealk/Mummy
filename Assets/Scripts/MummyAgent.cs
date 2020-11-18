using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MummyAgent : Agent
{
    public Transform m_TargetTransform;
    public GameObject m_Floor;
    public Material m_OrangeMaterial;
    public Material m_RedMaterial;

    private Renderer m_FloorRenderer;
    private Material m_BlueMaterial;

    // User-defined Observations
    private Transform m_Transform;
    private Rigidbody m_Rigidbody;

    // 초기화 작업을 위해 한번 호출
    public override void Initialize()
    {
        m_Transform = GetComponent<Transform>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_FloorRenderer = m_Floor.GetComponent<Renderer>();
        m_BlueMaterial = m_FloorRenderer.material;

        MaxStep = 1000;
    }

    // 에피소드가 시작할 때마다 호출
    public override void OnEpisodeBegin()
    {
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

        m_Transform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.05f, Random.Range(-4.0f, 4.0f));
        m_TargetTransform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), 0.55f, Random.Range(-4.0f, 4.0f));

        StartCoroutine(ResetMaterial());
    }

    // 환경 정보를 관측 및 수집하여 정책 결정을 위해 브레인에 전달
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(m_TargetTransform.localPosition); // 3 (x, y, z)
        sensor.AddObservation(m_Transform.localPosition);       // 3 (x, y, z)
        sensor.AddObservation(m_Rigidbody.velocity.x);          // 1 (x)
        sensor.AddObservation(m_Rigidbody.velocity.z);          // 1 (z)
    }

    // 브레인으로부터 전달받은 행동을 실행
    public override void OnActionReceived(float[] vectorAction)
    {
        // Normalize
        float horizontal = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
        float vertical = Mathf.Clamp(vectorAction[1], -1.0f, 1.0f);
        Vector3 direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
        m_Rigidbody.AddForce(direction.normalized * 50.0f);

        // Reward
        SetReward(-0.001f);
    }

    // 휴리스틱 알고리즘 테스트 혹은 모방학습용
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
        // Debug.Log($"Heuristic [0]={actionsOut[0]} [1]={actionsOut[1]}");
    }

    // MonoBehaviour
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("DeadZone"))
        {
            m_FloorRenderer.material = m_RedMaterial;

            SetReward(-1.0f);
            EndEpisode();
        }
        else if (collision.collider.CompareTag("Target"))
        {
            m_FloorRenderer.material = m_OrangeMaterial;

            SetReward(+1.0f);
            EndEpisode();
        }
    }

    IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.2f);

        m_FloorRenderer.material = m_BlueMaterial;
    }
}
