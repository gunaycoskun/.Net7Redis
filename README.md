docker network create redis-network
docker run -d --name redis-master -p 6379:6379 --network redis-network redis redis-server
 
 
docker run -d --name redis-slave1 -p 6380:6379 --network redis-network redis redis-server --slaveof redis-master 6379
docker run -d --name redis-slave2 -p 6381:6379 --network redis-network redis redis-server --slaveof redis-master 6379
docker run -d --name redis-slave3 -p 6382:6379 --network redis-network redis redis-server --slaveof redis-master 6379

docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' redis-master //redis master ip port verir

//sentinel.conf dosyası
# Sentinel port
port 26379
daemonize yes
logfile "/var/log/redis/sentinel.log"
pidfile "/var/run/redis/sentinel.pid"
 
# Master Redis'i izlemek için yapılandırma
sentinel monitor mymaster 172.18.0.2 6379 2
 
# Master'ın ne kadar sürede "down" olarak kabul edileceği
sentinel down-after-milliseconds mymaster 5000
 
# Master'ın failover zaman aşımı
sentinel failover-timeout mymaster 10000
 
# Paralel senkronizasyon sayısı
sentinel parallel-syncs mymaster 1
 
 
 
docker run -d --name redis-sentinel1 --network redis-network -v "..\sentinel.conf:/etc/redis/sentinel.conf" redis redis-sentinel /etc/redis/sentinel.conf
docker run -d --name redis-sentinel2 --network redis-network -v "..\sentinel.conf:/etc/redis/sentinel.conf" redis redis-sentinel /etc/redis/sentinel.conf
docker run -d --name redis-sentinel3 --network redis-network -v "..\sentinel.conf:/etc/redis/sentinel.conf" redis redis-sentinel /etc/redis/sentinel.conf
 
 
 
docker exec -it redis-sentinel1 redis-cli -p 26379 sentinel masters //slave sayısı 3 olmalı
 
 
docker exec -it redis-master redis-cli SET testkey "some value"
docker exec -it redis-slave1 redis-cli GET testkey
docker exec -it redis-slave2 redis-cli GET testkey
docker exec -it redis-slave3 redis-cli GET testkey
