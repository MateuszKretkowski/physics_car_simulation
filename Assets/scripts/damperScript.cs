using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class damperScript : MonoBehaviour
{
    public Spring spring;

    public GameObject targetPoint;
    public GameObject car;
    public GameObject springBody;

    [SerializeField] Rigidbody target_rb;
    [SerializeField] Rigidbody car_rb;

    [SerializeField] float energyLoss;
    [SerializeField] float tireOffset;

    [SerializeField] float tireDistance;
    [SerializeField] float carDistance;
    [SerializeField] Vector3 tireDirection;
    [SerializeField] Vector3 carDirection;

    void Start()
    {
        if (!car)
        {
            car = transform.root.gameObject;
        }
        if (targetPoint && spring && car)
        {
            targetPoint = Instantiate(targetPoint, new Vector3(transform.position.x, transform.position.y + tireOffset, transform.position.z), Quaternion.identity);
            target_rb = targetPoint.GetComponent<Rigidbody>();
            car_rb = car.GetComponent<Rigidbody>();
        }
    }

    void Update()
    {
        tireDirection = (transform.position - targetPoint.transform.position).normalized;
        carDirection = Vector3.up;
        tireDistance = CalculateDistance(transform.position + tireDirection * spring.relaxedDistance, targetPoint);
        carDistance = CalculateDistance(transform.position + carDirection * spring.relaxedDistance, car);
        CalculateForces();
        CalculateSpringBodyContraction();
    }

    float CalculateDistance(Vector3 self, GameObject target)
    {
        return Vector3.Distance(self, target.transform.position);
    }

    void CalculateForces()
    {
        Vector3 tireForce = CalculateTireForce();
        Vector3 carForce = CalculateCarForce();

        target_rb.AddForce(tireForce, ForceMode.Impulse);
        car_rb.AddForceAtPosition(carForce, transform.position);
    }

    Vector3 CalculateTireForce()
    {
        if (tireDistance >= spring.epsilon)
        {
            Debug.Log("spring broke.");
            return Vector3.zero;
        }
        float force = spring.stiffness * Mathf.Pow(tireDistance, 2) / 2;
        Debug.Log("force: " + force);
        Debug.DrawRay(targetPoint.transform.position, tireDirection, Color.red);
        return tireDirection * (tireDistance) * force / 2;
    }

    Vector3 CalculateCarForce()
    {
        if (carDistance >= spring.epsilon)
        {
            Debug.Log("spring broke: car too far.");
            return Vector3.zero;
        }
        if (!car_rb || !car)
        {
            return Vector3.zero;
        }
        float force = spring.stiffness * Mathf.Pow(carDistance, 2) / 2;
        float distanceToCar = CalculateDistance(transform.position, car);
        carDirection.x *= 0f;
        carDirection.z *= 0f;
        // force = spring.stiffness * Mathf.Pow((distance + distanceToCar), 2);
        Debug.DrawRay(transform.position, -carDirection, Color.pink);
        return -carDirection * (carDistance) * force / 2;
    }

    void CalculateSpringBodyContraction()
    {
        Vector3 newScale = new Vector3(springBody.transform.localScale.x, springBody.transform.localScale.y, tireDistance);
        springBody.transform.localScale = newScale;
        springBody.transform.LookAt(targetPoint.transform.position);
    }
}
