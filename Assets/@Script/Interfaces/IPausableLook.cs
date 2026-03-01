using UnityEngine;

public interface IPausableLook
{
    Transform FreezeAnchor { get; set; }
    bool IsFrozen { get; set; }
    float CachedWeight { get; set; }

    void PauseLook();
    void ResumeLook();
}