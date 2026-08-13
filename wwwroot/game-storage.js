// 將遊戲本機儲存功能放在固定命名空間下。
window.spergerGame = {
    // 將新分數附加到此瀏覽器的歷史紀錄。
    saveScore: function (scoreRecord) {
        // 使用專案專屬鍵名避免與其他網站資料衝突。
        const storageKey = "sperger_cockroach.scores";
        // 讀取既有 JSON，沒有資料時使用空陣列。
        const existingJson = window.localStorage.getItem(storageKey) || "[]";
        // 解析既有資料並在格式異常時回復為空陣列。
        let scoreRecords;
        // 捕捉使用者曾手動修改 localStorage 造成的格式錯誤。
        try {
            // 將 JSON 文字還原為陣列。
            scoreRecords = JSON.parse(existingJson);
        } catch {
            // 無效資料不阻止本次分數記錄。
            scoreRecords = [];
        }
        // 將最新結果放到陣列開頭。
        scoreRecords.unshift(scoreRecord);
        // 僅保留最近五十筆以控制瀏覽器儲存量。
        const recentRecords = scoreRecords.slice(0, 50);
        // 將結果寫回此裝置，不進行任何網路傳輸。
        window.localStorage.setItem(storageKey, JSON.stringify(recentRecords));
    }
};

// 文件完成後連接全域錯誤列的關閉按鈕。
document.addEventListener("DOMContentLoaded", function () {
    // 取得 Blazor 錯誤列與關閉按鈕。
    const errorUi = document.getElementById("blazor-error-ui");
    const dismissButton = errorUi ? errorUi.querySelector(".dismiss") : null;
    // 只有兩個節點都存在時才註冊事件。
    if (errorUi && dismissButton) {
        // 點擊後只隱藏錯誤列，不修改遊戲資料。
        dismissButton.addEventListener("click", function () {
            // 將錯誤列恢復為隱藏狀態。
            errorUi.style.display = "none";
        });
    }
});
