namespace OHunt.Web.Models
{
    public enum RunResult
    {
        Accepted = 1,
        PresentationError = 2,
        WrongAnswer = 3,
        TimeLimitExceed = 4,
        MemoryLimitExceed = 5,
        OutputLimitExceed = 6,
        RuntimeError = 7,
        CompileError = 8,
        UnknownError = 9,
        SubmitError = 10,
    }
}
