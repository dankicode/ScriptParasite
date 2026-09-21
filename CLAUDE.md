# ScriptParasite (dankicode fork)

Fork of arendvw/ScriptParasite. Grasshopper plugin that mirrors Rhino 8 C#/Python script
components to files on disk and syncs edits back. Build: `Grasshopper/ScriptParasite2.csproj`.

## Branches

- `master` mirrors `upstream/master` exactly. Never commit to it; sync with
  `git fetch upstream && git merge --ff-only upstream/master`, then push to origin.
- `fork/main` is the daily-driver branch and the one that gets built and installed.
  Rebase it onto `master` after each upstream sync. Conflicts land in
  `Grasshopper/ScriptParasiteComponent.cs` and the csproj.
- Do not open PRs against upstream. The maintainer does not accept AI-generated PRs.

## How the sync works (read before touching the watchers)

- Two-way sync with loop prevention via two flags. `ScriptComponentWatcher.IsUpdating` means
  "writing file -> component, ignore component events". `ScriptFilesystemWatcher.IsWriting`
  means "writing component -> file, ignore file events".
- File -> component starts on a threadpool thread (`FileSystemWatcher`) and is marshalled to
  the GH UI thread with `BeginInvoke`. Component -> file stays on the threadpool. The flag
  must be reset inside the UI-thread callback, after the solve, not after `BeginInvoke` returns.
- Both directions are debounced (`DebounceHelper`, 300 ms file side, 500 ms component side).

## Build and deploy

- `dotnet build Grasshopper/ScriptParasite2.csproj -c Release` builds all targets and also runs
  a Yak packaging step (harmless warning). Rhino 8 in .NET Core mode loads `net8.0-windows`.
- Deploy is a manual copy, not Yak. With Rhino closed:
  `Copy-Item Grasshopper\bin\Release\net8.0-windows\ScriptParasite2.gha "$env:APPDATA\McNeel\Rhinoceros\packages\8.0\ScriptParasite2\2.1.1.0\net8.0-windows\"`
  (the folder name says 2.1.1.0 from the original Yak install; it does not matter for loading).
  Do not also drop the gha in `%APPDATA%\Grasshopper\Libraries`; it double-loads.
- The `Grasshopper` NuGet package version must match the installed Rhino build. The csproj
  references `RhinoCodePluginGH.gha` from the install, and a version mismatch fails with
  CS1705. Check `(Get-Item 'C:\Program Files\Rhino 8\Plug-ins\Grasshopper\Grasshopper.dll').VersionInfo.FileVersion`.
- Versioning: keep upstream's three segments and add a fourth for fork builds (`2.1.2.1`), so
  the Grasshopper plugin list distinguishes a fork build from the stock release.
- No automated tests. Test by pointing a component at a scratch folder (e.g. `C:\tmp\sp-stub-test`),
  saving from the editor, and checking the component and the generated csproj / `.vscode` files.

## Decisions that differ from upstream

- `WriteScriptToComponent` does not call `SetParametersFromScript()`. It only works during the
  component's own solve cycle; parameter changes are made in Grasshopper directly. Upstream
  still calls it. The marshalling snapshot/restore from upstream issue #20 is kept.
  Consequence: README.md is upstream's, and its "What gets synced" claims about parameter
  adds/renames flowing from file to component do not hold on this fork. Its package-manager
  install instructions don't apply either.
- `OnFileChanged` captures every field into locals before any `await`, creates the document
  watcher per call and disposes it on every path, and resets `IsUpdating` inside the
  `BeginInvoke` callback. Upstream's version (fix for issue #19) resets the flag in a `finally`
  that runs as soon as `BeginInvoke` is scheduled, so the component watcher sees the in-progress
  write as an edit and writes the file back: that is the "file is newer" prompt on every save.
- All `async void` handlers are wrapped in try/catch. An unhandled exception there takes down Rhino.
- IDE project emission lives in `ProjectHelper.EnsureProjectCsharp`: fork csproj template
  (net8.0-windows, RhinoCodePluginGH reference) with upstream's assembly-path resolution.
  A csproj in the folder or any parent, or a sln in the folder, suppresses emission unless
  forced from the "Regenerate IDE project" context menu.

## Upstream history worth knowing

- The March 2026 leak audit is upstream issues #18 (dankicode) and #19 (arendvw). Arend
  withdrew three findings there: `CancellationTokenSource` disposal (no kernel handle is
  allocated when only `.Token` is used), Bitmap cloning in `Icon` (GH caches icons), and
  nulling managed references in `Dispose`. The fork keeps all three as hygiene. Don't re-audit
  them as leaks and don't strip them out.
- Upstream file names embed the date (`Name-yyMMdd_guid5.cs`), so the watched file name changes
  daily and the old-file cleanup won't match yesterday's file. Left as-is to stay close to upstream.

## Untracked working notes

`ONBOARDING.md` and `WALKTHROUGH.md` at the repo root are local audit
notes, not committed. Leave them out of `git add`.
