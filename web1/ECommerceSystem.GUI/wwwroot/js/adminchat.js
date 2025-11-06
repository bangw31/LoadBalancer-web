"use strict";

// ==================== LẤY TOKEN ADMIN ====================
const token = localStorage.getItem("authToken");
if (!token) {
    alert("Bạn cần đăng nhập lại với quyền admin!");
}

// ==================== SIGNALR CONNECTION ====================
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7068/chathub", { accessTokenFactory: () => token })
    .withAutomaticReconnect()
    .build();

let currentUserId = null;
let adminId = null;
const appendedMessageIds = new Set(); // dedupe set

// ==================== GET ADMIN ID FROM JWT ====================
function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

const payload = parseJwt(token);
const roles = payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
if (Array.isArray(roles) ? roles.includes("Admin") : roles === "Admin") {
    adminId = parseInt(payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || 1);
} else {
    alert("Token không hợp lệ hoặc không phải admin!");
}

// ==================== LOAD USER LIST ====================
async function loadUserList() {
    try {
        const res = await fetch("https://localhost:7068/api/chat/users", {
            headers: { "Authorization": "Bearer " + token }
        });
        if (!res.ok) {
            const txt = await res.text();
            console.error("❌ /api/chat/users failed:", res.status, txt);
            return;
        }
        const users = await res.json();

        const listDiv = document.getElementById("userList");
        if (!listDiv) return;
        listDiv.innerHTML = "";
        users.forEach(u => {
            const id = u.id ?? u.Id;
            const email = u.email ?? u.Email ?? u.UserName ?? ("User#" + id);
            const div = document.createElement("div");
            div.className = "user-item p-2 border-bottom";
            div.style.cursor = "pointer";
            div.textContent = email;
            div.onclick = () => openChat(id, email);
            listDiv.appendChild(div);
        });
    } catch (err) {
        console.error("❌ Không thể tải danh sách user:", err);
    }
}

// ==================== OPEN CHAT ====================
async function openChat(userId, email) {
    currentUserId = userId;
    const title = document.getElementById("chatTitle");
    if (title) title.textContent = "💬 Chat với " + email;
    const messages = document.getElementById("chatMessages");
    if (messages) messages.innerHTML = "";

    await loadChatHistory(userId);

    localStorage.setItem("currentChatUserId", String(userId));
    localStorage.setItem("currentChatUserEmail", email);
}

// ==================== LOAD CHAT HISTORY ====================
async function loadChatHistory(userId) {
    try {
        const res = await fetch(`https://localhost:7068/api/chat/history?withUserId=${userId}`, {
            headers: { "Authorization": "Bearer " + token }
        });

        if (!res.ok) {
            const txt = await res.text();
            console.error("❌ /api/chat/history failed:", res.status, txt);
            return;
        }

        const contentType = res.headers.get('content-type') || '';
        let messages;
        if (contentType.includes('application/json')) {
            try {
                messages = await res.json();
            } catch (e) {
                console.error("❌ Failed to parse JSON from /api/chat/history:", e);
                return;
            }
        } else {
            const txt = await res.text();
            console.warn("⚠️ /api/chat/history returned non-JSON:", txt);
            return;
        }

        console.log("💬 Lịch sử user:", messages);

        const chatDiv = document.getElementById("chatMessages");
        if (!chatDiv) return;
        chatDiv.innerHTML = "";

        messages.forEach(msg => {
            // dedupe by Id if server provided it in history
            const id = msg.id ?? msg.Id ?? null;
            if (id && appendedMessageIds.has(id)) return;
            if (id) appendedMessageIds.add(id);

            const senderId = msg.fromUserId ?? msg.FromUserId ?? msg.fromId ?? msg.senderId ?? msg.senderI;
            const content = msg.content ?? msg.Content ?? "";
            const sentAt = msg.sentAt ?? msg.SentAt ?? null;
            const sender = (senderId === adminId) ? "Admin" : "User";
            appendMessage(sender, content, sentAt);
        });

        scrollToBottom();
    } catch (err) {
        console.error("❌ Lỗi tải lịch sử chat:", err);
    }
}

// ==================== APPEND MESSAGE ====================
function appendMessage(sender, content, time = null) {
    const div = document.createElement("div");
    const now = time ? new Date(time) : new Date();
    const timeStr = now.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    div.className = sender === "Admin" ? "text-end mb-2" : "text-start mb-2";
    div.innerHTML = `
        <div class="d-inline-block px-2 py-1 rounded ${sender === "Admin" ? "bg-primary text-white" : "bg-light"}">
            <strong>${sender}:</strong> ${escapeHtml(content)}
        </div>
        <div class="text-muted" style="font-size:0.8em;">${timeStr}</div>
    `;
    const container = document.getElementById("chatMessages");
    if (container) container.appendChild(div);
}

