// CatGridChase.cs
using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class CatGridChase : MonoBehaviour
{
    [HideInInspector]
    public Transform target;               // Se asigna desde GameInitializer

    [Tooltip("Segundos que tarda en recorrer una casilla")]
    public float moveDuration = 0.3f;

    private Seeker seeker;
    private Path path;
    private int currentWaypoint;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
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
        while (currentWaypoint < path.vectorPath.Count)
        {
            Vector3 next = path.vectorPath[currentWaypoint];
            yield return StartCoroutine(MoveOneCell(transform.position, next));
            currentWaypoint++;
        }
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
