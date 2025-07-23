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
            const data = await response.json(); // Парсим JSON-ответ от сервера
            localStorage.setItem('jwtToken', data.token); // Сохраняем токен под ключом 'jwtToken'

            handleLoginSuccess(data);
            
            // alert("Вход прошел успешно! 🎉");

            // const response = await fetch('/home', { 
            //     method: 'GET',
            //     headers: {
            //         'Content-Type': 'application/json',
            //         'Authorization': `Bearer ${token}` // Добавляем JWT токен в заголовок
            //     }
            // });
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

function OnRegButton()
{
    window.location.href = "/registry.html";
}


// Example: In your login success handler (where you process the API response)
async function handleLoginSuccess(data) {
    // Save the JWT token
    localStorage.setItem('jwtToken', data.token);

    // --- IMPORTANT: Extract and save the userId ---
    let userId = null;
    try {
        // Decode the JWT token to get the user ID
        // The JWT is base64-encoded, so we need to decode it.
        // The payload (middle part) contains the claims.
        const tokenPayload = JSON.parse(atob(data.token.split('.')[1]));

        // Common JWT claims for user ID:
        // 'sub' (subject) is the standard claim for the principal (user ID)
        // 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier' is a common .NET claim type
        userId = tokenPayload.sub || tokenPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];

        if (userId) {
            localStorage.setItem('userId', userId);
            console.log("User ID saved to localStorage:", userId);
        } else {
            console.warn("JWT token did not contain a 'sub' or 'nameidentifier' claim for userId.");
        }
    } catch (e) {
        console.error("Failed to decode JWT or extract userId:", e);
    }
    // --- End of userId extraction ---

    // Redirect to home page
    window.location.href = '/home.html';
}