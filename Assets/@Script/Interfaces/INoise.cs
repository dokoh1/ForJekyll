using UnityEngine;

public interface INoise
{
    public float CurNoiseAmount { get; set; }
    
    public float NoiseCheckAmount { get; set; }
    public float SumNoiseAmount { get; set; }
    public float DecreaseSpeed { get; set; }
    public float MaxNoiseAmount { get; set; }
}
