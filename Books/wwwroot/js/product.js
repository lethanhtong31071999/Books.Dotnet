$(document).ready(function () {
    loadProductTable();
});

function loadProductTable() {
    $('#productTable').DataTable({
        "ajax": {
            url: "/Admin/Product/GetAllProducts",
        },
        scrollY: '500px',
        scrollCollapse: true,
        pagingType: "simple_numbers",
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
                        <a class="btn btn-warning mx-1" onclick="deleteProduct('/Admin/Product/Delete/${id}')" >
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

function deleteProduct(url) {
    if (url != "") {
        new swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover this imaginary file!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!"
        })
            .then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: url,
                        type: "Delete",
                        success: function (data) {
                            if (data) {
                                $('#productTable').DataTable().ajax.reload();
                                new swal(data.message, {
                                    icon: "success",
                                });
                            } else {
                            }
                        }
                    });            
                }
        });
    }
}