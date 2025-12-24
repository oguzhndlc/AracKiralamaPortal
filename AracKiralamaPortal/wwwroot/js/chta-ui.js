document.addEventListener("DOMContentLoaded", () => {
    console.log("💬 kullanıcı status sayfası yüklendi");

    const userId = document.getElementById("currentUserId")?.value;
    const connection = window.chatConnection;
    if (!connection) {
        console.error("❌ SignalR connection bulunamadı");
        return;
    }

    connection.on("UpdateOnlineUsers", users => {
        console.log("ONLINE USERS:", users);
        document.querySelectorAll("td[id^='status_']").forEach(td => {
            const dot = td.querySelector(".status-dot");
            const text = td.querySelector("span:last-child");
            if (dot) dot.classList.remove("status-online");
            if (dot) dot.classList.add("status-offline");
            if (text) text.innerText = "Offline";
        });
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
});
