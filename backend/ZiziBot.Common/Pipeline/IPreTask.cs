namespace ZiziBot.Common.Pipeline;

public interface IPreTask
{
    void Execute(PipelineBase pipelineBase, NextAction next);
}