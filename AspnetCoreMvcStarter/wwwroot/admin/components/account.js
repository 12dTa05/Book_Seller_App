/**
 * Page User List
 */

'use strict';

$(function () {
  let borderColor, bodyBg, headingColor;

  if (isDarkStyle) {
    borderColor = config.colors_dark.borderColor;
    bodyBg = config.colors_dark.bodyBg;
    headingColor = config.colors_dark.headingColor;
  } else {
    borderColor = config.colors.borderColor;
    bodyBg = config.colors.bodyBg;
    headingColor = config.colors.headingColor;
  }

  // Variable declaration for table
  var dt_user_table = $('#dt-user'),
    select2 = $('.select2');

  // Users datatable
  if (dt_user_table.length) {
    var dt_user = dt_user_table.DataTable({
      processing: true,
      serverSide: true,
      ajax: {
        url: '/admin/account/getaccountpagination',
        type: 'POST',
        dataType: 'json',
        data: function (d) {
          return d;
        }
      },
      columns: [
        {
          data: 'id', // Lấy ID của user để sử dụng trong đường dẫn
          name: 'avatar',
          orderable: false,
          searchable: false,
          render: function (id) {
            const avatarUrl = `/img/user/${id}.jpg`; // Đường dẫn ảnh dựa trên ID
            return `
                <img 
                    src="${avatarUrl}" 
                    class="rounded-circle" 
                    width="40" 
                    height="40" 
                    onerror="this.onerror=null;this.src='/img/no-image.png';" 
                    alt="Avatar">
            `;
          }
        },
        { data: 'username', name: 'username', title: 'Tên đăng nhập' },
        { data: 'fullname', name: 'fullname', title: 'Họ và tên' },
        { data: 'email', name: 'email', title: 'Email' },
        { data: 'phone', name: 'phone', title: 'Số điện thoại' },
        {
          data: 'isActive',
          name: 'isActive',
          title: 'Trạng thái',
          render: function (data) {
            return `<span class="badge rounded-pill ${data ? 'bg-success' : 'bg-secondary'}">
                    ${data ? 'Hoạt động' : 'Không hoạt động'}
                </span>`;
          }
        },
        {
          data: 'id',
          name: 'actions',
          orderable: false,
          searchable: false,
          title: 'Hành động',
          render: function (data) {
            return `
        <div class="d-flex gap-2">
            <button class="btn btn-sm btn-primary btn-edit" data-id="${data}">Sửa</button>
            <button class="btn btn-sm btn-danger btn-delete" data-key="${data}">Xóa</button>
            
        </div>`;
          }


        }
      ],



      

      dom:
        '<"row"' +
        '<"col-md-2"<"dt-action-buttons mt-5 mt-md-0"B>>' +
        '<"col-md-10"<"d-flex align-items-center justify-content-md-end"<"me-4"f><"add-new">>>' +
        '>t' +
        '<"row"' +
        '<"col-sm-12 col-md-6"i>' +
        '<"col-sm-12 col-md-6"p>' +
        '>',
      language: {
        search: '',
        searchPlaceholder: 'Tìm kiếm người dùng',
        paginate: {
          next: '<i class="ri-arrow-right-s-line"></i>',
          previous: '<i class="ri-arrow-left-s-line"></i>'
        },
        info: "Hiển thị _START_ đến _END_ của _TOTAL_ bản ghi",
        lengthMenu: "Hiển thị _MENU_ bản ghi"
      },
      buttons: [
        {
          extend: 'collection',
          className: 'btn btn-outline-secondary dropdown-toggle waves-effect',
          text: '<i class="ri-upload-2-line ri-16px me-2"></i>Xuất file',
          buttons: [
            {
              extend: 'excel',
              text: '<i class="ri-file-excel-line me-2"></i>Excel',
              className: 'dropdown-item'
            },
            {
              extend: 'pdf',
              text: '<i class="ri-file-pdf-line me-2"></i>PDF',
              className: 'dropdown-item'
            },
            {
              extend: 'print',
              text: '<i class="ri-printer-line me-2"></i>In',
              className: 'dropdown-item'
            }
          ]
        }
      ],
      responsive: {
        details: {
          display: $.fn.dataTable.Responsive.display.modal({
            header: function (row) {
              return 'Chi tiết người dùng';
            }
          }),
          type: 'column',
          renderer: function (api, rowIdx, columns) {
            const data = $.map(columns, function (col, i) {
              return col.title !== '' ?
                '<tr>' +
                '<td>' + col.title + ':</td> ' +
                '<td>' + col.data + '</td>' +
                '</tr>' :
                '';
            }).join('');

            return data ?
              $('<table class="table"/>').append(data) :
              false;
          }
        }
      }
    });
    // Lọc theo trạng thái hoạt động
    $('#FilterStatus').on('change', function () {
      const val = $(this).val();
      const table = dt_user_table.DataTable();
      table.column(6).search(val).draw();
    });
    // Add new button
    $('.add-new').html(
      "<button class='btn btn-primary waves-effect waves-light' data-bs-toggle='offcanvas' data-bs-target='#offcanvasAddUser'><i class='ri-add-line me-0 me-sm-1 d-inline-block d-sm-none'></i><span class= 'd-none d-sm-inline-block'> Thêm mới </span ></button>"
    );

    

  }
  $(document).ready(function () {
    $('#addNewUserForm').on('submit', function (e) {
      e.preventDefault(); // Ngăn hành vi mặc định của form

      const formData = new FormData(this);

      $.ajax({
        url: $(this).attr('action'), // URL lấy từ thuộc tính action của form
        type: $(this).attr('method'), // Method lấy từ thuộc tính method của form
        data: formData,
        processData: false,
        contentType: false,
        headers: {
          'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
          if (response.success) {
            // Sử dụng Toastr để hiển thị thông báo thành công
            toastr.success(response.message, 'Thành công');
            $('#dt-user').DataTable().ajax.reload(); // Tải lại bảng dữ liệu
            $('#offcanvasAddUser').offcanvas('hide'); // Ẩn offcanvas
            // Reset form
            $('#addNewUserForm')[0].reset();
            $('#img-avatar').attr('src', '/img/no-image.png');
            $('.select2').val(null).trigger('change');
          } else {
            toastr.success(response.message, 'Thành công');
            $('#dt-user').DataTable().ajax.reload(); // Tải lại bảng dữ liệu
            $('#offcanvasAddUser').offcanvas('hide'); // Ẩn offcanvas
            // Reset form
            $('#addNewUserForm')[0].reset();
            $('#img-avatar').attr('src', '/img/no-image.png');
            $('.select2').val(null).trigger('change');
          }
        },
        error: function () {
          // Sử dụng Toastr để hiển thị thông báo lỗi
          toastr.error('Đã xảy ra lỗi khi lưu tài khoản.', 'Lỗi');
        }
      });
    });
  });
  // Delete Record
  $(document).on('click', '.btn-delete', function (e) {
    e.preventDefault();
    const key = $(this).data('key');

    if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
      $.ajax({
        url: `/admin/account/delete/${key}`,
        dataType: 'json',
        method: 'POST',
        success: function (response) {
          if (!response) {
            showToaster("Lỗi", "Xóa thất bại");
            return;
          }
          dt_user.ajax.reload();
          showToaster("Thành công", "Xóa thành công");
        },
        error: function () {
          showToaster("Lỗi", "Đã xảy ra lỗi khi xóa");
        }
      });
    }
  });
  

});
