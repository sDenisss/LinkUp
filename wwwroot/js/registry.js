document.getElementById("registerForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const form = e.target;
    const userData = {
        UserName: form.userName.value,
        Email: form.email.value,
        Password: form.password.value,
        UniqueName: form.uniqueName.value
    };

    try {
        const response = await fetch("/api/register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(userData)
        });

        if (response.ok) {
            alert("Регистрация прошла успешно! 🎉");
            window.location.href = "/home.html"; // или chat.html
        } else {
            const error = await response.text();
            alert("Ошибка регистрации: " + error);
        }
    } catch (err) {
        console.error("Ошибка:", err);
        alert("Ошибка подключения к серверу.");
    }
});

function orLoginButton()
{
    window.location.href = "/login.html";
}