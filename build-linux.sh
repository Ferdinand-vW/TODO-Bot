cp lib/System.Data.SQLite.dll bin/
mcs /out:bin/Bot.exe src/* -r:System.Data.dll -r:lib/System.Data.SQLite.dll