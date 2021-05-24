
start /d ..\Valuator\ dotnet run --no-build --urls "http://localhost:5001" 
start /d ..\Valuator\ dotnet run --no-build  --urls "http://localhost:5002"

start "RankCalculator1" /d ..\RankCalculator\ dotnet run --no-build
start "RankCalculator2" /d ..\RankCalculator\ dotnet run --no-build

start /d ..\..\ nginx.exe 