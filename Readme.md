# sperger_cockroach

《蟲鳴谷：泥沼暴食狩獵》是以 .NET 10 獨立式 Blazor WebAssembly 製作的互動式文字冒險與打地鼠遊戲。

## 內容設定

網站標題、開場介紹、章節劇情、獵場布局、得分規則、標靶權重與結算評語皆集中於 `wwwroot/config.yaml`，修改後會隨 Git 一併版控。

## 本機執行

使用 Visual Studio Code 終端機執行：

```powershell
dotnet run
```

預設網址為 `http://localhost:5044`。

## GitHub Pages

推送至 `main` 分支後，`.github/workflows/deploy-pages.yaml` 會發布 `bin/Release` 產生的靜態 WebAssembly 網站。請在 GitHub 儲存庫的 Pages 設定中將來源選為 GitHub Actions。

玩家選擇記錄分數時，資料只保存在該瀏覽器的 `localStorage`，不會上傳至伺服器。
