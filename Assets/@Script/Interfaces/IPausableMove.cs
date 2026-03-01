public interface IPausableMove
{
    float PrevSpeed { get; set; }
    bool WasStopped { get; set; }

    void PauseMove();
    void ResumeMove();
}