using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public enum PartType { Head, Chest, Arms, Legs }

    [Header("Settings")]
    public PartType partType = PartType.Chest;

    public float DamageMultiplier => partType switch
    {
        PartType.Head  => 4.0f,
        PartType.Chest => 1.0f,
        PartType.Arms  => 0.75f,
        PartType.Legs  => 0.5f,
        _ => 1.0f
    };
}
