ScriptParasite
==============

Two-way sync between Grasshopper script components and external editors. Edit your scripts in VS Code, Visual Studio, Rider, Pycharm instead of the built-in Grasshopper editor.

When enabled, the script parasite component watches the C# component it’s grouped with, and writes the changes to the file [Nickname].cs in the selected folder. By default this is the folder “GrasshopperScripts” in my documents.

#### Installation
- Install using the Rhino 8 package manager

#### Getting started
![Introduction.gif](Grasshopper/docs/Introduction.gif)

- Add the script component to the canvas.
- Drag from the top to connect to the C# script.
- Set enabled to true
- Head over to your My Documents\GrasshopperScripts
- With Visual Studio Code: Open the folder.
- With Visual Studio Community or Rider, open the GrasshopperScripts.csproj file.
- That’s it!

#### Support
For support regarding this plugin, please do not use the comments below, but use the grasshopper and rhino forum at discourse.mcneel.com with the tag scriptparasite.

For bugs, please use the github issue tracker

#### Folders
The folder that is last successfully written to will be saved as the default export folder for all future times you use the component.

If the folder does not contain a .csproj file (used by Visual Studio, Visual Studio code, and all C# ide’s), a new csproj file is created for you with the correct references to Rhino and Grasshopper.

#### Watching for changes
The following changes to the component are synced and written to the C# file:

    Added parameters (input/output)
    Changed parameter name (input/output)
    Changed parameter type and list type (input)

#### IDE’s/Editors


#### Grasshopper for Mac
This plugin is reported to have been working for mac.

#### Source code / Licence / Contributing
The plugin is open source (MIT Licence) and available on Github. If you have suggestions or improvements, throw me an email, or send an issue/pull request on github, and I'll get back to you.

#### Version history
- 2026-03-07, Version 2.1.0: Added project support for python and C#. 
- 2025-11-20, Version 2.0.0: Major overhaul, release of ScriptParasite 2 for python and C# (Rhino 8 only)
- 2021-08-18, Version 1.1.0: Fixed whitespace issues, line numbers are now matching the line numbers in the editor. Solved Visual Studio and Visual Studio Code problems, improved robustness. Dropped Rhino 5 / grasshopper 0.9.x support.
- 2018-12-21, Version 1.0.0: Initial release

#### Credits
- [Andrew Heumann], [Anton Kerezov], [Zac Zhang] for kindly contributing code improvements

#### Contact
For any issues, kindly ask on the discourse forum of McNeel, or open an issue on github.

[Andrew Heumann]:https://github.com/andrewheumann
[Anton Kerezov]:[https://github.com/dilomo]
[Zac Zhang]:[https://github.com/ZacZhangzhuo]
