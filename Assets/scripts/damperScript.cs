using UnityEngine;

public class damperScript : MonoBehaviour
{
    public Spring spring;

    public GameObject targetPoint;
    public GameObject body;
    
    Rigidbody target_rb;

    [SerializeField] float energyLoss;

    void Start()
    {
        if (targetPoint && spring)
        {
            target_rb = targetPoint.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        CalculateForces();
    }   

    void CalculateForces()
    {
        // distance between two targets
        Vector3 unit = ((transform.position - targetPoint.transform.position) / (transform.position - targetPoint.transform.position).magnitude).normalized;
        float distance = ((transform.position + unit*spring.relaxedDistance - targetPoint.transform.position) / (transform.position + unit * spring.relaxedDistance - targetPoint.transform.position).magnitude).magnitude;
        if (distance >= spring.epsilon) 
        {
            Debug.Log("spring broke.");
            return;
        }
        // F = -kx
        float force = spring.stiffness * distance * energyLoss;
        Debug.Log("force: " + force);
        target_rb.AddForce(unit * force, ForceMode.Impulse);
    }
}
