// 匯入專案根元件。
using sperger_cockroach.Components;
// 匯入設定讀取服務。
using sperger_cockroach.Services;
// 匯入 Blazor WebAssembly 主機功能。
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

// 建立在瀏覽器中執行的 WebAssembly 主機。
var builder = WebAssemblyHostBuilder.CreateDefault(args);
// 將 App 元件掛載到 wwwroot/index.html 的 app 節點。
builder.RootComponents.Add<App>("#app");
// 建立以網站基底網址為準的 HTTP 用戶端。
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// 註冊從 config.yaml 讀取全域遊戲內容的服務。
builder.Services.AddScoped<GameConfigService>();
// 啟動 WebAssembly 應用程式。
await builder.Build().RunAsync();
