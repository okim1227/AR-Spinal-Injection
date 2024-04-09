using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPointer : MonoBehaviour
{

    public OVRHand rightHand;
    public GameObject CurrentTarget { get; private set; }

    [SerializeField] private bool showRaycast = true;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LineRenderer lineRenderer;

    private Color _originalColor;
    private Renderer _currentRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckHandPointer(rightHand);
    }

    private void CheckHandPointer(OVRHand hand) {
        if (Physics.Raycast(hand.PointerPose.position, hand.PointerPose.forward, out RaycastHit hit, Mathf.Infinity, targetLayer)) {
            if (CurrentTarget != hit.transform.gameObject) {
                CurrentTarget = hit.transform.gameObject;
                _currentRenderer = CurrentTarget.GetComponent<Renderer>();
                _originalColor = _currentRenderer.material.color;
                _currentRenderer.material.color = highlightColor;
            }
            UpdateRayVisualization(hand.PointerPose.position, hit.point, true);
        } else {
            if (CurrentTarget != null) {
                _currentRenderer.material.color = _originalColor;
                CurrentTarget = null;
            }
            UpdateRayVisualization(hand.PointerPose.position, hand.PointerPose.position + hand.PointerPose.forward * 1000, false);
        }
    }

    private void UpdateRayVisualization(Vector3 start, Vector3 end, bool hitSomething) {
        if (showRaycast && lineRenderer != null) {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.material.color = hitSomething ? Color.green : Color.red;
        }
        else if (lineRenderer != null) {
            lineRenderer.enabled = false;
        }
    }
}
