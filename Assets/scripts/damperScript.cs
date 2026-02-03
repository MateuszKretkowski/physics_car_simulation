using UnityEngine;

public class damperScript : MonoBehaviour
{
    public Spring spring;

    public GameObject targetPoint;
    public GameObject springBody;
    
    Rigidbody target_rb;

    [SerializeField] float energyLoss;
    [SerializeField] float distance;
    [SerializeField] Vector3 direction;

    void Start()
    {
        if (targetPoint && spring)
        {
            target_rb = targetPoint.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        CalculateDistance();
        CalculateForces();
        CalculateSpringBodyContraction();
    }   

    void CalculateDistance()
    {
        direction = ((transform.position - targetPoint.transform.position) / (transform.position - targetPoint.transform.position).magnitude).normalized;
        distance = Vector3.Distance(transform.position + direction * spring.relaxedDistance, targetPoint.transform.position);
    }

    void CalculateForces()
    {
        // distance between two targets
        if (distance >= spring.epsilon) 
        {
            Debug.Log("spring broke.");
            return;
        }
        // F = -kx
        float force = spring.stiffness * Mathf.Pow(distance, 2) / 2;
        Debug.Log("force: " + force);
        Debug.DrawRay(targetPoint.transform.position, direction, Color.red);
        target_rb.AddForce(direction * distance * force * energyLoss, ForceMode.Impulse);
    }

    void CalculateSpringBodyContraction()
    {
        Vector3 newScale = new Vector3(springBody.transform.localScale.x, springBody.transform.localScale.y, distance);
        springBody.transform.localScale = newScale;
        springBody.transform.LookAt(targetPoint.transform.position);
    }
}
