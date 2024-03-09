using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public float FollowSpeed = 2f;
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, -10f);
            transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
        }
    }

    // Method to set the target dynamically
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
