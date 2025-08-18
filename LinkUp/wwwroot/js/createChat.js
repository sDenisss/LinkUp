document.addEventListener('DOMContentLoaded', () => {
    const chatNameInput = document.getElementById('chatNameInput');
    const createChatButton = document.getElementById('createChatButton');
    const createChatStatusMessage = document.getElementById('createChatStatusMessage');
    const creatorId = localStorage.getItem('userId');

    // Получаем JWT токен из локального хранилища
    const token = localStorage.getItem('jwtToken');

    // Проверка наличия токена
    if (!token) {
        console.error('JWT Token not found. User might not be logged in.');
        createChatStatusMessage.textContent = 'Для создания чата войдите в систему.';
        createChatStatusMessage.classList.add('error'); // Добавим класс для стилизации ошибки
        // Можно перенаправить на страницу логина
        // window.location.href = '/login.html';
        return;
    }

    // Обработчик нажатия кнопки "Создать чат"
    createChatButton.addEventListener('click', async () => {
        const chatName = chatNameInput.value.trim();

        if (chatName === "") {
            createChatStatusMessage.textContent = "Пожалуйста, введите название чата.";
            createChatStatusMessage.classList.remove('success');
            createChatStatusMessage.classList.add('error');
            return;
        }

        createChatStatusMessage.textContent = 'Создание чата...';
        createChatStatusMessage.classList.remove('success', 'error');

        try {
            const response = await fetch('/api/CreateChat', { // Предполагаем такой маршрут API
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}` // Отправляем токен для авторизации
                },
                body: JSON.stringify({ name: chatName, creatorId: creatorId}) // Отправляем название чата
            });

            if (response.ok) {
                const newChat = await response.json(); // Если API возвращает данные нового чата
                createChatStatusMessage.textContent = `Чат "${newChat.name || chatName}" успешно создан!`;
                createChatStatusMessage.classList.remove('error');
                createChatStatusMessage.classList.add('success');
                chatNameInput.value = ''; // Очищаем поле ввода
                console.log('New chat created:', newChat);
                // Можно перенаправить пользователя на главную страницу или в созданный чат
                // setTimeout(() => {
                //     window.location.href = '/home.html';
                // }, 2000);
            } else if (response.status === 401) {
                console.error('Unauthorized: Invalid or expired token.');
                createChatStatusMessage.textContent = 'Необходимо войти в систему. Токен недействителен или просрочен.';
                createChatStatusMessage.classList.remove('success');
                createChatStatusMessage.classList.add('error');
                localStorage.removeItem('jwtToken');
                localStorage.removeItem('userId');
                window.location.href = '/login.html'; // Перенаправляем на логин
            } else {
                const errorText = await response.text();
                console.error('Failed to create chat:', response.status, errorText);
                createChatStatusMessage.textContent = `Ошибка при создании чата: ${errorText || response.statusText}`;
                createChatStatusMessage.classList.remove('success');
                createChatStatusMessage.classList.add('error');
            }
        } catch (error) {
            console.error('Network error creating chat:', error);
            createChatStatusMessage.textContent = 'Сетевая ошибка при создании чата. Проверьте подключение.';
            createChatStatusMessage.classList.remove('success');
            createChatStatusMessage.classList.add('error');
        }
    });
});

// Функция для кнопки "На главную"
function BackHome() {
    window.location.href = '/home.html'; // Укажите правильный путь к вашей главной странице
}