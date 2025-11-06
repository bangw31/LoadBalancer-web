"use strict";

/* ====================== GUARD (ngăn load 2 lần) ====================== */
if (window.__commentsBootstrapped) {
    console.debug("[comments] already bootstrapped");
} else {
    window.__commentsBootstrapped = true;

    /* ====================== CONFIG ====================== */
    const BASE_URL = "https://localhost:7068"; // backend API + hub
    const token = localStorage.getItem("authToken");

    // Lấy productId từ hidden input
    const productIdEl = document.getElementById("productId");
    if (!productIdEl) throw new Error("productId element not found");
    const productId = parseInt(productIdEl.value);
    if (isNaN(productId)) throw new Error("productId is not a number");

    // DOM elements
    const commentsList = document.getElementById("commentsList");
    const commentInput = document.getElementById("commentInput");
    const commentBtn = document.getElementById("commentBtn");

    // ensure a fallback so scripts don't crash if BASE_URL isn't set
    const API_BASE = (window.BASE_URL && window.BASE_URL.toString()) || '';

    /* ====================== DOM HELPERS ====================== */
    function addCommentToDOM(comment, prepend = false) {
        if (!commentsList) return;
        const div = document.createElement("div");
        div.className = "list-group-item mb-2";
        div.innerHTML = `<strong>${comment.userName || "User"}:</strong>
                     ${comment.content} <br/>
                     <small>${new Date(comment.createdAt).toLocaleString()}</small>`;
        if (prepend) commentsList.prepend(div);
        else commentsList.appendChild(div);
    }

    function safeParseJson(text) {
        if (!text || !text.trim()) return null;
        try { return JSON.parse(text); } catch { return null; }
    }

    /* ====================== LOAD COMMENTS ====================== */
    async function loadComments() {
        try {
            if (!commentsList) return;
            console.log("🔄 Loading comments...");
            const res = await fetch(`${API_BASE}/api/comments/product/${productId}`, {
                headers: token ? { Authorization: "Bearer " + token } : {}
            });

            if (!res.ok) {
                console.error("❌ Load comments failed with status:", res.status);
                commentsList.innerHTML = '<div class="text-muted">Không tải được bình luận</div>';
                return;
            }

            const text = await res.text();          // tránh lỗi body rỗng
            const comments = safeParseJson(text) || [];
            commentsList.innerHTML = "";
            comments.forEach(c => addCommentToDOM(c, false));
            console.log(`✅ Loaded ${comments.length} comments`);
        } catch (err) {
            console.error("❌ Load comments failed:", err);
            if (commentsList) commentsList.innerHTML = '<div class="text-muted">Không tải được bình luận</div>';
        }
    }

    /* ====================== SIGNALR ====================== */
    // Kết nối 1 lần, tự retry
    if (!window.commentConn && token) {
        window.commentConn = new signalR.HubConnectionBuilder()
            .withUrl(`${BASE_URL}/commenthub`, { accessTokenFactory: () => token })
            .withAutomaticReconnect()
            .build();

        // Nhận comment realtime từ server
        window.commentConn.on("ReceiveComment", comment => {
            // Server PHẢI gửi kèm productId trong payload
            if (comment?.productId === productId) addCommentToDOM(comment, true);
        });

        async function startConnection() {
            try {
                await window.commentConn.start();
                console.log("✅ SignalR connected");
                // join group theo productId
                await window.commentConn.invoke("JoinProductGroup", productId);
            } catch (err) {
                console.error("❌ SignalR connection failed:", err);
                setTimeout(startConnection, 5000); // retry 5s
            }
        }
        startConnection();
    }

    /* ====================== SEND COMMENT ====================== */
    let sending = false;
    async function sendComment() {
        if (!commentInput || sending) return;
        const content = commentInput.value.trim();
        if (!content) return;

        const payload = { productId, content };
        console.log("Sending comment payload:", payload);

        sending = true;
        commentBtn && (commentBtn.disabled = true);

        try {
            const res = await fetch(`${BASE_URL}/api/comments`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    ...(token ? { Authorization: "Bearer " + token } : {})
                },
                body: JSON.stringify(payload)
            });

            const text = await res.text(); // luôn đọc text để lấy thông điệp lỗi nếu có
            if (!res.ok) {
                console.error(`❌ HTTP ${res.status}:`, text);
                let msg = "Không thể gửi bình luận";
                const maybe = safeParseJson(text);
                if (maybe?.message) msg = maybe.message;
                alert(msg);
                return;
            }

            // API nên trả JSON của comment vừa tạo
            const comment = safeParseJson(text);
            commentInput.value = "";

            // Hiển thị ngay trên UI (optimistic update); realtime từ server vẫn sẽ tới nhưng không bị double
            if (comment) addCommentToDOM(comment, true);
        } catch (err) {
            console.error("❌ Send comment failed:", err);
            alert("Không thể gửi bình luận");
        } finally {
            sending = false;
            commentBtn && (commentBtn.disabled = false);
        }
    }

    /* ====================== EVENT LISTENERS ====================== */
    commentBtn?.addEventListener("click", sendComment);
    commentInput?.addEventListener("keyup", e => { if (e.key === "Enter") sendComment(); });

    /* ====================== INITIAL LOAD ====================== */
    loadComments();

    /* ====================== DEBUG ====================== */
    console.log("👤 currentUser =", JSON.stringify({ token }, null, 2));
}
