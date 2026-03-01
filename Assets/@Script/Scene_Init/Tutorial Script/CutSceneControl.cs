using System;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneControl : MonoBehaviour
{
    [Serializable]
    public class RendererGroup
    {
        public List<GameObject> renderers;
    }
    
    public List<RendererGroup> AniRenderers;

    public void Start()
    {
        if (AniRenderers == null || AniRenderers.Count == 0 || AniRenderers[0] == null) return;
        
        foreach (var r in AniRenderers)
        {
            foreach (var rr in r.renderers)
            {
                foreach (var rrr in rr.GetComponentsInChildren<Renderer>())
                {
                    rrr.enabled = false;
                }
            }
        }
    }

    public void Situation()
    {
        if (AniRenderers == null || AniRenderers.Count == 0 || AniRenderers[0] == null) return;
        
        foreach (var r in AniRenderers[0].renderers)
        {
            foreach (var rr in r.GetComponentsInChildren<Renderer>())
            {
                rr.enabled = true;
            }
        }
        
        AniRenderers.RemoveAt(0);
    }
}