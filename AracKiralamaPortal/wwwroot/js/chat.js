document.addEventListener("DOMContentLoaded", () => {
    console.log("✅ CHAT JS YÜKLENDİ");
    const userInputId = document.getElementById("currentUserId");
    const userInputName = document.getElementById("currentUserName");

    const userId = userInputId?.value || null;
    const userName = userInputName?.value || null;

    const messageInput = document.getElementById("messageInput");
    const chatMessages = document.getElementById("chatMessages");
    const chatList = document.getElementById("chatList");
    const typingStatus = document.getElementById("typingStatus");
    const chatTitle = document.getElementById("chatTitle");

    let selectedUser = "";
    let typingTimeout = null;
    let lastTypingSent = 0;

    let messageCache = {
        general: [],
        private: {}
    };

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .withAutomaticReconnect()
        .build();

    connection.start()
        .then(() => {
            console.log("SignalR bağlandı");
            if (userId && userName) {
                connection.invoke("RegisterUser", userId, userName);
            }
        })
        .catch(err => console.error(err));

    function sendMessage() {
        if (!messageInput || !messageInput.value.trim()) return;

        const text = messageInput.value.trim();

        if (selectedUser) {
            connection.invoke("SendPrivateMessage", selectedUser, text);
        } else {
            connection.invoke("SendMessage", text);
        }

        messageInput.value = "";

        stopTyping();
    }

    document.getElementById("sendMessage")?.addEventListener("click", sendMessage);

    messageInput?.addEventListener("keydown", e => {
        if (e.key === "Enter") sendMessage();
    });

    messageInput?.addEventListener("input", () => {
        const now = Date.now();
        if (now - lastTypingSent > 500) {
            connection.invoke("Typing", selectedUser || null);
            lastTypingSent = now;
        }

        clearTimeout(typingTimeout);
        typingTimeout = setTimeout(stopTyping, 1000);
    });

    function stopTyping() {
        clearTimeout(typingTimeout);
        connection.invoke("StopTyping", selectedUser || null);
    }

    connection.on("ReceiveMessage", data => {
        messageCache.general.push(data);

        if (!selectedUser) {
            appendMessage(data);
        }
    });

    connection.on("ReceivePrivateMessage", data => {
        console.log("📩 PRIVATE MESSAGE GELDİ:", data);
        const otherUserId = data.sender === userId ? data.receiver : data.sender;
        const otherUserName = data.sender === userId ? data.receiverName : data.senderName;

        if (!messageCache.private[otherUserId]) {
            messageCache.private[otherUserId] = [];
            addChatToListIfNeeded(otherUserId, otherUserName);
        }

        messageCache.private[otherUserId].push(data);

        if (chatMessages && selectedUser === otherUserId) {
            appendMessage(data);
        } else {
            updateUnreadBadge(otherUserId);
            showToast(otherUserName, data.text, otherUserId, otherUserName);
        }
    });




    connection.on("UpdateOnlineUsers", users => {
        console.log("ONLINE USERS:", users);
        const list = document.getElementById("onlineUsers");
        if (list) {
            list.innerHTML = "";
            users.forEach(u => {
                if (u.id === userId) return;
                const li = document.createElement("li");
                li.textContent = u.username;
                li.onclick = () => selectUser(u.id, u.username);
                list.appendChild(li);
            });
        }
        document.querySelectorAll("td[id^='status_']").forEach(td => {
            const dot = td.querySelector(".status-dot");
            const text = td.querySelector("span:last-child");

            dot.classList.remove("status-online");
            dot.classList.add("status-offline");
            text.innerText = "Offline";
        });

        users.forEach(u => {
            const td = document.getElementById(`status_${u.id}`);
            if (!td) return;

            const dot = td.querySelector(".status-dot");
            const text = td.querySelector("span:last-child");

            dot.classList.remove("status-offline");
            dot.classList.add("status-online");
            text.innerText = "Online";
        });
    });


    connection.on("UserTyping", senderUserId => {
        if (!typingStatus) return;

        if (selectedUser === senderUserId || (!selectedUser && senderUserId)) {
            typingStatus.innerText = "Yazıyor...";
        }
    });

    connection.on("UserStoppedTyping", senderUserId => {
        if (selectedUser === senderUserId || !selectedUser) {
            if (typingStatus) typingStatus.innerText = "";
        }
    });
    window.selectUser = function (id, name) {
        selectedUser = id;
        chatTitle.innerText = name || "Genel Sohbet";
        chatMessages.innerHTML = "";
        if (typingStatus) typingStatus.innerText = "";

        const history = selectedUser
            ? messageCache.private[id] || []
            : messageCache.general;

        history.forEach(m => appendMessage(m));
        clearUnreadBadge(id);
    };

    chatTitle?.addEventListener("click", () => selectUser("", "Genel Sohbet"));

    function appendMessage(data) {
        const isMe = data.sender === userId;

        const div = document.createElement("div");
        div.className = `message ${isMe ? "outgoing" : "incoming"}`;

        if (!isMe) {
            const small = document.createElement("small");
            small.innerText = data.senderName;
            div.appendChild(small);
        }

        const p = document.createElement("p");
        p.innerText = data.text;

        const time = document.createElement("span");
        time.className = "time";
        time.innerText = data.time;

        div.appendChild(p);
        div.appendChild(time);

        chatMessages.appendChild(div);
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }
    injectChatStyles();
    function injectChatStyles() {
        const style = document.createElement("style");
        style.innerHTML = `
        #chatMessages {
            padding: 15px;
            display: flex;
            flex-direction: column;
            gap: 10px;
            overflow-y: auto;
            background: #0f172a;
        }

        .message {
            max-width: 70%;
            padding: 10px 14px;
            border-radius: 12px;
            word-wrap: break-word;
            animation: fadeIn 0.2s ease-in;
            font-family: Arial, sans-serif;
        }

        .message.incoming {
            align-self: flex-start;
            background: #1e293b;
            color: #e5e7eb;
            border-top-left-radius: 0;
        }

        .message.outgoing {
            align-self: flex-end;
            background: #2563eb;
            color: #ffffff;
            border-top-right-radius: 0;
        }

        .message p {
            margin: 0;
            font-size: 14px;
            line-height: 1.4;
        }

        .message small {
            display: block;
            font-size: 11px;
            opacity: 0.7;
            margin-bottom: 4px;
        }

        .message .time {
            display: block;
            text-align: right;
            font-size: 10px;
            opacity: 0.6;
            margin-top: 4px;
        }

        #typingStatus {
            font-size: 12px;
            color: #38bdf8;
            margin-left: 10px;
            animation: blink 1.2s infinite;
        }

        @keyframes fadeIn {
            from { opacity: 0; transform: translateY(4px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @keyframes blink {
            0% { opacity: .3; }
            50% { opacity: 1; }
            100% { opacity: .3; }
        }
    `;
        document.head.appendChild(style);
    }

    function showToast(title, message, userId, userName, duration = 4000) {
        console.log("🔥 showToast çağrıldı:", title, message, userId, userName);
        let container = document.querySelector(".toast-container");
        if (!container) {
            container = document.createElement("div");
            container.className = "toast-container";
            document.body.appendChild(container);
        }

        const toast = document.createElement("div");
        toast.className = "toast-notification";

        toast.innerHTML = `
        <strong>${title}</strong>
        <div>${message}</div>
    `;

        toast.addEventListener("click", () => {
            selectUser(userId, userName);
            toast.remove();
        });

        container.appendChild(toast);
        console.log("🧱 Toast DOM'a eklendi:", toast);
        setTimeout(() => toast.classList.add("show"), 10);

        setTimeout(() => {
            toast.classList.remove("show");
            setTimeout(() => toast.remove(), 300);
        }, duration);
    }



    function addChatToListIfNeeded(id, name) {
        if (document.getElementById(`chat-item-${id}`)) return;

        const li = document.createElement("li");
        li.id = `chat-item-${id}`;
        li.innerHTML = `${name} <span id="badge-${id}" class="badge"></span>`;
        li.onclick = () => selectUser(id, name);
        chatList.appendChild(li);
    }
        
    function updateUnreadBadge(id) {
        const badge = document.getElementById(`badge-${id}`);
        if (badge) badge.style.display = "inline-block";
    }

    function clearUnreadBadge(id) {
        const badge = document.getElementById(`badge-${id}`);
        if (badge) badge.style.display = "none";
    }

    function injectToastStyles() {
        const style = document.createElement("style");
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
        transition: opacity .3s ease, transform .3s ease;
        box-shadow: 0 8px 20px rgba(0,0,0,.35);
        font-family: Arial, sans-serif;
    }

    .toast-notification.show {
        opacity: 1;
        transform: translateY(0);
    }

    .toast-notification strong {
        display: block;
        font-size: 14px;
        margin-bottom: 4px;
    }
    `;
        document.head.appendChild(style);
    }

    injectToastStyles();
});
