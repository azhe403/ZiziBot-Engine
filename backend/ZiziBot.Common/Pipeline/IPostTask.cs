namespace ZiziBot.Common.Pipeline;

public interface IPostTask
{
    void Execute(PipelineBase pipelineBase);
}