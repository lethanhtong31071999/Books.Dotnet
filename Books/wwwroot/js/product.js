
$(document).ready(function () {
    loadProductTable();
});

function loadProductTable() {
    $('#dataTableAdmin').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        pagingType: "simple_numbers",
        "ajax": {
            url: "/Admin/Product/GetAllProducts",
            type: "POST",
        },
        scrollY: '500px',
        scrollCollapse: true,
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (id) {
                    return `
                    <div class="btn-group d-flex justify-content-center" role="group">
                        <a class="btn btn-success mx-1" href="/Admin/Product/Upsert/${id}" >
                            <i class="bi bi-pencil-square"></i>
                            Update
                        </a>
                        <a class="btn btn-warning mx-1" onclick="deleteItemInAdmin('/Admin/Product/Delete/${id}')" >
                            <i class="bi bi-trash-fill"></i>
                            Delete
                        </a>
                    </div>
                `;
                }
            }
        ]
    });
}