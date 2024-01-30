async function getTasks() {
    // отправляет запрос и получаем ответ
    const response = await fetch("/api/tasks", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // если запрос прошел нормально
    if (response.ok === true) {
        // получаем данные
        const users = await response.json();
        const tasks = document.getElementById("tasks");
        // добавляем полученные элементы в таблицу
        users.forEach(user => tasks.append(taskLine(user)));
    }
}

async function getTask(id) {
    const response = await fetch(`/api/tasks/${id}`, {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const task = await response.json();
        document.getElementById("taskId").value = task.id;
        document.getElementById("heading").value = task.heading;
        document.getElementById("content").value = task.content;
        document.getElementById('date').value = task.date;
        document.getElementById('priority').value = task.priority;
    }
    else {
        // если произошла ошибка, получаем сообщение об ошибке
        const error = await response.json();
        console.log(error.message); // и выводим его на консоль
    }
}

// создание блока задачи
function taskLine(task) {

    let li = document.createElement('li');
    li.className = "taskLine";

    let id = document.createElement('input');
    id.setAttribute("type", "hidden");
    id.setAttribute("id", "taskLineId");
    id.value = task.id
    li.append(id);

    li.append(task.priority);
    li.append(document.createElement('br'));


    li.append(task.heading);
    li.append(document.createElement('br'));

    li.append(task.content);
    li.append(document.createElement('br'));

    li.append(task.date)

    const linksP = document.createElement("p");

    const editLink = document.createElement("button");
    editLink.append("Изменить");
    editLink.addEventListener("click", async () => await getTask(task.id));
    linksP.append(editLink);

    const removeLink = document.createElement("button");
    removeLink.append("Удалить");
    removeLink.addEventListener("click", async () => {
        await deleteUser(task.id);
    });
    linksP.append(removeLink);

    li.append(linksP);

    return li;
}

// сброс значений формы
document.getElementById("resetBtn").addEventListener("click", () => reset());

// отправка формы
document.getElementById("saveBtn").addEventListener("click", async () => {

    const id = document.getElementById("taskId").value;
    const heading = document.getElementById("heading").value
    const content = document.getElementById("content").value;
    const date = document.getElementById("date").value;
    const priority = document.getElementById("priority").value;

    if (id === "")
        await createTask(content, heading, date, priority);
    else
        await editTask(id, heading, content, date, priority);
    reset();
});

// Добавление пользователя
async function createTask(newContent, heading, date, priority) {

    const response = await fetch("api/tasks", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            heading: heading,
            content: newContent,
            date: date
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        const tasks = document.getElementById("tasks");
        tasks.append(taskLine(user));
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

// Изменение пользователя
async function editTask(taskId, heading, content, date, priority) {
    const response = await fetch("api/tasks", {
        method: "PUT",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            id: taskId,
            heading: heading,
            content: content,
            date: date,
            priority: priority
        })
    });
    if (response.ok === true) {
        const user = await response.json();
        document.querySelector(`#taskLineId[value='${user.id}']`).parentNode.replaceWith(taskLine(user));
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

// Удаление задачи
async function deleteUser(id) {
    const response = await fetch(`/api/tasks/${id}`, {
        method: "DELETE",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const user = await response.json();
        var task = document.querySelector(`#taskLineId[value='${user.id}']`).parentNode;
        task.remove();
    }
    else {
        const error = await response.json();
        console.log(error.message);
    }
}

// сброс данных формы после отправки
function reset() {
    document.querySelectorAll('.mainForm input').forEach((item) => {
        item.value = "";
    });
}

getTasks();
