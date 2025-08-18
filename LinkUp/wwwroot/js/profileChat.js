// Получаем chatId из query-параметров (?chatId=...)
const urlParams = new URLSearchParams(window.location.search);
const chatId = localStorage.getItem("chatId");
const chatName = localStorage.getItem("chatName");

// DOM элементы
const chatNameEl = document.getElementById("chatName");
const userList = document.getElementById("userList");
const input = document.getElementById("uniqueNameInput");
const message = document.getElementById("addUserMessage");

const token = localStorage.getItem("jwtToken"); // или как ты его хранишь


async function loadChatData() {
  if (!chatId) {
    alert("Не удалось получить chatId из URL.");
    return;
  }

  try {
    // Подгружаем участников чата
    const response = await fetch(`/api/Chat/${chatId}/users`, {
    headers: {
        "Authorization": `Bearer ${token}`
    }
    });

    if (!response.ok) {
      throw new Error("Ошибка при загрузке пользователей");
    }

    const users = await response.json();

    // Очищаем список и заново наполняем
    userList.innerHTML = "";
    users.forEach(user => {
      const li = document.createElement("li");
      li.textContent = user.uniqueName;
      userList.appendChild(li);
    });

    chatNameEl.textContent = `Чат #${chatName}...`;
    // chatNameEl.textContent = `Чат #${chatName.substring(0, 25)}...`;
  } catch (err) {
    console.error(err);
    message.textContent = "Ошибка загрузки участников.";
    message.style.color = "red";
  }
}

async function addUser() {
  const newUserUniqueName = input.value.trim();

  if (!newUserUniqueName) {
    message.textContent = "Введите уникальное имя.";
    message.style.color = "red";
    return;
  }

  try {
    // Получаем ID пользователя по его uniqueName (допустим, через API)
    const userResponse = await fetch(`/api/User/by-unique-name/${newUserUniqueName}`);
    if (!userResponse.ok) throw new Error("Пользователь не найден");
    const user = await userResponse.json();

    // Отправляем POST-запрос на добавление
    const response = await fetch("/api/AddUserToChat", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        chatId: chatId,
        addUserId: user.id
      })
    });

    if (!response.ok) throw new Error("Ошибка добавления пользователя");

    // Обновляем список
    await loadChatData();

    message.textContent = `Пользователь "${newUserUniqueName}" добавлен.`;
    message.style.color = "green";
    input.value = "";
  } catch (err) {
    console.error(err);
    message.textContent = err.message || "Ошибка.";
    message.style.color = "red";
  }
}

function BackHome() {
  window.location.href = "/home.html";
}

// Запускаем при загрузке
window.addEventListener("DOMContentLoaded", loadChatData);
