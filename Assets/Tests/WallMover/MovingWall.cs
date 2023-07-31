using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class MovingWall : MonoBehaviour {
    [SerializeField] Vector3 Offset;
    [SerializeField] float Speed = 1;
    [SerializeField] float pauseDuration = 3f;

    private Vector3 p0;
    private Vector3 p1;

    // TODO: Handle RotationDelta as well?
    public Vector3 MotionDelta;
    public Vector3 PreviousMotionDelta;

    private Vector3 targetPosition;
    private bool movingTowardsP1;

    private void Start() {
        p0 = transform.position;
        p1 = transform.TransformPoint(Offset);
        movingTowardsP1 = true;
        targetPosition = p1;
        StartCoroutine(MoveObject());
    }

    private void FixedUpdate() {
        Vector3 previousPosition = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.fixedDeltaTime);
        PreviousMotionDelta = MotionDelta;
        MotionDelta = transform.position - previousPosition;
    }

    private IEnumerator MoveObject() {
        while (true) {
            yield return new WaitForSeconds(pauseDuration);
            movingTowardsP1 = !movingTowardsP1;
            targetPosition = movingTowardsP1 ? p1 : p0;
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f) {
                yield return null;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.TransformPoint(Offset));
    }
}