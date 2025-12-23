document.addEventListener("DOMContentLoaded", () => {
    console.log("💬 chat-ui.js yüklendi");

    const userId = document.getElementById("currentUserId")?.value;
    const userName = document.getElementById("currentUserName")?.value;

    const messageInput = document.getElementById("messageInput");
    const chatMessages = document.getElementById("chatMessages");
    const chatList = document.getElementById("chatList");
    const chatTitle = document.getElementById("chatTitle");
    const typingStatus = document.getElementById("typingStatus");
    const onlineUsersList = document.getElementById("onlineUsersList");

    window.selectedUser = "";
    let messageCache = { general: [], private: {} };

    const connection = window.chatConnection; // Eğer chat-realtime.js yüklenmişse
    if (!connection) {
        console.error("❌ SignalR connection bulunamadı");
        return;
    }

    // ---------------- MESAJ GÖNDER ----------------
    function sendMessage() {
        if (!messageInput || !messageInput.value.trim()) return;

        const text = messageInput.value.trim();
        const time = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

        const messageData = {
            sender: userId,
            senderName: userName,
            receiver: window.selectedUser || null,
            text: text,
            time: time
        };

        // SignalR ile gönder
        if (window.selectedUser) {
            connection.invoke("SendPrivateMessage", window.selectedUser, text)
                .catch(err => console.error("❌ Private message gönderilemedi:", err));
        } else {
            connection.invoke("SendMessage", text)
                .catch(err => console.error("❌ Genel mesaj gönderilemedi:", err));
        }

        // Cache ve UI
        if (window.selectedUser) {
            messageCache.private[window.selectedUser] = messageCache.private[window.selectedUser] || [];
            messageCache.private[window.selectedUser].push(messageData);
        } else {
            messageCache.general.push(messageData);
        }
        appendMessage(messageData)
        clearUnreadBadge(window.selectedUser);
        messageInput.value = "";
        stopTyping();
    }

    // ---------------- EVENT LISTENER ----------------
    document.getElementById("sendMessage")?.addEventListener("click", sendMessage);

    messageInput?.addEventListener("keydown", e => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault(); // Enter ile yeni satır engellenir
            sendMessage();
        }
    });

    // -------------------- CHAT SEÇİMİ --------------------
    window.selectUser = function (id, name) {
        window.selectedUser = id;
        chatTitle.innerText = name || "Genel Sohbet";
        chatMessages.innerHTML = "";
        typingStatus.innerText = "";

        const history = id
            ? messageCache.private[id] || []
            : messageCache.general;

        history.forEach(m => appendMessage(m));
        clearUnreadBadge(id);
    };

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

    // -------------------- GLOBAL MESAJ EKLEME --------------------
    window.appendMessageToUI = (data) => {
        const senderId = data.sender === userId ? data.receiver : data.sender;

        // Sadece karşı taraftan gelen mesajlar için
        const isMyMessage = data.sender === userId;
        if (!isMyMessage && senderId) {
            updateUnreadBadge(senderId);
            showToast(data.senderName, data.text, senderId, data.senderName);
        }


        // Cache ekle
        if (senderId) {
            messageCache.private[senderId] = messageCache.private[senderId] || [];
            messageCache.private[senderId].push(data);
        } else {
            messageCache.general.push(data);
        }

        // Seçili kullanıcıya göster
        if (window.selectedUser === senderId || (!senderId && window.selectedUser === "")) {
            appendMessage(data);
            clearUnreadBadge(senderId);
        }

        // Chat listesine ekle
        addChatToListIfNeeded(senderId, data.senderName);
    };


    // -------------------- ÇEVRİMİÇİ KULLANICILAR --------------------
    connection.on("UpdateOnlineUsers", users => {
        console.log("ONLINE USERS:", users);

        const list = document.getElementById("onlineUsersList"); // doğru ID
        if (!list) return;

        list.innerHTML = ""; // önce temizle

        users.forEach(u => {
            if (u.id === userId) return; // kendimizi göstermeyelim

            const li = document.createElement("li");
            li.textContent = u.username; // property isimleri SignalR’dan gelene uygun olmalı
            li.onclick = () => selectUser(u.id, u.username);

            // badge için span ekleyebiliriz
            const badge = document.createElement("span");
            badge.id = `badge-${u.id}`;
            badge.className = "badge";
            badge.style.display = "none"; // başlangıçta görünmesin
            li.appendChild(badge);

            list.appendChild(li);

            // Sohbet listesine ekle (chatList)
            addChatToListIfNeeded(u.id, u.username);
        });
    });


    // -------------------- TYPING --------------------
    let typingTimeout = null;
    let lastTypingSent = 0;

    function stopTyping() {
        clearTimeout(typingTimeout);
        if (window.connection) {
            window.connection.invoke("StopTyping", window.selectedUser || null)
                .catch(err => console.error(err));
        }
    }

    // messageInput input event listener
    messageInput?.addEventListener("input", () => {
        const now = Date.now();
        if (now - lastTypingSent > 500) {
            if (window.connection) {
                window.connection.invoke("Typing", window.selectedUser || null)
                    .catch(err => console.error(err));
            }
            lastTypingSent = now;
        }

        clearTimeout(typingTimeout);
        typingTimeout = setTimeout(stopTyping, 1000);
    });


    // -------------------- URL’den kullanıcı aç --------------------
    const params = new URLSearchParams(window.location.search);
    const openUser = params.get("userId");
    if (openUser) selectUser(openUser, "Sohbet");
});
