# ScriptParasite (dankicode fork)

Fork of arendvw/ScriptParasite. Grasshopper plugin that mirrors Rhino 8 C#/Python script
components to files on disk and syncs edits back. Build: `Grasshopper/ScriptParasite2.csproj`.

## Branches

- `master` mirrors `upstream/master` exactly. Never commit to it; sync with
  `git fetch upstream && git merge --ff-only upstream/master`, then push to origin.
- `feature/ide-stubs` is the daily-driver branch and the one that gets built and installed.
  Rebase it onto `master` after each upstream sync. Conflicts land in
  `Grasshopper/ScriptParasiteComponent.cs` and the csproj.
- Do not open PRs against upstream. The maintainer does not accept AI-generated PRs.

## Build and deploy

- `dotnet build Grasshopper/ScriptParasite2.csproj -c Release` builds all targets and also runs
  a Yak packaging step (harmless warning). Rhino 8 in .NET Core mode loads `net8.0-windows`.
- Deploy is a manual copy, not Yak. With Rhino closed:
  `Copy-Item Grasshopper\bin\Release\net8.0-windows\ScriptParasite2.gha "$env:APPDATA\McNeel\Rhinoceros\packages\8.0\ScriptParasite2\2.1.1.0\net8.0-windows\"`
  (the folder name says 2.1.1.0 from the original Yak install; it does not matter for loading).
- The `Grasshopper` NuGet package version must match the installed Rhino build. The csproj
  references `RhinoCodePluginGH.gha` from the install, and a version mismatch fails with
  CS1705. Check `(Get-Item 'C:\Program Files\Rhino 8\Plug-ins\Grasshopper\Grasshopper.dll').VersionInfo.FileVersion`.
- Versioning: keep upstream's three segments and add a fourth for fork builds (`2.1.2.1`), so
  the Grasshopper plugin list distinguishes a fork build from the stock release.

## Decisions that differ from upstream

- `WriteScriptToComponent` does not call `SetParametersFromScript()`. It only works during the
  component's own solve cycle; parameter changes are made in Grasshopper directly. Upstream
  still calls it. The marshalling snapshot/restore from upstream issue #20 is kept.
- IDE project emission lives in `ProjectHelper.EnsureProjectCsharp`: fork csproj template
  (net8.0-windows, RhinoCodePluginGH reference) with upstream's assembly-path resolution.
  A csproj in the folder or any parent, or a sln in the folder, suppresses emission unless
  forced from the "Regenerate IDE project" context menu.
- Watcher lifecycle: locals are captured before any `await`, async-void handlers are wrapped
  in try/catch, and the document watcher is created per file change and disposed on every
  path. Upstream reuses one watcher and never disposes it.

## Untracked working notes

`ONBOARDING.md`, `PLAN.md`, `REVIEW.md`, `WALKTHROUGH.md` at the repo root are local audit
notes, not committed. Leave them out of `git add`.
