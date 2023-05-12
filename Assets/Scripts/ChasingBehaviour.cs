using System;
using Pathfinding;
using UnityEngine;

public class ChasingBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2Int scanningArea;
    [SerializeField] private float nodeSize;
    [SerializeField, Range(0.5f, 3)] public float recreateGraphPeriod;

    private AstarPath _path;
    private float _time;
    private GridGraph _graph;
    [NonSerialized] public Seeker Seeker;

    private void Start()
    {
        _path = GetComponent<AstarPath>();
        _graph = _path.data.gridGraph;
        Seeker = GetComponent<Seeker>();
    }

    private void FixedUpdate()
    {
        // _time += Time.fixedDeltaTime;
        // if (_time > recreateGraphPeriod)
        // {
        //     _time = 0;
        //     CreateNewPath();
        // }
    }

    public void CreateNewPath()
    {
        _graph.center = transform.position;
        _graph.SetDimensions(scanningArea.x, scanningArea.y, nodeSize);
        _graph.Scan();
    }
}