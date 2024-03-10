using System;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public float FollowSpeed = 4f;
    private Transform target;
    public float followSpeed = 2f;
    private Camera mainCamera;
    private float pixelsToUnits;
    [SerializeField]
    private int aspectRatioHeight = 9;
    [SerializeField]
    private int aspectRatioWidth = 16;
    [SerializeField]
    private int pixelsPerUnit = 16;
    private float inGamePixelsVertical;
    private float inGamePixelsHorizontal;
    private Vector3 gameUnitAccuratePosition;
    [SerializeField]
    private Boolean pixelSnapping;

    void Start()
    {
        gameUnitAccuratePosition = transform.position;
        mainCamera = Camera.main;
        Debug.Log(mainCamera.orthographicSize);
        float heightTiles = (mainCamera.orthographicSize * 2);
        float widthTiles = (heightTiles / aspectRatioHeight * aspectRatioWidth);
        inGamePixelsVertical = heightTiles * pixelsPerUnit;
        inGamePixelsHorizontal = widthTiles * pixelsPerUnit;
    }
    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 newPos = new Vector3(target.position.x, target.position.y, -10f);
            Vector3 newPosSmooth = Vector3.Slerp(gameUnitAccuratePosition, newPos, FollowSpeed * Time.deltaTime);
            gameUnitAccuratePosition = newPosSmooth;
            //update supposed position
            if (pixelSnapping) {
                transform.position = GetNearestPixel(newPosSmooth);
            }
            else
            {
                transform.position = gameUnitAccuratePosition;
            }
        }
    }

    Vector3 GetNearestPixel(Vector3 position)
    {
        // Convert position from world units to pixels
        float pixelX = position.x * pixelsPerUnit;
        float pixelY = position.y * pixelsPerUnit;

        // Calculate the nearest pixel by rounding to the nearest integer pixel
        float nearestPixelX = Mathf.Round(pixelX);
        float nearestPixelY = Mathf.Round(pixelY);

        // Convert the nearest pixel back to world units
        float nearestX = nearestPixelX / pixelsPerUnit;
        float nearestY = nearestPixelY / pixelsPerUnit;

        // Return the nearest pixel position
        return new Vector3(nearestX, nearestY, position.z);
    }

    // Method to set the target dynamically
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}