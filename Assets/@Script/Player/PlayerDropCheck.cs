using System;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDropCheck
{
    private float _offset = 1.0f;
    private float _sampleRadius = 1.5f;
    private float _minEdgeDistance = 0.35f;
    private int _areaMask = NavMesh.AllAreas;

    private readonly Vector3[] _candidates = new Vector3[8];
    private Transform _playerTransform;

    private const float DIAG = 0.70710678f;

    public PlayerDropCheck(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }

    public bool DropCheck(out Vector3 result)
    {
        var pos = _playerTransform.position;
        var fwd = _playerTransform.forward;
        var right = _playerTransform.right;

 
        _candidates[0] = pos - fwd * _offset;
        _candidates[1] = pos - right * _offset;
        _candidates[2] = pos + right * _offset;
        _candidates[3] = pos + fwd * _offset;

        float d = _offset * DIAG;
        _candidates[4] = pos - fwd * d - right * d;
        _candidates[5] = pos - fwd * d + right * d;
        _candidates[6] = pos + fwd * d - right * d;
        _candidates[7] = pos + fwd * d + right * d;

        //_candidates[0] = _playerTransform.position - _playerTransform.forward * _offset;
        //_candidates[1] = _playerTransform.position - _playerTransform.right * _offset;
        //_candidates[2] = _playerTransform.position + _playerTransform.right * _offset;
        //_candidates[3] = _playerTransform.position + _playerTransform.forward * _offset;

        for (int i = 0; i < _candidates.Length; i++)
        {
            if (!NavMesh.SamplePosition(_candidates[i], out var hit, _sampleRadius, _areaMask))
            {
                continue;
            }

            if (NavMesh.FindClosestEdge(hit.position, out var edge, _areaMask))
            {
                float edgeDist = Vector3.Distance(hit.position, edge.position);
                if (edgeDist < _minEdgeDistance)
                    continue;
            }

            result = hit.position;
            return true;
        }

        result = default;
        return false;
    }
}