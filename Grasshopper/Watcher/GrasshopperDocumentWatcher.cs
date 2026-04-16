using System;
using System.Threading;
using System.Threading.Tasks;
using Grasshopper.Kernel;

namespace ScriptParasite.Watcher;

public class GrasshopperDocumentWatcher : IDisposable
{
    private TaskCompletionSource<bool> _waitCompletion;

    public GrasshopperDocumentWatcher(GH_Document document)
    {
        Document = document;
        document.SolutionStart += DocumentOnSolutionStart;
        document.SolutionEnd += DocumentOnSolutionEnd;
        State = document.SolutionState;
    }

    public GH_ProcessStep State { get; set; }

    private void DocumentOnSolutionEnd(object sender, GH_SolutionEventArgs e)
    {
        State = e.Document.SolutionState;
        _waitCompletion?.TrySetResult(true);
    }

    private void DocumentOnSolutionStart(object sender, GH_SolutionEventArgs e)
    {
        State = e.Document.SolutionState;
    }

    public async Task WaitForSolutionEnd(int timeout)
    {
        if (State == GH_ProcessStep.PostProcess || State == GH_ProcessStep.PreProcess)
            return;

        _waitCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        // Double-check after assigning TCS to close the race window where solution ended
        // between the first state check and TCS creation
        if (State == GH_ProcessStep.PostProcess || State == GH_ProcessStep.PreProcess)
        {
            _waitCompletion.TrySetResult(true);
        }

        using var cts = new CancellationTokenSource(timeout);
        cts.Token.Register(() => _waitCompletion?.TrySetException(new TimeoutException()));

        await _waitCompletion.Task;
    }

    public GH_Document Document { get; set; }

    public void Dispose()
    {
        if (Document == null) return;
        Document.SolutionStart -= DocumentOnSolutionStart;
        Document.SolutionEnd -= DocumentOnSolutionEnd;
        _waitCompletion?.TrySetException(new TimeoutException("Watcher disposed"));
        Document = null;
    }
}
