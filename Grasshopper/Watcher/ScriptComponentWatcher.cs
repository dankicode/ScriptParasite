using System;
using Grasshopper.Kernel;
using JetBrains.Annotations;
using RhinoCodePluginGH.Components;
using ScriptParasite.Helper;

namespace ScriptParasite.Watcher;

public class ScriptComponentWatcher : IDisposable
{
    public ScriptComponentWatcher(BaseLanguageComponent component)
    {
        Component = component;
        CurrentScript = GetScript();
        Debouncer = DebounceHelper.Debounce(SendUpdate, 500);

        component.SolutionExpired += SolutionExpired;
        component.ObjectChanged += ObjectChanged;
        component.AttributesChanged += DocumentOnAttributesChanged;
    }

    private void SendUpdate()
    {
        ScriptUpdated?.Invoke(this, EventArgs.Empty);
    }

    private Action Debouncer { get; set; }

    private void DocumentOnAttributesChanged(IGH_DocumentObject sender, GH_AttributesChangedEventArgs e)
    {
        if (IsUpdating)
        {
            return;
        }
        Debouncer?.Invoke();
    }

    private void ObjectChanged(IGH_DocumentObject sender, GH_ObjectChangedEventArgs e)
    {
        if (IsUpdating)
        {
            return;
        }
        Debouncer?.Invoke();
    }

    [UsedImplicitly] public event EventHandler<EventArgs> ScriptUpdated;

    public string CurrentScript { get; set; }

    public string GetScript()
    {
        var result = Component.TryGetSource(out var script);
        if (result)
        {
            return script;
        }
        return null;
    }

    private void SolutionExpired(IGH_DocumentObject sender, GH_SolutionExpiredEventArgs e)
    {
        if (IsUpdating)
        {
            return;
        }
        var script = GetScript();
        // Fix D: was calling GetScript() a second time instead of using the stored variable
        if (CurrentScript == script || script == null) return;

        CurrentScript = script;
        Debouncer?.Invoke();
    }

    public BaseLanguageComponent Component { get; set; }

    private volatile bool _isUpdating;
    public bool IsUpdating { get => _isUpdating; set => _isUpdating = value; }

    public void Dispose()
    {
        if (Component == null)
        {
            return;
        }

        Component.SolutionExpired -= SolutionExpired;
        Component.ObjectChanged -= ObjectChanged;
        Component.AttributesChanged -= DocumentOnAttributesChanged;
        Component = null;
        Debouncer = null;
    }
}
