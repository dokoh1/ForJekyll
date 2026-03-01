
    public interface ILog
    {
        void CreateLog(DialogueView view);

        bool CreateLogJump(DialogueView view, out int index);
    }