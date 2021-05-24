setx DB_RUS "localhost:6000"
setx DB_EU "localhost:6001"
setx DB_OTHER "localhost:6002"

cd ..\..\Redis\

start "Original_server" redis-server
start "Russian_server" redis-server --port 6000
start "European_server" redis-server --port 6001
start "Other_countries_server" redis-server --port 6002
