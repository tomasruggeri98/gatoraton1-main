// CatGridChase.cs
using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class CatGridChase : MonoBehaviour
{
    [HideInInspector]
    public Transform target;            // Se asigna desde GameInitializer

    [Tooltip("Segundos que tarda en recorrer una casilla")]
    public float moveDuration = 0.3f;

    // Referencias para animación y flip
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Seeker seeker;
    private Path path;
    private int currentWaypoint;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Lanza la búsqueda de ruta al ratón.
    /// </summary>
    public void BeginChase()
    {
        if (target == null)
        {
            Debug.LogError("CatGridChase: target no asignado.");
            return;
        }
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (p.error)
        {
            Debug.LogWarning("CatGridChase ruta error: " + p.errorLog);
            return;
        }

        path = p;
        currentWaypoint = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        // Activar animación de caminar
        if (animator != null) animator.SetBool("isMoving", true);

        while (currentWaypoint < path.vectorPath.Count)
        {
            Vector3 from = transform.position;
            Vector3 to = path.vectorPath[currentWaypoint];

            // Voltear sprite según la dirección en X
            float dx = to.x - from.x;
            if (spriteRenderer != null && Mathf.Abs(dx) > 0.01f)
                spriteRenderer.flipX = dx < 0f;

            // Mover una celda suavemente
            yield return MoveOneCell(from, to);

            currentWaypoint++;
        }

        // Desactivar animación de caminar
        if (animator != null) animator.SetBool("isMoving", false);

        // Esperar un instante y reiniciar persecución
        yield return new WaitForSeconds(0.5f);
        BeginChase();
    }

    IEnumerator MoveOneCell(Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < moveDuration)
        {
            transform.position = Vector3.Lerp(from, to, t / moveDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }
}
