
$(document).ready(function () {
    loadCompanyTable();
});

function loadCompanyTable() {
    $('#dataTableAdmin').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        pagingType: "simple_numbers",
        "ajax": {
            url: "/Admin/Company/GetAllCompanies",
            type: "POST",
        },
        scrollY: '500px',
        scrollCollapse: true,
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "postalCode", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "id",
                "width": "15%",
                "render": function (id) {
                    return `
                    <div class="btn-group d-flex justify-content-center" role="group">
                        <a class="btn btn-success mx-1" href="/Admin/Company/Upsert/${id}" >
                            <i class="bi bi-pencil-square"></i>
                            Update
                        </a>
                        <a class="btn btn-warning mx-1" onclick="deleteItemInAdmin('/Admin/Company/Delete/${id}')" >
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