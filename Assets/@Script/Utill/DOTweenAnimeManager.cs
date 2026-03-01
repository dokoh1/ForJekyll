using System.Collections.Generic;
using DG.Tweening;

public static class DOTweenAnimeManager
{
    static readonly HashSet<string> _exceptIds = new();
    static readonly List<Tween> _pausedTweens = new List<Tween>();

    public static void AddExceptionId(string id)
    {
        if (!string.IsNullOrEmpty(id)) _exceptIds.Add(id);
    }

    public static void PauseRunningTweens()
    {
        _pausedTweens.Clear();

        var playing = DOTween.PlayingTweens();
        if (playing == null || playing.Count == 0) return;

        for (int i = 0; i < playing.Count; i++)
        {
            Tween t = playing[i];
            if (t == null || !t.IsActive() || !t.IsPlaying()) continue;
            if (IsExceptById(t)) continue;
            t.Pause();
            _pausedTweens.Add(t);
        }
    }

    public static void ResumeCapturedTweens()
    {
        for (int i = 0; i < _pausedTweens.Count; i++)
        {
            Tween t = _pausedTweens[i];
            if (t != null && t.IsActive())
                t.Play();
        }

        _pausedTweens.Clear();
    }

    static bool IsExceptById(Tween t)
    {
        var id = t.stringId;
        return !string.IsNullOrEmpty(id) && _exceptIds.Contains(id);
    }
}