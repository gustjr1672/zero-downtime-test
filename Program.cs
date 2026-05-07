var builder = WebApplication.CreateBuilder(args);

// 서비스에 헬스체크 추가
builder.Services.AddHealthChecks();

var app = builder.Build();

// 헬스체크 엔드포인트 매핑 (/health 경로로 접근 시 Healthy 반환)
app.MapHealthChecks("/health");

// 버전 확인을 위한 간단한 API (Blue/Green 구분용)
app.MapGet("/", () => "Hello! This is Version 10.0 (Blue)");
//app.MapGet("/", () => "Hello! This is Version 8.0 (Green)");

app.Run();