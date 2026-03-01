using UnityEngine.Playables;

public static class CutsceneBlackspace
{
    public static void Play(this PlayableDirector playable, bool needBlackspace)
    {
        if (needBlackspace)
        {
            GameManager.Instance.fadeManager.AddBlackSpace(playable);
        }
        
        playable.Play();
    }
}
