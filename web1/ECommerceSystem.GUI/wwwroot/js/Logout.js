"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const logoutBtn = document.getElementById("logoutBtn");
    if (!logoutBtn) return;

    logoutBtn.addEventListener("click", async function () {
        try {
            await fetch("https://localhost:7068/api/auth/logout", { method: "POST" });
        } catch (err) {
            console.warn("Logout API lỗi:", err);
        }

        // Xóa token + user khỏi localStorage
        localStorage.removeItem("authToken");
        localStorage.removeItem("currentUser");

        // Chuyển về trang login
        window.location.href = "/Account/Login";
    });
});
