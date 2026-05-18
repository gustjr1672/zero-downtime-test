# ==========================================
# 1. 빌드 환경 (주방 세팅 및 요리)
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 라이브러리(패키지) 복원 - 도커 캐시를 활용해 빌드 속도 향상
COPY ["ZeroDowntimeApi.csproj", "./"]
RUN dotnet restore "./ZeroDowntimeApi.csproj"

# 나머지 모든 소스 코드 복사 및 최종 빌드(Publish)
COPY . .
RUN dotnet publish "ZeroDowntimeApi.csproj" -c Release -o /app/publish

# ==========================================
# 2. 실행 환경 (완성된 요리 플레이팅)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 컨테이너가 8080 포트를 사용한다고 명시 (우리의 블루/그린 포트)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 1번(build) 단계에서 만들어진 결과물(.dll 등)만 쏙 빼서 가져오기
COPY --from=build /app/publish .

# 도커 컨테이너가 켜질 때 실행할 최종 명령어
ENTRYPOINT ["dotnet", "ZeroDowntimeApi.dll"]