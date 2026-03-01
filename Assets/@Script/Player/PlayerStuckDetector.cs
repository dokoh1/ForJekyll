using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStuckDetector
{
    private Vector3 _lastPosition;
    private float _stuckTimer;

    private readonly float _stuckDistanceThreshold = 0.5f;
    private readonly float _stuckTimeLimit = 0.12f;

    private Transform _playerTransform;

    public bool IsStuck { get; private set; }

    public PlayerStuckDetector(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _lastPosition = playerTransform.position;
        _stuckTimer = 0f;
        IsStuck = false;
    }

    public void Update(Vector2 inputDir, Transform withNpc)
    {
        bool hasInput = inputDir.sqrMagnitude > 0.01f;
        float moved = (_playerTransform.position - _lastPosition).sqrMagnitude;

        if (hasInput && moved < _stuckDistanceThreshold * _stuckDistanceThreshold && IsInputTowardsNpc(inputDir, withNpc))
        {
            _stuckTimer += Time.deltaTime;

            if (_stuckTimer >= _stuckTimeLimit)
            {
                IsStuck = true;
            }
        }
        else
        {
            _stuckTimer = 0f;
            IsStuck = false;
        }

        _lastPosition = _playerTransform.position;
    }

    private bool IsInputTowardsNpc(Vector2 input, Transform withNpc)
    {
        if (withNpc == null) return false;
        Vector3 inputDir = _playerTransform.forward * input.y + _playerTransform.right * input.x;
        Vector3 toNpc = withNpc.position - _playerTransform.position;
        inputDir.y = 0;
        toNpc.y = 0;

        return Vector3.Angle(inputDir.normalized, toNpc.normalized) < 70f;
    }
}