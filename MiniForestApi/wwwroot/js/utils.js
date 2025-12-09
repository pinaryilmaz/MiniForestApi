// js/utils.js

// Backend adreslerini global olarak tanımlıyoruz
const BASE_URL = "http://localhost:5097";
const FOCUS_URL = BASE_URL + "/Focus";
const GOAL_URL = BASE_URL + "/Goal";

// Global oturum değişkenlerini tanımlıyoruz (window ile erişilebilir)
window.currentSessionId = null;
window.timerInterval = null;
window.remainingSeconds = 0;

// Hata İşleyici Fonksiyon: API yanıtını işler (Tüm Manager'lar kullanır)
async function handleApiResponse(response) {
    const data = await response.json().catch(() => ({})); 
    
    if (response.ok) {
        return { success: true, body: data.body, message: data.message };
    }

    if (data && data.errors) {
        let validationMessages = '';
        for (const key in data.errors) {
            validationMessages += data.errors[key].join(', ') + '\n';
        }
        return { success: false, message: 'Doğrulama Hatası:\n' + validationMessages };
    }
    
    if (data && data.message) {
        return { success: false, message: data.message };
    }

    return { success: false, message: `Sunucu hatası. HTTP Status: ${response.status}` };
}