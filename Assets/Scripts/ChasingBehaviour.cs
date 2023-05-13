using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.Serialization;

public class ChasingBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2Int scanningArea;
    [SerializeField] private float nodeSize;
    [SerializeField, Range(0.5f, 3)] public float recreateGraphPeriod;

    [NonSerialized] public AIPath aipath;
    private AstarPath path;
    private float _time;
    private GridGraph _graph;

    private void Start()
    {
        path = GetComponent<AstarPath>();
        aipath = GetComponent<AIPath>();
        _graph = path.data.gridGraph;
    }

    private void FixedUpdate()
    {
        _time += Time.fixedDeltaTime;
        if (_time > recreateGraphPeriod)
        {
            _time = 0;
            CreateNewPath();
        }
    }

    private void CreateNewPath()
    {
        _graph.center = transform.position;
        _graph.SetDimensions(scanningArea.x, scanningArea.y, nodeSize);
        _graph.Scan();
    }
}