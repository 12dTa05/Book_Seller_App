function registerDatatable(elementName, columns, urlApi) {
  $(elementName).DataTable({
    scrollY: '300px',
    scrollX: true,
    dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6 d-flex justify-content-center justify-content-md-end"f>>t<"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>>',
    language: {
      paginate: {
        next: '<i class="ri-arrow-right-s-line"></i>',
        previous: '<i class="ri-arrow-left-s-line"></i>'
      }
    },
    processing: true,
    serverSide: true,
    columns: columns,
    ajax: {
      url: urlApi,
      type: 'POST',
      dataType: 'json',
      data: function (d) {
        if (d.start === 0) {
          d.start = 1;
        }
        console.log('Start after adjustment:', d.start);
        return d;
      }


    }
  });
}
