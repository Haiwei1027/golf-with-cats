using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CatState
{
    IDLE,
    RUN,
    WALK
}

[RequireComponent(typeof(SpriteRenderer))]
public class CatAnimator : MonoBehaviour
{
    private new SpriteRenderer renderer;

    [SerializeField] Sprite[] idle;
    [SerializeField] Sprite[] run;
    [SerializeField] Sprite[] walk;

    [SerializeField] private int[] frameRates;
    [SerializeField] public int catID;

    private CatState targetState = CatState.IDLE;

    private Sprite[][] animations;

    private float previousX;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();

        LoadSprites();

        SetState(CatState.IDLE);
        StartCoroutine(AnimationLoopCoroutine());
    }

    public void LoadSprites()
    {
        idle = Resources.LoadAll<Sprite>($"Sprites/Cat-{catID}-Idle");
        run = Resources.LoadAll<Sprite>($"Sprites/Cat-{catID}-Run");
        walk = Resources.LoadAll<Sprite>($"Sprites/Cat-{catID}-Walk");

        animations = new Sprite[][] { idle, run, walk };
    }

    private void Update()
    {
        if (transform.position.x - previousX > 0.002f)
        {
            renderer.flipX = false;
        }
        else if (transform.position.x - previousX < -0.002f)
        {
            renderer.flipX = true;
        }
        previousX = transform.position.x;
    }

    public void SetState(CatState newState)
    {
        if (targetState == newState) { return; }

        targetState = newState;
    }

    IEnumerator AnimationLoopCoroutine()
    {
        while (true)
        {
            CatState currentState = targetState;
            Sprite[] sprites = animations[(int)currentState];
            WaitForSeconds interframeWait = new WaitForSeconds(1f / frameRates[(int)currentState]);
            int spritePointer = 0;
            while (currentState == targetState)
            {
                renderer.sprite = sprites[spritePointer++];
                spritePointer %= sprites.Length;
                yield return interframeWait;
            }
        }
    }
}