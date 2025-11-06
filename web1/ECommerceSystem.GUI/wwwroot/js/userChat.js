"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const messagesDiv = document.getElementById("chatMessages");
    const input = document.getElementById("chatMessageInput");
    const sendBtn = document.getElementById("sendChatMessage");
    const closeBtn = document.getElementById("closeChat");

    if (!messagesDiv || !input || !sendBtn) {
        console.error("Chat elements not found on this page.");
        return;
    }

    const currentUser = window.currentUser;
    if (!currentUser) {
        alert("Bạn cần đăng nhập để sử dụng chat!");
        return;
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub", {
            accessTokenFactory: () => localStorage.getItem("authToken")
        })
        .withAutomaticReconnect()
        .build();

    function appendMessage(sender, text) {
        const msgDiv = document.createElement("div");
        msgDiv.classList.add("chat-message", sender === "user" ? "user" : "admin");
        msgDiv.textContent = text;
        messagesDiv.appendChild(msgDiv);
        messagesDiv.scrollTop = messagesDiv.scrollHeight;
    }

    sendBtn.addEventListener("click", sendMessage);
    input.addEventListener("keydown", e => { if (e.key === "Enter") sendMessage(); });
    closeBtn.addEventListener("click", () => window.location.href = "/Home/Index");

    function sendMessage() {
        const msg = input.value.trim();
        if (!msg) return;

        connection.invoke("SendMessageFromCustomer", currentUser.id, msg)
            .catch(err => console.error(err));

        appendMessage("user", `Bạn: ${msg}`);
        input.value = "";
    }

    connection.on("ReceiveMessage", (senderId, message) => {
        appendMessage("admin", `Admin: ${message}`);
    });

    connection.start()
        .then(() => console.log("Kết nối SignalR thành công"))
        .catch(err => console.error("Lỗi kết nối SignalR:", err));
});
