xcopy c:lib/System.Data.SQLite.dll d:bin/
csc /out:bin/Bot.exe /recurse:src\*.cs /r:System.Data.dll /r:lib/System.Data.SQLite.dll