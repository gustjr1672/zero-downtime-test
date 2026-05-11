#!/bin/bash

# 1. 현재 Nginx가 바라보고 있는 타겟 확인
IS_GREEN=$(docker ps -q -f name=api-green)

if [ -n "$IS_GREEN" ]; then
    CURRENT_TARGET="api-green"
    NEW_TARGET="api-blue"
    OLD_TARGET="api-green"
    NEW_PORT="8080"
else
    CURRENT_TARGET="api-blue"
    NEW_TARGET="api-green"
    OLD_TARGET="api-blue"
    NEW_PORT="8081"
fi

echo "CURRENT_TARGET=[$CURRENT_TARGET]"
echo " 배포 시작: 새로운 버전($NEW_TARGET)을 준비합니다."

# 2. 이미지 태그 생성 및 환경변수 주입 (Compose가 사용할 수 있도록)
export IMAGE_TAG="v$(date +%s)"

# 3. 새로운 타겟만 백그라운드로 빌드 및 실행 (이때 기존 타겟은 건드리지 않음)
docker compose up -d --build $NEW_TARGET

# 4. 헬스 체크
echo "헬스 체크 진행 중... (http://localhost:$NEW_PORT/health)"
for i in {1..10}
do
    STATUS_CODE=$(curl -o /dev/null -s -w "%{http_code}\n" http://localhost:$NEW_PORT/health)
    if [ "$STATUS_CODE" == "200" ]; then
        echo "헬스 체크 통과!"
        break
    fi
    echo "대기 중... ($i/10)"
    sleep 2
done

echo "도커 네트워크 및 C# 앱 워밍업 대기 중... (3초)"
sleep 3

if [ "$STATUS_CODE" != "200" ]; then
    echo "헬스 체크 실패! 새 컨테이너를 내립니다."
    docker compose stop $NEW_TARGET
    exit 1
fi

# 5. Nginx 스위칭
echo " Nginx 트래픽을 $NEW_TARGET 으로 전환합니다."
sed -i "s/server .*:8080;/server $NEW_TARGET:8080;/g" nginx.conf
sed -i 's/\r//g' nginx.conf

docker cp nginx.conf nginx-proxy:/etc/nginx/nginx.conf

# 문법 검사: Nginx에게 대본에 문제 없는지 먼저 확인받음
docker compose exec -T nginx-proxy nginx -t

# 리로드: 완벽하게 확인된 상태에서 새로고침!
docker compose exec -T nginx-proxy nginx -s reload

echo "Nginx 교대 대기 중... (5초)"
sleep 5

# 6. 구버전 내리기
echo " 트래픽 전환 완료. 구버전($OLD_TARGET)을 종료합니다."
docker compose stop $OLD_TARGET

echo " 무중단 배포($NEW_TARGET)가 완료되었습니다!"