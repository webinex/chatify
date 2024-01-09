namespace Webinex.Chatify.Services.Tasks;

internal interface IJob<TTask>
    where TTask : ITask
{
    Task InvokeAsync(TTask task);
}