function scrollToBottom() {
    const div = document.getElementById("chatMessages");
    if (div) div.scrollTop = div.scrollHeight;
}

// small helper to avoid injecting raw HTML from messages
function escapeHtml(unsafe) {
    return String(unsafe)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// ==================== SEND MESSAGE ====================
async function sendAdminMessage() {
    const input = document.getElementById("chatMessageInput");
    const sendBtn = document.getElementById("sendChatBtn") || document.getElementById("sendChatMessage");
    const message = input?.value.trim();
    if (!message || !currentUserId) return;

    // disable button while waiting for server to persist + broadcast
    if (sendBtn) sendBtn.disabled = true;

    try {
        const payload = { FromUserId: adminId, ToUserId: currentUserId, Content: message };
        console.log("➡️ POST /api/chat/admin/send payload:", payload);

        const res = await fetch("https://localhost:7068/api/chat/admin/send", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": "Bearer " + token
            },
            body: JSON.stringify(payload)
        });

        if (!res.ok) {
            const txt = await res.text().catch(() => "");
            console.error("❌ Gửi tin nhắn thất bại:", res.status, txt);
            return;
        }

        // Do NOT append locally here; server will broadcast the persisted message back via SignalR.
        // Clearing input is fine so user can type next message.
        if (input) input.value = "";
    } catch (err) {
        console.error("❌ Gửi tin nhắn lỗi:", err);
    } finally {
        if (sendBtn) sendBtn.disabled = false;
    }
}

// ==================== RECEIVE MESSAGE REALTIME ====================
connection.on("ReceiveMessage", (message) => {
    // message may be an object or different shapes depending on sender
    const fromId = message?.fromUserId ?? message?.FromUserId ?? message?.fromId ?? message?.senderId ?? message?.senderI;
    const toId = message?.toUserId ?? message?.ToUserId;
    const content = message?.content ?? message?.Content ?? (typeof message === "string" ? message : "");
    const id = message?.id ?? message?.Id ?? null;
    const sentAt = message?.sentAt ?? message?.SentAt ?? null;

    if (!fromId && !toId && !content) return;

    // dedupe by Id if available
    if (id && appendedMessageIds.has(id)) return;
    if (id) appendedMessageIds.add(id);

    if (fromId === adminId && toId === currentUserId) {
        appendMessage("Admin", content, sentAt);
        scrollToBottom();
    } else if (fromId === currentUserId) {
        appendMessage("User", content, sentAt);
        scrollToBottom();
    } else {
        // new message from other user -> visual hint in user list
        console.log(`📨 Tin mới từ user #${fromId}`);
        highlightUserInList(fromId);
    }
});

function highlightUserInList(userId) {
    const listDiv = document.getElementById("userList");
    if (!listDiv) return;
    const userItem = Array.from(listDiv.children).find(el => el.onclick && el.onclick.toString().includes(String(userId)));
    if (userItem) {
        userItem.style.backgroundColor = "#ffeeba";
        setTimeout(() => userItem.style.backgroundColor = "", 2000);
    }
}

// ==================== KẾT NỐI SIGNALR ====================
connection.start()
    .then(() => {
        console.log("✅ Admin connected to SignalR");
        loadUserList();

        const lastUserId = localStorage.getItem("currentChatUserId");
        const lastEmail = localStorage.getItem("currentChatUserEmail");
        if (lastUserId && lastEmail) {
            openChat(parseInt(lastUserId), lastEmail);
        }
    })
    .catch(err => console.error("❌ SignalR error:", err));

// ==================== EVENTS (attach after DOM ready) ====================
document.addEventListener("DOMContentLoaded", () => {
    const sendBtn = document.getElementById("sendChatBtn") || document.getElementById("sendChatMessage");
    const input = document.getElementById("chatMessageInput");

    // avoid double-binding
    if (sendBtn && !sendBtn.dataset.bound) {
        sendBtn.addEventListener("click", sendAdminMessage);
        sendBtn.dataset.bound = "1";
    }

    if (input && !input.dataset.bound) {
        input.addEventListener("keydown", e => {
            if (e.key === "Enter") sendAdminMessage();
        });
        input.dataset.bound = "1";
    }
});
