
$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadOrderManagement(status);
});

function loadOrderManagement(status) {
    $('#dataTableAdmin').DataTable({
        processing: true,
        serverSide: true,
        filter: true,
        pagingType: "simple_numbers",
        "ajax": {
            url: `/Admin/Order/GetAllOrderHeaders?status=${status}`,
            type: "POST",
        },
        scrollY: '500px',
        scrollCollapse: true,
        "columns": [
            { "data": "id", "width": "7%" },
            { "data": "name", "width": "13%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "user.email", "width": "18%" },
            { "data": "city", "width": "13%" },
            { "data": "orderStatus", "width": "10%" },
            {
                "data": "orderDate",
                "render": function (date) {
                    const formatedDate = new Date(date);
                    return `
                        <span>${formatedDate.toLocaleDateString()}</span>
                    `;
                },
                "width": "12%",
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group d-flex justify-content-center" role="group">
                            <a class="btn btn-primary mx-1" href="/Admin/Order/Detail?orderId=${data}" >
                                <i class="bi bi-pencil-square"></i>
                            </a>
                        </div>
                    `;
                },
                "width": "7%",
            }
        ]
    });
}