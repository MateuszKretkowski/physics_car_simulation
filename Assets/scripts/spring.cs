using UnityEngine;

[CreateAssetMenu(fileName = "SpringSO", menuName = "Physics/SpringSO")]
public class Spring : ScriptableObject
{
    public float mass;
    public float stiffness;
    public float epsilon;
    public float relaxedDistance;
    public float carRelaxedDistance;
}