using UnityEngine;
using UnityEngine.UIElements;
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
        carDirection = (transform.position - new Vector3(transform.position.x, car.transform.position.y, transform.position.z)).normalized;
        carDirection.x = 0;
        carDirection.z = 0;
        tireDirection.x = 0;
        tireDirection.z = 0;
        tireDistance = CalculateDistance(transform.position + tireDirection * spring.relaxedDistance, targetPoint.transform.position);
        carDistance = CalculateDistance(transform.position + carDirection * spring.carRelaxedDistance, new Vector3(transform.position.x, car.transform.position.y, transform.position.z));
        Debug.Log("CarDistance: " + carDistance);
        Debug.Log("WIthour CarDistance: " + CalculateDistance(transform.position, new Vector3(transform.position.x, car.transform.position.y, transform.position.z)));
        CalculateForces();
        CalculateSpringBodyContraction();
    }

    float CalculateDistance(Vector3 self, Vector3 target)
    {
        return Vector3.Distance(self, target);
    }

    void CalculateForces()
    {
        Vector3 tireForce = CalculateTireForce();
        Vector3 carForce = CalculateCarForce();

        target_rb.AddForce(tireForce, ForceMode.Impulse);
        car_rb.AddForceAtPosition(carForce, new Vector3(transform.position.x, car.transform.position.y, transform.position.z));
    }

    Vector3 CalculateTireForce()
    {
        if (tireDistance >= spring.epsilon)
        {
            Debug.Log("spring broke.");
            return Vector3.zero;
        }
        float force = spring.stiffness * Mathf.Pow(tireDistance, 2) / 2;
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
        // force = spring.stiffness * Mathf.Pow((distance + distanceToCar), 2);
        Debug.DrawRay(new Vector3(transform.position.x, car.transform.position.y, transform.position.z), carDirection, Color.pink);
        return -carDirection * (carDistance) * force / 2;
    }

    void CalculateSpringBodyContraction()
    {
        Vector3 newScale = new Vector3(springBody.transform.localScale.x, springBody.transform.localScale.y, tireDistance);
        springBody.transform.localScale = newScale;
        springBody.transform.LookAt(targetPoint.transform.position);
    }
}
