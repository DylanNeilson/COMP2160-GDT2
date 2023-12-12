using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject aimGuide;

    [SerializeField] private float followSpeed = 5f;

    private bool isAiming;

    private Transform playerTransform;
    private Transform aimGuideTransform;


    private Vector3 cameraOffset;

    void Start()
	{
        playerTransform = player.transform;
        aimGuideTransform = aimGuide.transform;

        cameraOffset = transform.position - playerTransform.position;

    }

	void Update()
	{
		isAiming = player.isHoldingBall;

        Vector3 targetPosition;

        if (isAiming)
        {
            targetPosition = aimGuideTransform.position + cameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else 
        {
            targetPosition = playerTransform.position + cameraOffset;
            transform.position = targetPosition;
        }
    }

    void ArrowToBall()
    {
        Debug.Log("implement Arrow later");
    }

}