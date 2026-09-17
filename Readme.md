# sperger_cockroach

《蟲鳴谷：泥沼暴食狩獵》是以 .NET 10 獨立式 Blazor WebAssembly 製作的響應式打地鼠遊戲。主遊戲採 10 × 10 泥沼棋盤、五種難度、三章各 20 秒，另包含角色檔案、尋寶支線與瀏覽器本機成績。

## 遊戲規則

- 普通毛毛蟲 +10、肥嫩毛毛蟲 +20、瓢蟲 +5。
- 藍莓提高倍率，最高 ×4；小老鼠 -15 並清除倍率；木柴扣除本章 5 秒。
- 每章結束達 100 分即結算，否則最多進行三章。
- 成績評級為 SSS、A～B、C～D。

所有內容、權重、速度與文字均集中於 `wwwroot/config.yaml`。圖片放在 `wwwroot/images`，音效由 Web Audio 即時合成。

## 本機執行與測試

使用 Visual Studio Code 終端機：

```powershell
dotnet run
dotnet test tests/sperger_cockroach.Tests/sperger_cockroach.Tests.csproj
dotnet publish --configuration Release
```

預設網址為 `http://localhost:5044`。主遊戲成績只在玩家選擇保存後寫入該瀏覽器的 `localStorage`，最多 50 筆，不會上傳至伺服器。

## GitHub Pages

推送至 `main` 分支後，`.github/workflows/deploy-pages.yaml` 會發布靜態 WebAssembly 網站。工作流程會依儲存庫名稱調整 `<base href>`，建立 SPA 用的 `404.html`，並加入 `.nojekyll`。
