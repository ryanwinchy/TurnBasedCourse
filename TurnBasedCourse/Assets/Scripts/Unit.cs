using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] Animator unitAnimator;
    Vector3 targetPosition;

    private void Awake()
    {
        targetPosition = transform.position;  //By default, unit does not move.
    }

    private void Update()
    {

        float stoppingDistance = 0.1f;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)   //When have a gap bigger than stopping distance (this prevents twitching when gets there)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;     //Gives us direction and magnitude (distance between two points), we only need direction so can use normalize.

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;     //Makes framerate independent. Otherwise will move faster if higher frame rate cos update runs more (it runs once per frame).

            float rotateSpeed = 10f;  //So rotates faster.
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);      //Make char face where he is going.
                                                               //Lerp is applying smoothing between current transform.forward and moveDirection, the end result. Will move towards it each frame smoothly. Will move most at start, as transform is updating , so gap smaller and smaller, looks nice. For linear, would just store original transform.
            unitAnimator.SetBool("Moving", true);
        }
        else
            unitAnimator.SetBool("Moving", false);




    }
    public void SetTargetPosition(Vector3 _targetPosition)
    {
        targetPosition = _targetPosition;
    }
}
