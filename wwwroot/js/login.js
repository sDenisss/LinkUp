document.getElementById("loginForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const form = e.target;
    const userData = {
        // UserName: form.userName.value,
        Email: { value: form.email.value },
        Password: form.password.value,
        // UniqueName: form.uniqueName.value
    };

    try {
        const response = await fetch("/api/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(userData)
        });

        if (response.ok) {
            alert("Вход прошел успешно! 🎉");
            window.location.href = "/home.html"; // или chat.html
        } else {
            const error = await response.text();
            alert("Ошибка входа: " + error);
        }
    } catch (err) {
        console.error("Ошибка:", err);
        alert("Ошибка подключения к серверу.");
    }
});

function orRegButton()
{
    window.location.href = "/registry.html";
}