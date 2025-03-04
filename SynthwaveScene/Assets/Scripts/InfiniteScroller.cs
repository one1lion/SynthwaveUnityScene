using System;

using Unity.VisualScripting;

using UnityEngine;

// This is from a super fast run through example from clayman_dev (https://twitch.tv/clayman_dev)
//   who worked through this example in a live stream
public class InfiniteScroller : MonoBehaviour
{
    private const float _groundSpeedScale = -30.96f;

    // Define the mountain prefab
    public GameObject MountainPrefab;
    // The ground object
    public GameObject Ground;
    // Distance of each chunk
    public float Distance = 500;
    // How fast should the chunks move
    public float Speed = 100;
    // When do we reset the chunks (how far behind the camera)
    public float ResetThresold = -1675;
    // How many chunks to hold in memory, 4 is needed at minium
    public int MountainChunks = 6;

    // The offset of the mountain prefab
    public float MountainXOffset = -246.29f;
    public float MountainYOffset = 0;

    // The array of mountain chunks
    private GameObject[] _mountainArray;
    // The ground object's material
    private Material _groundMaterial;
    // The previous speed (only update if it changes)
    private float _prevSpeed;

    void Start()
    {
        // Get the reference to the ground's material
        _groundMaterial = Ground.GetComponent<Renderer>().material;
        UpdateSpeedIfNeeded();

        // Initialize an array of mountain chunks
        _mountainArray = new GameObject[MountainChunks];
        for (int i = 0; i < MountainChunks; i++)
        {
            // Spawn chunks at their positions, important part is distance * index
            _mountainArray[i] = Instantiate(MountainPrefab, transform.position + new Vector3(MountainXOffset, MountainYOffset, Distance * i), MountainPrefab.transform.rotation);
        }
    }

    void Update()
    {
        // Update the ground object's material's GridShader ShaderGraph's _Offset_Movement_Speed so that it moves relative to the set Speed where the _Offset_Movement_Speed is -3.23 when Speed is 100
        UpdateSpeedIfNeeded();

        // Move all chunks at speed
        for (int i = 0; i < _mountainArray.Length; i++)
        {
            var obj = _mountainArray[i].transform;
            // Set all of the x and y positions to the same value (in case the MountainXOffset is changed)
            obj.position = new Vector3(MountainXOffset, MountainYOffset, obj.position.z);

            obj.position -= new Vector3(0, 0, Speed * Time.deltaTime);
            if (obj.position.z < ResetThresold)
            {
                // If z position exceed reset threshold, move back to start
                obj.transform.position = transform.position + new Vector3(0, 0, Distance * _mountainArray.Length);
            }
        }
    }

    private void UpdateSpeedIfNeeded()
    {
        if (_prevSpeed == Speed)
        {
            return;
        }

        _prevSpeed = Speed;

        // Calculate the Y value for the Vector2.
        float yValue = _prevSpeed / _groundSpeedScale;

        // Get the current Vector2 value from the material.
        Vector2 currentVector = _groundMaterial.GetVector("_Offset_Movement_Speed");

        // Create a new Vector2 with the updated Y value.
        var newVector = new Vector2(currentVector.x, yValue);

        // Update the material's Vector2 property.
        _groundMaterial.SetVector("_Offset_Movement_Speed", newVector);
    }
}
