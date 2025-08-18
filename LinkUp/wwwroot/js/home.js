document.addEventListener('DOMContentLoaded', () => {
    const chatListElement = document.querySelector('.chats');
    const chatWindowElement = document.getElementById('chatWindow');
    const messagesListElement = document.getElementById('messages');
    const messageInput = document.getElementById('messageInput');
    const sendButton = document.getElementById('sendButton');
    const currentChatNameElement = document.getElementById('currentChatName');
    const myProfileLink = document.getElementById('myProfileLink');
    const logoutButton = document.getElementById('logoutButton');

    let currentChatId = null; // Для отслеживания текущего активного чата
    let currentUserId = null; // Будет получен из JWT (если нужно для отображения "моих" сообщений)
    let signalRConnection = null; // Переменная для SignalR соединения

    const token = localStorage.getItem('jwtToken');

    // --- Инициализация и проверки ---
    if (!token) {
        console.error('JWT Token not found. User might not be logged in.');
        window.location.href = '/login.html';
        return;
    }

    // Если хотите получить ID пользователя из JWT для отображения в "Мой профиль"
    // Или для определения, чьи сообщения текущего пользователя
    // Нужно декодировать JWT или получить его из ответа логина.
    // Пока оставим его пустым, или предположим, что он придет с /api/Chat/my-chats
    // Или, лучше, получите его при логине и сохраните в localStorage.
    // Пример: localStorage.setItem('userId', data.userId);
    currentUserId = localStorage.getItem('userId'); 
    if (myProfileLink && currentUserId) {
        // fetch user profile data if needed, or just display username if you saved it
        // myProfileLink.textContent = localStorage.getItem('username') || 'Мой профиль';
    }


    // --- Функции для получения данных ---

    async function fetchAndDisplayChats() {
        try {
            const response = await fetch('/api/Chat/my-chats', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const chats = await response.json();
                renderChatList(chats);
            } else if (response.status === 401) {
                console.error('Unauthorized: Invalid or expired token.');
                localStorage.removeItem('jwtToken');
                localStorage.removeItem('userId'); // Очищаем также userId, если сохраняли
                window.location.href = '/login.html';
            } else {
                const errorText = await response.text();
                console.error('Failed to fetch chats:', response.status, errorText);
            }
        } catch (error) {
            console.error('Network error fetching chats:', error);
        }
    }

    async function fetchAndDisplayMessages(chatIdToLoad) {
        if (!chatIdToLoad) return;
        localStorage.setItem('chatId', chatIdToLoad);

        messagesListElement.innerHTML = '<li class="loading-messages">Загрузка сообщений...</li>'; // Индикатор загрузки
        messagesListElement.scrollTop = messagesListElement.scrollHeight; // Прокрутка вниз

        try {
            const response = await fetch(`/api/Message/${chatIdToLoad}/messages`, { // Используем новый маршрут
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                const messages = await response.json();
                renderMessages(messages);
                currentChatId = chatIdToLoad; // Обновляем активный чат
                enableMessageInput(true); // Включаем ввод сообщения

                // Перезапускаем или присоединяемся к SignalR хабу для нового чата
                await setupSignalRConnection(currentChatId);

            } else if (response.status === 401) {
                console.error('Unauthorized: Invalid or expired token.');
                localStorage.removeItem('jwtToken');
                localStorage.removeItem('userId');
                window.location.href = '/login.html';
            } else if (response.status === 403) { // 403 Forbidden - нет доступа к чату
                const errorText = await response.text();
                console.error('Forbidden: No access to this chat:', errorText);
                messagesListElement.innerHTML = '<li class="error-message">Нет доступа к этому чату.</li>';
                enableMessageInput(false);
            }
            else {
                const errorText = await response.text();
                console.error('Failed to fetch messages:', response.status, errorText);
                messagesListElement.innerHTML = '<li class="error-message">Ошибка загрузки сообщений.</li>';
                enableMessageInput(false);
            }
        } catch (error) {
            console.error('Network error fetching messages:', error);
            messagesListElement.innerHTML = '<li class="error-message">Ошибка сети при загрузке сообщений.</li>';
            enableMessageInput(false);
        }
    }

    // --- Функции для отрисовки HTML ---

    function renderChatList(chats) {
        chatListElement.innerHTML = '';
        if (chats && chats.length > 0) {
            chats.forEach(chat => {
                const li = document.createElement('li');
                li.classList.add('chat-item');
                li.dataset.chatId = chat.id;
                li.textContent = chat.name;

                

                li.addEventListener('click', () => {
                    document.querySelectorAll('.chat-item').forEach(item => {
                        item.classList.remove('active');
                    });
                    li.classList.add('active');
                    currentChatNameElement.textContent = chat.name; // Обновляем заголовок чата
                    fetchAndDisplayMessages(chat.id); // Загружаем сообщения для выбранного чата

                    localStorage.setItem('chatName', chat.name)
                });
                chatListElement.appendChild(li);
            });

            // Автоматически выбираем первый чат и загружаем его сообщения
            // if (chats.length > 0) {
            //     const firstChatElement = chatListElement.querySelector('.chat-item');
            //     if (firstChatElement) {
            //         firstChatElement.classList.add('active');
            //         currentChatNameElement.textContent = firstChatElement.textContent;
            //         fetchAndDisplayMessages(firstChatElement.dataset.chatId);
            //     }
            // }

        } else {
            chatListElement.innerHTML = '<li class="no-chats">У вас пока нет чатов.</li>';
        }
    }

    function renderMessages(messages) {
        messagesListElement.innerHTML = ''; // Очищаем список перед отрисовкой
        if (messages && messages.length > 0) {
            messages.forEach(msg => {
                addMessageToDisplay(msg); // Переиспользуем функцию для добавления одного сообщения
            });
        } else {
            messagesListElement.innerHTML = '<li class="no-messages">В этом чате пока нет сообщений.</li>';
        }
        messagesListElement.scrollTop = messagesListElement.scrollHeight; // Прокручиваем вниз
    }

    // Функция для добавления ОДНОГО сообщения в список
    async function addMessageToDisplay(message) {
        const li = document.createElement('li');
        li.classList.add('message-item');
        // Добавляем класс для стилизации "мои" сообщения
        if (currentUserId && message.senderId === currentUserId) {
            li.classList.add('my-message');
        } else {
            li.classList.add('other-message');
        }

        const senderSpan = document.createElement('span');
        senderSpan.classList.add('message-sender');

        const response = await fetch(`/api/User/unique-name-by-id/${message.senderId}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}` // обязательно!
            }
        });

        if (response.ok) {
            const senderName = await response.text(); // <-- потому что строка
            message.senderName = senderName;
        } else {
            message.senderName = 'Неизвестно';
        }

        // В идеале, здесь нужно будет получить имя отправителя,
        // пока используем ID или заглушку.
        // Для отображения имени пользователя можно:
        // 1. Возвращать имя отправителя в MessageDto
        // 2. Сделать дополнительный запрос на получение имени пользователя
        // senderSpan.textContent = message.senderName || `Пользователь ${message.senderId.substring(0, 8)}:`; // Отображаем имя или часть ID
        senderSpan.textContent = message.senderName; // Отображаем имя или часть ID

        const contentDiv = document.createElement('div');
        contentDiv.classList.add('message-content');
        contentDiv.textContent = message.content;

        const timestampSpan = document.createElement('span');
        timestampSpan.classList.add('message-timestamp');
        timestampSpan.textContent = new Date(message.timestamp).toLocaleTimeString(); // Форматируем время

        li.appendChild(senderSpan);
        li.appendChild(contentDiv);
        li.appendChild(timestampSpan);
        messagesListElement.appendChild(li);
        messagesListElement.scrollTop = messagesListElement.scrollHeight; // Прокручиваем вниз
    }

    function enableMessageInput(enable) {
        messageInput.disabled = !enable;
        sendButton.disabled = !enable;
        if (enable) {
            messageInput.focus();
        }
    }

    EnterOnChatProfile.addEventListener('click', () => {
        if (signalRConnection && signalRConnection.state !== signalR.HubConnectionState.Disconnected) {
            signalRConnection.stop();
        }
        window.location.href = '/profileChat.html';
    });

    CreateChat.addEventListener('click', () => {
        if (signalRConnection && signalRConnection.state !== signalR.HubConnectionState.Disconnected) {
            signalRConnection.stop();
        }
        window.location.href = '/createChat.html';
    });


    // --- SignalR Логика ---
    async function setupSignalRConnection(chatIdToJoin) {
        // Если соединение уже установлено, просто присоединяемся к новому чату
        if (signalRConnection && signalRConnection.state === signalR.HubConnectionState.Connected) {
            console.log(`SignalR: Leaving old chat (${currentChatId}), joining new chat (${chatIdToJoin})`);
            // Можно добавить метод LeaveChat на хабе, если необходимо
            await signalRConnection.invoke("JoinChat", chatIdToJoin);
            return;
        }

        // Создаем новое соединение SignalR
        signalRConnection = new signalR.HubConnectionBuilder()
            .withUrl("/chatHub", { accessTokenFactory: () => token }) // Передаем JWT токен
            .build();

        // Обработчик получения нового сообщения
        signalRConnection.on("ReceiveMessage", (chatId, senderId, senderName, content, timestamp) => {
            // Если сообщение пришло для текущего активного чата, отображаем его
            if (chatId === currentChatId) {
                addMessageToDisplay({
                    id: crypto.randomUUID(), // SignalR не возвращает ID сообщения, генерируем для отображения
                    chatId: chatId,
                    senderId: senderId,
                    senderName: senderName, // Если хаб передает имя отправителя
                    content: content,
                    timestamp: timestamp
                });
            }
        });

        // Обработчик присоединения к чату (опционально, для отладки)
        signalRConnection.on("ChatJoined", (chatId) => {
            console.log(`Successfully joined chat ${chatId} via SignalR.`);
        });

        // Запуск соединения
        try {
            await signalRConnection.start();
            console.log("SignalR connected!");
            await signalRConnection.invoke("JoinChat", chatIdToJoin); // Присоединяемся к чату
        } catch (err) {
            console.error("SignalR connection error:", err);
            alert("Ошибка подключения к чату в реальном времени.");
        }
    }

    // --- Отправка сообщений ---
    sendButton.addEventListener("click", async () => {
        const content = messageInput.value.trim();

        console.log("Content:", content); // Check if message is empty
        console.log("currentChatId:", currentChatId); // Check its value
        console.log("currentUserId:", currentUserId); // Check its value

        if (content === "" || !currentChatId || !currentUserId) {
            console.log("Message sending aborted: One of the required values is missing.");
            return;
        }

        const messageData = {
            chatId: currentChatId,
            senderId: currentUserId,
            content: content
        };

        // Оптимизация: Сначала отображаем сообщение на UI, затем отправляем на сервер
        // Добавляем сообщение сразу, предполагая успех, чтобы UI был отзывчивым
        // addMessageToDisplay({
        //     id: crypto.randomUUID(),
        //     chatId: currentChatId,
        //     senderId: currentUserId,
        //     content: content,
        //     timestamp: new Date().toISOString() // Используем текущее время
        // });
        messageInput.value = ""; // Очищаем поле ввода сразу

        try {
            // 1. Отправляем через SignalR (для других пользователей в чате)
            if (signalRConnection && signalRConnection.state === signalR.HubConnectionState.Connected) {
                await signalRConnection.invoke("SendMessageToChat", messageData.chatId, messageData.senderId, messageData.content);
            } else {
                console.warn("SignalR not connected, message will only be saved to DB.");
            }

            // 2. Отправляем в контроллер API, чтобы сохранить в БД
            const response = await fetch("/api/sendMessage", { // Убедитесь, что ваш API имеет этот маршрут
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}` // Важно: JWT для сохранения
                },
                body: JSON.stringify(messageData)
            });

            if (!response.ok) {
                console.error("Ошибка при сохранении сообщения через API:", await response.text());
                // В случае ошибки сохранения, можно добавить UI-индикатор ошибки
            }
        } catch (err) {
            console.error("Ошибка отправки сообщения:", err);
            // В случае сетевой ошибки
        }
    });

    // Добавьте обработчик для Enter в поле ввода
    messageInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            sendButton.click();
        }
    });

    // --- Logout ---
    logoutButton.addEventListener('click', () => {
        localStorage.removeItem('jwtToken');
        localStorage.removeItem('userId');
        if (signalRConnection && signalRConnection.state !== signalR.HubConnectionState.Disconnected) {
            signalRConnection.stop();
        }
        window.location.href = '/login.html';
    });



    // --- Начальная загрузка ---
    fetchAndDisplayChats();
});