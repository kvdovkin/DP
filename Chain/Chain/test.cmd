
start "1" dotnet run --no-build 8000 localhost 8001 true
start "2" dotnet run --no-build 8001 localhost 8002
start "3" dotnet run --no-build 8002 localhost 8003
start "4" dotnet run --no-build 8003 localhost 8004
start "5" dotnet run --no-build 8004 localhost 8005
start "6" dotnet run --no-build 8005 localhost 8000