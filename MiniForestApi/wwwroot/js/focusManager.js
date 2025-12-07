// js/focusManager.js

// Oturum başlatma
async function startSession() {
    // Diğer dosyalarda tanımlı global değişkenlere erişiyoruz
    const input = document.getElementById("durationInput");
    const value = input.value.trim(); 
    
    if (value === "" || value === null) {
        alert("Lütfen oturum süresi giriniz.");
        return;
    }

    const minutes = parseInt(value, 10);
    
    if (isNaN(minutes) || minutes <= 0) {
         alert("Lütfen 0'dan büyük bir süre giriniz.");
         return;
    }

    try {
        const response = await fetch(FOCUS_URL + "/start", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ durationMinutes: minutes })
        });

        const data = await handleApiResponse(response);
        
        if (!data.success) {
            alert("Hata: " + data.message);
            return;
        }

        const session = data.body;
        window.currentSessionId = session.id;
        document.getElementById("currentSessionId").textContent = window.currentSessionId;

        startTimer(minutes * 60);
        loadSessions();
        loadToday();
        updateGoalProgress(); // GoalManager'dan çağrılır
    } catch (err) {
        alert("İstek sırasında bağlantı hatası oluştu: " + err);
    }
}

// Oturumu bitirme
async function finishSession(auto = false) {
    if (window.currentSessionId === null) {
        if (!auto) alert("Aktif oturum yok.");
        return;
    }

    try {
        const response = await fetch(FOCUS_URL + "/finish/" + window.currentSessionId, {
            method: "POST"
        });
        
        const data = await handleApiResponse(response);

        if (!data.success) {
            if (!auto) alert("Hata: " + data.message);
            return;
        }

        clearInterval(window.timerInterval);
        window.remainingSeconds = 0;
        updateTimerDisplay();
        window.currentSessionId = null;
        document.getElementById("currentSessionId").textContent = "Yok";

        if (!auto) {
            alert("Oturum tamamlandı.");
        }

        loadSessions();
        loadToday();
        updateGoalProgress(); // GoalManager'dan çağrılır
    } catch (err) {
        if (!auto) alert("İstek sırasında bağlantı hatası oluştu: " + err);
    }
}

// Geri sayım başlatma
function startTimer(seconds) {
    clearInterval(window.timerInterval);
    window.remainingSeconds = seconds;
    updateTimerDisplay();

    window.timerInterval = setInterval(() => {
        window.remainingSeconds--;
        updateTimerDisplay();

        if (window.remainingSeconds <= 0) {
            clearInterval(window.timerInterval);
            if (window.currentSessionId !== null) {
                finishSession(true);
            }
        }
    }, 1000);
}

// Geri sayım ekranını güncelleme
function updateTimerDisplay() {
    const display = document.getElementById("timerDisplay");
    if (window.remainingSeconds <= 0) {
        display.textContent = "Süre bitti";
    } else {
        const m = Math.floor(window.remainingSeconds / 60);
        const s = window.remainingSeconds % 60;
        display.textContent = m + " dk " + s.toString().padStart(2, "0") + " sn";
    }
}

// Bugünkü özeti yükleme
async function loadToday() {
    try {
        const response = await fetch(FOCUS_URL + "/today");
        const data = await handleApiResponse(response);

        if (!data.success) {
            document.getElementById("todaySummary").textContent = "Hata: " + data.message;
            return;
        }

        const body = data.body;
        const date = new Date(body.date).toLocaleDateString('tr-TR'); 

        document.getElementById("todaySummary").textContent =
            date + " tarihinde toplam " + 
            body.totalMinutes + " dakika odaklandınız.";
    } catch (err) {
        document.getElementById("todaySummary").textContent =
            "İstek sırasında bağlantı hatası oluştu: " + err;
    }
}

// Tüm oturumları listeleme
async function loadSessions() {
    try {
        const response = await fetch(FOCUS_URL); 
        const data = await handleApiResponse(response);

        if (!data.success) {
            alert("Hata: " + data.message);
            return;
        }

        const list = document.getElementById("sessionsList");
        list.innerHTML = "";

        const sessions = data.body;
        // Listeyi Bootstrap listesi gibi göster
        sessions.forEach(s => {
            const li = document.createElement("li");
            li.classList.add("list-group-item");
            const status = s.isCompleted ? "Tamamlandı" : "Devam ediyor";
            
            li.textContent =
                "ID: " + s.id +
                " | Süre: " + s.durationMinutes + " dk" +
                " | Başlangıç: " + new Date(s.startTime).toLocaleString() +
                " | Durum: " + status;
            list.appendChild(li);
        });
    } catch (err) {
        alert("İstek sırasında bağlantı hatası oluştu: " + err);
    }
}