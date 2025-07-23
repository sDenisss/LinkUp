// Примерные значения, позже ты будешь получать их после логина
const chatId = "c552d322-208e-4a97-817c-b21065266811"; // замените на свой чат
const senderId = "995decbf-81a5-4fd0-b9ec-1bb523299b1f"; // замените на свой userId



const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub") // настройка должна совпадать с сервером
    .build();

connection.start()
    .then(() => {
        console.log("SignalR connected!");

        // Присоединяемся к чату
        connection.invoke("JoinChat", chatId);
    })
    .catch(err => console.error("SignalR error:", err));
    
document.getElementById("sendButton").addEventListener("click", async () => {
    const content = document.getElementById("messageInput").value.trim();
    if (content === "") return;

    const message = {
        id: crypto.randomUUID(), // ✅ создаём GUID на клиенте
        chatId: chatId,
        senderId: senderId,
        content: content
    };

    // 1. Сначала отправим на сервер через SignalR (реальное время)
    connection.invoke("SendMessageToChat", message.chatId, message.senderId, message.content)
        .catch(err => console.error("SignalR error:", err));

    // 2. Параллельно отправим в контроллер API, чтобы сохранить в БД
    try {
        const response = await fetch("/api/SendMessage", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(message)
        });

        if (!response.ok) {
            console.error("Ошибка при сохранении сообщения через API:", await response.text());
        }
    } catch (err) {
        console.error("Ошибка отправки в контроллер:", err);
    }

    document.getElementById("messageInput").value = "";
});
