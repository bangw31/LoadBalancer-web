$(document).ready(function () {
    // Add todo
    $('#addTodo').click(function () {
        let todoText = $('#todoInput').val().trim();
        if (todoText) {
            let todoItem = `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    ${todoText}
                    <span class="text-danger delete-btn" style="cursor:pointer;">✖</span>
                </li>`;
            $('#todoList').append(todoItem);
            $('#todoInput').val('');
        }
    });

    // Remove todo
    $(document).on('click', '.delete-btn', function () {
        $(this).closest('li').remove();
    });

    // Add by pressing Enter
    $('#todoInput').keypress(function (e) {
        if (e.which === 13) {
            $('#addTodo').click();
        }
    });
});
