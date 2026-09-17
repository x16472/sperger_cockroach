# sperger_cockroach

《蟲鳴谷：泥沼暴食狩獵》是以 .NET 10 獨立式 Blazor WebAssembly 製作的響應式打地鼠遊戲。主遊戲採 10 × 10 泥沼棋盤、五種難度、三章各 20 秒，另包含角色檔案、尋寶支線與瀏覽器本機成績。

## 遊戲規則

- 普通毛毛蟲 +10、肥嫩毛毛蟲 +20、瓢蟲 +5。
- 藍莓提高倍率，最高 ×4；小老鼠 -15 並清除倍率；木柴扣除本章 5 秒。
- 每章結束達 100 分即結算，否則最多進行三章。
- 成績評級為 SSS、A～B、C～D。

所有內容、權重、速度與文字均集中於 `wwwroot/config.yaml`。圖片放在 `wwwroot/images`，音效由 Web Audio 即時合成。

## 本機執行、偵錯與測試

使用 Visual Studio Code 終端機：

```powershell
dotnet run
dotnet test tests/sperger_cockroach.Tests/sperger_cockroach.Tests.csproj
dotnet publish --configuration Release
```

預設網址為 `http://localhost:5044`。主遊戲成績只在玩家選擇保存後寫入該瀏覽器的 `localStorage`，最多 50 筆，不會上傳至伺服器。

### Visual Studio Code 偵錯

請先安裝 Microsoft C# Dev Kit。專案已提供 `.vscode/launch.json`，請在「執行與偵錯」選擇下列設定：

- `Blazor WASM：啟動並偵錯`：尚未啟動站台時使用，由 VS Code 啟動專案並開啟 Edge 偵錯視窗。
- `Blazor WASM：附加至現有站台`：已透過 `dotnet run` 或 `start.bat` 啟動 `http://localhost:5044` 時使用，避免再次啟動造成連接埠占用。

Blazor WebAssembly 的偵錯代理在頁面啟動後才會就緒，因此 `Program.cs` 與首次載入頁面的 `OnInitialized{Async}` 中斷點不一定會命中。偵錯初始化流程時，先啟動偵錯工作階段，再重新載入頁面；互動事件與後續元件生命週期可直接設定中斷點。

## GitHub Pages

推送至 `main` 分支後，`.github/workflows/deploy-pages.yaml` 會發布靜態 WebAssembly 網站。工作流程會依儲存庫名稱調整 `<base href>`，建立 SPA 用的 `404.html`，並加入 `.nojekyll`。
