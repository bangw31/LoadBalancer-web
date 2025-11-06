document.addEventListener("DOMContentLoaded", function () {
    const container = document.getElementById("chatBubbleContainer");
    if (!container) return;

    const btn = document.createElement("button");
    btn.innerHTML = "💬";
    btn.id = "chatToggle";
    btn.style.position = "fixed";
    btn.style.bottom = "20px";
    btn.style.right = "20px";
    btn.style.width = "55px";
    btn.style.height = "55px";
    btn.style.borderRadius = "50%";
    btn.style.background = "#2563eb";
    btn.style.color = "#fff";
    btn.style.border = "none";
    btn.style.fontSize = "26px";
    btn.style.boxShadow = "0 4px 10px rgba(0,0,0,0.2)";
    btn.style.cursor = "pointer";

    container.appendChild(btn);

    btn.addEventListener("click", () => {
        window.location.href = "/Home/UserChat";
    });
});
