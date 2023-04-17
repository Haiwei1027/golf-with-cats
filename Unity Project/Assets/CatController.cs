using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CatAnimator))]
public class CatController : MonoBehaviour
{

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float runThreshold;

    private CatAnimator animator;

    private Vector2 destination;

    void Start()
    {
        animator = GetComponent<CatAnimator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.catID = animator.catID++ % 2 + 1;
            animator.LoadSprites();
        }
        float dist = Vector2.Distance(destination, transform.position);
        if (dist > 0.1f)
        {
            float speed;
            if (dist > runThreshold)
            {
                animator.SetState(CatState.RUN);

                speed = runSpeed;
            }
            else
            {
                animator.SetState(CatState.WALK);

                speed = walkSpeed;
            }
            transform.position = transform.position + ((Vector3)destination - transform.position).normalized * speed * Time.deltaTime;
        }
        else
        {
            animator.SetState(CatState.IDLE);
        }
    }
}
