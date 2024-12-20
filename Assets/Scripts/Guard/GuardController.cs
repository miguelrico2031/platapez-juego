using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GuardController : MonoBehaviour
{
    public AGuardState CurrentState { get; private set; }
    public GuardMovement Movement { get; private set; }
    public Transform SpriteTransform { get; private set; }
    
    [FormerlySerializedAs("_targetRange")] [field: SerializeField] public float TargetRange;
    [FormerlySerializedAs("_targetLayers")] [field: SerializeField] public LayerMask TargetLayers;
    [field: SerializeField] public float LookAroundRotSpeed;
    
    [SerializeField] private int _lastSlotTargetBufferSize = 3;

    
    private SlotMachine[] _lastSlotTargetBuffer;
    private int _bufferIndex = 0;


    private void Awake()
    {
        Movement = GetComponent<GuardMovement>();
        SpriteTransform = transform.GetChild(0);
        ClearBuffer();
    }

    private void Start()
    {
        SetState(new GuardSelectTarget(this));
    }

    private void Update()
    {
        CurrentState?.Update(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, TargetRange);
    }



    public void SetState(AGuardState state)
    {
        var old = CurrentState;
        CurrentState?.Exit(state);
        CurrentState = state;
        CurrentState?.Begin(old);
    }

    public PlayerController GetPlayer() => FindAnyObjectByType<PlayerController>();



    public bool IsInBuffer(SlotMachine slot)
    {
        return _lastSlotTargetBuffer.Contains(slot);
    }

    public void AddToBuffer(SlotMachine slot)
    {
        _lastSlotTargetBuffer[_bufferIndex] = slot;
        _bufferIndex = (_bufferIndex + 1) % _lastSlotTargetBufferSize;
    }

    public void ClearBuffer()
    {
        _bufferIndex = 0;
        _lastSlotTargetBuffer = new SlotMachine[_lastSlotTargetBufferSize];
    }
}