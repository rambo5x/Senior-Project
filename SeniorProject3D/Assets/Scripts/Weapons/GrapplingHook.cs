using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHook : Weapon
{

    [Header("Attachables")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private CharacterController controller;

    [Header("Positions")]
    [SerializeField] private Transform grapplingHook;
    [SerializeField] private Transform grappleEndpoint;
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform playerBody;

    [Header("Grapple Values")]
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private float maxGrappleDistance;
    [SerializeField] private float hookSpeed;
    [SerializeField] private Vector3 offset;

    private bool isFiring, isGrappling;
    private Vector3 _hookPoint;

    void Start()
    {
        lineRenderer.enabled = false;
    }
    
    void Update()
    {
        if(grapplingHook.parent == handPos)
        {
            grapplingHook.localPosition = new Vector3(0.22f, -0.11f, 0.2f);
            grapplingHook.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        if(Input.GetMouseButtonDown(0))
        {
            FireHook();
        }

        if(isGrappling)
        {
            grapplingHook.position = Vector3.Lerp(grapplingHook.position, _hookPoint, hookSpeed * Time.deltaTime); //launches hook
            if(Vector3.Distance(grapplingHook.position, _hookPoint) < 0.5f)
            {
                controller.enabled = false;
                playerBody.position = Vector3.Lerp(playerBody.position, _hookPoint - offset, hookSpeed * Time.deltaTime);
                    if(Vector3.Distance(playerBody.position, _hookPoint - offset) < 0.5f)
                    {
                        controller.enabled = true;
                        isGrappling = false;
                        grapplingHook.SetParent(handPos);
                        lineRenderer.enabled = false;
                    }
            }
        }
    }

    void LateUpdate()
    {
        if(lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, grappleEndpoint.position);
            lineRenderer.SetPosition(1, handPos.position + new Vector3(0f,-0.11f,0f));
        }
    }

    private void FireHook()
    {
        if(isFiring || isGrappling) return;

        isFiring = true;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, maxGrappleDistance, grappleLayer))
        {
            _hookPoint = hit.point;
            isGrappling = true;
            grapplingHook.parent = null;
            lineRenderer.enabled = true;
            
        }
        isFiring = false;
    }
}
