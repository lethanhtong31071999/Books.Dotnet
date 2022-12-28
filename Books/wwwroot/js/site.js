// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function deleteItemInAdmin(url) {
    if (url) {
        new swal({
            title: "Are you sure?",
            text: "Once deleted, you will not be able to recover it!",
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
                                $('#dataTableAdmin').DataTable().ajax.reload();
                                new swal(data.message, {
                                    icon: "success",
                                });
                            }
                        }
                    });
                }
            });
    }
}
