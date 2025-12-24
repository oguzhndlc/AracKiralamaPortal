document.addEventListener("DOMContentLoaded", () => {
    console.log("🌍 chat-realtime.js yüklendi");

    let selectedUser = "";

    const userId = document.getElementById("currentUserId")?.value;
    const userName = document.getElementById("currentUserName")?.value;

    if (!userId) {
        console.warn("❌ userId yok, realtime başlatılmadı");
        return;
    }
    window.chatConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .withAutomaticReconnect()
        .build();

    const connection = window.chatConnection;

    connection.on("ReceivePrivateMessage", data => {
        console.log("🔔 GLOBAL mesaj alındı:", data);

        const otherUserId = data.sender === userId ? data.receiver : data.sender;
        const otherUserName = data.sender === userId ? data.receiverName : data.senderName;

        if (data.sender === userId) return;

        if (window.appendMessageToUI) window.appendMessageToUI(data);

        if (window.selectedUser !== otherUserId) {
            if (window.showChatToast) window.showChatToast(otherUserName, data.text, otherUserId);
        }
    });


    connection.on("UpdateOnlineUsers", users => {
        console.log("🌐 Çevrimiçi kullanıcılar:", users);
        if (window.updateOnlineUsersUI) window.updateOnlineUsersUI(users);
    });

    connection.start()
        .then(() => {
            console.log("🟢 SignalR bağlandı");
            connection.invoke("RegisterUser", userId, userName);
        })
        .catch(err => console.error(err));

    window.showChatToast = (title, message, userId, duration = 4000) => {
        let container = document.querySelector(".toast-container");
        if (!container) {
            container = document.createElement("div");
            container.className = "toast-container";
            document.body.appendChild(container);
        }

        const toast = document.createElement("div");
        toast.className = "toast-notification show";
        toast.innerHTML = `<strong>${title}</strong><div>${message}</div>`;

        toast.onclick = () => {
            window.location.href = `/Admin/Messages?userId=${userId}`;
        };

        container.appendChild(toast);

        setTimeout(() => {
            toast.classList.remove("show");
            setTimeout(() => toast.remove(), 300);
        }, duration);
    };

    (function injectToastStyles() {
        if (document.getElementById("toast-style")) return;

        const style = document.createElement("style");
        style.id = "toast-style";
        style.innerHTML = `
            .toast-container {
                position: fixed;
                bottom: 20px;
                right: 20px;
                z-index: 999999;
                display: flex;
                flex-direction: column;
                gap: 10px;
            }

            .toast-notification {
                background: #2563eb;
                color: #fff;
                padding: 12px 16px;
                border-radius: 8px;
                min-width: 240px;
                cursor: pointer;
                opacity: 0;
                transform: translateY(15px);
                transition: all .3s ease;
                box-shadow: 0 8px 20px rgba(0,0,0,.35);
                font-family: Arial, sans-serif;
            }

            .toast-notification.show {
                opacity: 1;
                transform: translateY(0);
            }

            .toast-notification strong {
                display: block;
                margin-bottom: 4px;
            }
        `;
        document.head.appendChild(style);
    })();
});
