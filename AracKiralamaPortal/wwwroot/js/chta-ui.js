document.addEventListener("DOMContentLoaded", () => {
    console.log("💬 kullanıcı status sayfası yüklendi");

    const userId = document.getElementById("currentUserId")?.value;
    const connection = window.chatConnection; // chat-realtime.js ile kurulu SignalR
    if (!connection) {
        console.error("❌ SignalR connection bulunamadı");
        return;
    }

    connection.on("UpdateOnlineUsers", users => {
        console.log("ONLINE USERS:", users);

        // Önce tüm status’ları offline yap
        document.querySelectorAll("td[id^='status_']").forEach(td => {
            const dot = td.querySelector(".status-dot");
            const text = td.querySelector("span:last-child");
            if (dot) dot.classList.remove("status-online");
            if (dot) dot.classList.add("status-offline");
            if (text) text.innerText = "Offline";
        });

        // Online olan kullanıcıları işaretle
        users.forEach(u => {
            const td = document.getElementById(`status_${u.id}`);
            if (!td) return;

            const dot = td.querySelector(".status-dot");
            const text = td.querySelector("span:last-child");
            if (dot) {
                dot.classList.remove("status-offline");
                dot.classList.add("status-online");
            }
            if (text) text.innerText = "Online";
        });
    });

    // Eğer sayfa açıldığında bağlantı zaten başlatılmışsa kullanıcı listesi gelsin
    // (örn: connection.invoke("GetOnlineUsers") gibi server tarafı method varsa çağırabilirsin)
});
