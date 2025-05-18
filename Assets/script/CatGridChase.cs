// CatGridChase.cs
using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class CatGridChase : MonoBehaviour
{
    [Tooltip("Transform del ratón.")]
    public Transform target;
    public float moveDuration = 0.3f;

    private Seeker seeker;
    private Path path;
    private int currentWaypoint;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Inicia la persecución: calcula la ruta una única vez.
    /// </summary>
    public void BeginChase()
    {
        if (target == null) return;

        // Activa la animación de movimiento
        if (animator != null) animator.SetBool("isMoving", true);

        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error) return;
        path = p;
        currentWaypoint = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        // Recorre todos los waypoints una vez
        while (currentWaypoint < path.vectorPath.Count)
        {
            Vector3 from = transform.position;
            Vector3 to = path.vectorPath[currentWaypoint];

            // Voltear sprite según dirección X
            float dx = to.x - from.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.01f)
                spriteRenderer.flipX = dx < 0f;

            // Interpolación suave
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

        // Al completar la ruta, posicionarse exactamente sobre el target
        if (target != null)
            transform.position = target.position;

        // Detener animación de movimiento
        if (animator != null) animator.SetBool("isMoving", false);

        // NO reiniciamos BeginChase ni el bucle: terminamos aquí
        yield break;
    }
}
