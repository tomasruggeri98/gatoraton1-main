using UnityEngine;
using Pathfinding;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class CatGridChase : MonoBehaviour
{
    public Transform target;
    public float moveDuration = 0.3f;

    // NUEVO: referencia al Animator y SpriteRenderer
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Seeker seeker;
    private Path path;
    private int currentWaypoint;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        // NUEVO: cach� del Animator y SpriteRenderer
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BeginChase()
    {
        if (target == null) return;
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    IEnumerator FollowPath()
    {
        // NUEVO: activar animaci�n de caminar
        if (animator != null) animator.SetBool("isMoving", true);

        while (currentWaypoint < path.vectorPath.Count)
        {
            Vector3 from = transform.position;
            Vector3 to = path.vectorPath[currentWaypoint];

            // NUEVO: voltear sprite seg�n la direcci�n en X
            float dx = to.x - from.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.01f)
                spriteRenderer.flipX = dx < 0f;

            float t = 0f;
            while (t < moveDuration)
            {
                transform.position = Vector3.Lerp(from, to, t / moveDuration);
                t += Time.deltaTime;
                yield return null;
            }
            transform.position = to;
            currentWaypoint++;
        }

        // NUEVO: desactivar animaci�n de caminar
        if (animator != null) animator.SetBool("isMoving", false);

        yield return new WaitForSeconds(0.5f);
        BeginChase();
    }
}
