async function getTasks() {
    // ���������� ������ � �������� �����
    const response = await fetch("/api/tasks", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    // ���� ������ ������ ���������
    if (response.ok === true) {
        // �������� ������
        const users = await response.json();
        const tasks = document.getElementById("tasks");
        // ��������� ���������� �������� � �������
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
        // ���� ��������� ������, �������� ��������� �� ������
        const error = await response.json();
        console.log(error.message); // � ������� ��� �� �������
    }
}

// �������� ����� ������
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
    editLink.append("��������");
    editLink.addEventListener("click", async () => await getTask(task.id));
    linksP.append(editLink);

    const removeLink = document.createElement("button");
    removeLink.append("�������");
    removeLink.addEventListener("click", async () => {
        await deleteUser(task.id);
    });
    linksP.append(removeLink);

    li.append(linksP);

    return li;
}

// ����� �������� �����
document.getElementById("resetBtn").addEventListener("click", () => reset());

// �������� �����
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

// ���������� ������������
async function createTask(newContent, heading, date, priority) {
    if (priority == "")
        priority = null;
    const response = await fetch("api/tasks", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            heading: heading,
            content: newContent,
            date: date,
            priority: priority
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

// ��������� ������������
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

// �������� ������
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

// ����� ������ ����� ����� ��������
function reset() {
    document.querySelectorAll('.mainForm input').forEach((item) => {
        item.value = "";
    });
}

getTasks();
