# Iterate
 Command line utility to iterate over files and folders, performing one or more commands on each matching item.

**Syntax**:<br />
<code><pre>iterate /folder:{FolderPath}
    [/filemask:{FileMask}]
    [/recurse:{True|False}]
    [/variables:[{{Var1Name}:{Var1Value}},...,{{VarNName}:{VarNValue}}]]
    /commands:[{Command1},{Command2},...,{Commandn}]
    [/excludedir:[{PathPattern1},...,{PathPatternn}]]
    [/log:{LogFilename}]
    [/wait]
    [/?]</pre></code>

**Where**:<br />


        {FolderPath}    - Pathname of the starting folder.
        {FileMask}      - Mask to match on filenames.
        {LogFilename}   - Filename of the text log to generate.
        {PathPattern}  - Portion of a directory name.

        /log            - Create a log file containing the output of
                          the commands.
        /recurse        - Value indicating whether to recurse folders.
                          Default = true.
        /variables      - JSON array of variables to insert in commands.
        /commands       - JSON string array of individual commands to
                          run in sequence.
        /excludedir     - Patterns in the path to exclude when recursing
                          through folders. Each value can be anything from
                          a portion of a folder name to a complete path.
        /wait           - Wait for user keypress after application finishes.
        /?              - Display this help message.

Each command can contain one or more variable names, enclosed in curly
braces. For example,

        "move {Filename} {Dest}\{RelativeFolderName}\{Filename}"

**Command and variable quote formatting**<br />
The commands and variables arguments support multi-level quoting.
If either of the commands or variables arguments contains spaces, the
 double-quote character is used to surround the entire argument and its
 parameters. This format will be required in most cases.
Single apostrophes are used to surround each command, variable name, and
 variable value.
The caret is used to represent double quotes within individual values.

**Quoting Examples**:<br />

        "/variables:[{'Name':'MyName'},{'OtherName':'Other Value'}]"
        "/commands:['xcopy ^{FromFilename}^ ^{ToFilename}^', 'echo OK']"

**Built-in variables**:<br />
The following variable names are built in.

| Name | Description |
|------|-------------|
| {Extension} | The extension portion of the filename only. |
| {Filename} | The filename and extension, without path. |
| {FilenameOnly} | The portion of the filename without the extension.
| {FullFilename} | The fully qualified path and filename of the current file. |
| {FullFoldername} | The full path name of the current folder. |
| {Index} | The match index of the current file. |
| {RelativeFolderName} | The portion of the current folder name that is offset from the starting folder name. For example, if the starting folder is 'C:\Temp' and the current working folder is 'C:\Temp\bin\files', the relative name is 'bin\files'. |
| {StartFolderName} | The name of the starting folder. |
| {Username} | The username of the user currently logged-in. |


**Examples**:</br>

        iterate /log:C:\Temp\IterateLog.txt "/excludedir:['node_modules','app\views\tools\FontDetailDialog.js']" "/folder:C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendev\pendev" /filemask:*.js /recurse:true "/variables:[{'VarEFExe':'C:\Users\{Username}\Documents\Visual Studio 2015\Projects\EnsureFolder\EnsureFolder\bin\Debug\EnsureFolder.exe'},{'VarCM0Exe':'C:\Users\{Username}\Documents\Visual Studio 2017\Projects\CodeMapper0\CodeMapper0\bin\Debug\CodeMapper0.exe'}]" "/commands:['^{VarEFExe}^ ^C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendoc\{RelativeFolderName}^','^{VarEFExe}^ ^C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendoc\CallSource\{RelativeFolderName}^','esprima-cli ^/input:{FullFilename}^ ^/output:C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendoc\{RelativeFolderName}\{FilenameOnly}-js.json^','^{VarCM0Exe}^ /mode:syntax /format:json ^/input:C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendoc\{RelativeFolderName}\{FilenameOnly}-js.json^ ^/output:C:\Users\{Username}\Documents\GitHub\Scaffold\Docs\Reference\pendoc\CallSource\{RelativeFolderName}\{FilenameOnly}-js.json^']" /wait

