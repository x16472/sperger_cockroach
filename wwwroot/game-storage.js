// 將本機成績與 Web Audio 功能收斂在固定命名空間。
window.spergerGame = {
    // 讀取成績時容忍使用者手動修改造成的格式錯誤。
    getScores: function () {
        try {
            const value = JSON.parse(window.localStorage.getItem("sperger_cockroach.scores") || "[]");
            return Array.isArray(value) ? value : [];
        } catch {
            return [];
        }
    },

    // 將最新成績放在最前方，並限制最多五十筆。
    saveScore: function (scoreRecord) {
        const scoreRecords = window.spergerGame.getScores();
        scoreRecords.unshift(scoreRecord);
        window.localStorage.setItem("sperger_cockroach.scores", JSON.stringify(scoreRecords.slice(0, 50)));
    },

    // 依互動種類合成短促雙音，避免依賴外部音訊檔案。
    playInteractionSound: function (category) {
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        if (!AudioContext) {
            return;
        }

        window.spergerGame.audioContext ??= new AudioContext();
        const context = window.spergerGame.audioContext;
        if (context.state === "suspended") {
            context.resume();
        }

        const tones = {
            BlueBerry: [880, 1180],
            Mouse: [360, 240],
            PileOfWood: [360, 240],
            Miss: [190, 150],
            Coin: [920, 1320]
        };
        const [firstFrequency, secondFrequency] = tones[category] || [700, 950];
        const startAt = context.currentTime;

        const playTone = function (frequency, startsAt, duration) {
            const oscillator = context.createOscillator();
            const gain = context.createGain();
            oscillator.type = "sine";
            oscillator.frequency.value = frequency;
            gain.gain.setValueAtTime(0.0001, startsAt);
            gain.gain.exponentialRampToValueAtTime(0.12, startsAt + 0.01);
            gain.gain.exponentialRampToValueAtTime(0.0001, startsAt + duration);
            oscillator.connect(gain);
            gain.connect(context.destination);
            oscillator.start(startsAt);
            oscillator.stop(startsAt + duration);
        };

        playTone(firstFrequency, startAt, 0.07);
        playTone(secondFrequency, startAt + 0.11, 0.09);
    }
};

document.addEventListener("DOMContentLoaded", function () {
    const errorUi = document.getElementById("blazor-error-ui");
    const dismissButton = errorUi ? errorUi.querySelector(".dismiss") : null;
    if (errorUi && dismissButton) {
        dismissButton.addEventListener("click", function () {
            errorUi.style.display = "none";
        });
    }
});
