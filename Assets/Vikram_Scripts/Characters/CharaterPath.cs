using UnityEngine;
using UnityEngine.Splines;

public class CharaterPath : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 1f;
    public bool loop = false;

    private float distanceTravelled = 0f;
    private float totalLength;

    void Start()
    {
        if (spline != null)
            totalLength = spline.CalculateLength();
    }

    void Update()
    {
        if (spline == null || totalLength <= 0f)
            return;

        // Move along the spline based on speed
        distanceTravelled += speed * Time.deltaTime;

        if (distanceTravelled > totalLength)
        {
            if (loop)
                distanceTravelled = 0f;
            else
                distanceTravelled = totalLength;
        }

        // Convert distance to normalized value (0–1)
        float t = distanceTravelled / totalLength;

        // Get position on spline
        Vector3 position = spline.EvaluatePosition(t);

        // Get tangent direction (forward direction)
        Vector3 tangent = spline.EvaluateTangent(t);
        Quaternion rotation = Quaternion.LookRotation(tangent, Vector3.up);

        // Apply movement
        transform.SetPositionAndRotation(position, rotation);
    }
}
