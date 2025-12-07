// js/goalManager.js

// Hedef belirleme
async function setGoal() {
    const input = document.getElementById("goalInput");
    const minutes = parseInt(input.value, 10);

    if (isNaN(minutes) || minutes < 1 || minutes > 1440) {
        alert("Lütfen 1 ile 1440 arasında geçerli bir hedef dakika giriniz.");
        return;
    }

    try {
        const response = await fetch(GOAL_URL + "/set-goal", { 
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ targetMinutes: minutes })
        });

        const data = await handleApiResponse(response);
        if (!data.success) {
            alert("Hedef belirlenemedi: " + data.message);
            return;
        }

        alert(`Yeni günlük hedefiniz ${minutes} dakika olarak ayarlandı.`);
        updateGoalProgress(); 
    } catch (err) {
        alert("Hedef isteği sırasında bağlantı hatası oluştu: " + err);
    }
}

// İlerleme çubuğunu ve mesajları güncelleme
async function updateGoalProgress() {
    try {
        const response = await fetch(GOAL_URL + "/summary");
        const data = await handleApiResponse(response);

        const congratsMessage = document.getElementById("congratsMessage");
        
        if (!response.ok || !data.success) {
            document.getElementById("goalDisplay").textContent = `Günlük Hedef: (API Hatası)`;
            congratsMessage.style.display = 'none';
            return;
        }

        const summary = data.body;
        const percentage = summary.progressPercentage;
        const progressBar = document.getElementById("progressBar");

        // Metinleri güncelle
        document.getElementById("goalDisplay").textContent = `Günlük Hedef: ${summary.targetMinutes} dakika`;
        document.getElementById("progressText").textContent = 
            `Bugün Çalışılan: ${summary.totalMinutesToday} dakika (%${percentage})`;

        // Çubuğu güncelle
        progressBar.style.width = `${percentage}%`;
        
        // %100'ü geçince rengi ve tebrikler mesajını güncelle
        if (percentage >= 100) {
            progressBar.style.backgroundColor = '#28a745'; 
            congratsMessage.style.display = 'block'; 
        } else {
            progressBar.style.backgroundColor = '#4CAF50'; 
            congratsMessage.style.display = 'none'; 
        }
        
        // Diğer bölümleri otomatik güncelle
        loadToday(); 
        loadSessions(); 

    } catch (err) {
        console.error("Hedef takibi yüklenirken hata:", err);
    }
}