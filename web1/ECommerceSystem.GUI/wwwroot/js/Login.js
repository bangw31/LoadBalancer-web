"use strict";

document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("loginForm");
    const errorDiv = document.getElementById("loginError");

    if (!form) return console.warn("Form login không tồn tại trên trang.");

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const username = document.getElementById("Username")?.value.trim();
        const password = document.getElementById("Password")?.value.trim();

        if (!username || !password) {
            errorDiv.textContent = "Vui lòng nhập username và password";
            errorDiv.classList.remove("d-none");
            return;
        }

        try {
            const res = await fetch("/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!res.ok) throw new Error("Login thất bại");

            const data = await res.json();
            if (!data.token || !data.user) throw new Error("Dữ liệu login không hợp lệ");

            // Lưu token + user info
            localStorage.setItem("authToken", data.token);
            localStorage.setItem("currentUser", JSON.stringify(data.user));

            console.log("✅ Login thành công:", data.user);

            // Redirect theo role
            if (data.user.role === "Admin") {
                window.location.href = "/Admin/Index";
            } else {
                window.location.href = "/UserChat";
            }

        } catch (err) {
            console.error("❌ Lỗi login:", err);
            errorDiv.textContent = "Sai username hoặc password";
            errorDiv.classList.remove("d-none");
        }
    });
});
