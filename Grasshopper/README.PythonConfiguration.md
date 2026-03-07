# Manual Python Configuration

ScriptParasite auto-configures VS Code for Python components. For other editors like PyCharm, you'll need to set up the paths manually.

## Rhino Python paths

Rhino 8 stores its Python 3 environment in your user profile under `.rhinocode/py39-rh8`. You need to add these paths to your editor for autocomplete to work:

**Windows:**
```
C:\Users\<username>\.rhinocode\py39-rh8\site-stubs
C:\Users\<username>\.rhinocode\py39-rh8\site-rhinopython
C:\Users\<username>\.rhinocode\py39-rh8\site-rhinoghpython
C:\Users\<username>\.rhinocode\py39-rh8\site-envs\default-<id>
```

**macOS:**
```
/Users/<username>/.rhinocode/py39-rh8/site-stubs
/Users/<username>/.rhinocode/py39-rh8/site-rhinopython
/Users/<username>/.rhinocode/py39-rh8/site-rhinoghpython
/Users/<username>/.rhinocode/py39-rh8/site-envs/default-<id>
```

The Python interpreter is at:

- **Windows:** `C:\Users\<username>\.rhinocode\py39-rh8\python.exe`
- **macOS:** `/Users/<username>/.rhinocode/py39-rh8/python`

The `default-<id>` folder has a unique suffix per installation. Check your `site-envs` folder for the actual name.

For more on Rhino's Python environments, see the [Rhino developer documentation on Python Package Environments](https://developer.rhino3d.com/guides/scripting/advanced-pyvenvs/).

## PyCharm

1. Open **Settings** (`Ctrl+Alt+S` / `Cmd+,`)
2. Go to **Project** → **Python Interpreter**
3. Click the interpreter dropdown → **Show All**
4. Select your interpreter, then click the **Show Interpreter Paths** icon in the toolbar (folder icon)
5. Add each of the paths listed above
6. Set the interpreter to Rhino's `python.exe` / `python`

## Other editors

For any editor that supports custom Python paths, add the paths listed above to the editor's Python path configuration and point the interpreter to Rhino's embedded Python 3.