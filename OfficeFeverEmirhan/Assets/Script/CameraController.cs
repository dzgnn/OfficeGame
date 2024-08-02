using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMove;
    private Vector3 diff;
    private Vector3 newPos;
    private float lerpVal = 10;

    void Start()
    {
        diff = transform.position - playerMove.transform.position;
    }

    void FixedUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        newPos = Vector3.Lerp(transform.position,
        new Vector3(playerMove.transform.position.x, playerMove.transform.position.y, playerMove.transform.position.z) + diff,
        Time.deltaTime * lerpVal);
        transform.position = newPos;
    }
}
