
function createExcel(eval) {
    //  $("#excel").toggle();
    var excelIdDoc = localData[eval];
    var x = excelIdDoc.NumDoc;
    localStorage["DataT"] = JSON.stringify(excelIdDoc);
    window.location.href = 'dataTable.html';
   // ajaxCall("POST", "./api/Student", JSON.stringify(excelIdDoc), success, err);

   
}

//function success(data) {
//    window.location.href = 'dataTable.html';

//    try {
//        $('#tbl').DataTable({
//            dom: 'Bfrtip',
//            buttons: [
//                'excel', 'pdf', 'print'
//            ],
//            data: data,

//            pageLength: 20,
//            columns: [
//                //{
//                //    data: "BatchID",
//                //    render: function (data, type, row, meta) {

//                //        let data_for_tbl = "data-btnId='" + data + "'";

//                //        editBtn = "<button type='button' class = 'editBtn btn btn-success'" + data_for_tbl + "> Edit </button>";
//                //        //deleteBtn = "<button type='button' class = 'deleteBtn btn btn-danger'" + data_for_tbl + "> Delete </button>";
//                //        return editBtn /*+ deleteBtn*/;
//                //    }
//                //}, 
//                { data: "FirstName" },
//                { data: "LastName" },
//                { data: "Id" },
//                { data: "FinalScore" },
//                //    data: "Date",
//                //{
//                //    render: function (data) {
//                //        return data.substr(0, 10);
//                //    }
//                //},
//                //{ data: "Tank" },
//                //{
//                //    data: "Tank",
//                //    render: function (data, type, row, meta) {//func to check how much is the batch vol
//                //        if (data > 60)
//                //            return 6000;
//                //        else if (data > 40)
//                //            return 4000;
//                //        else
//                //            return 2000;
//                //    }
//                //},
//                //{ data: "Wort_volume" },
//                //{ data: "Keg20_amount" },
//                //{
//                //    data: "Keg20_amount",
//                //    render: function (data, type, row, meta) {
//                //        // ADD function *20 litter
//                //        return data * 20
//                //    }
//                //},
//                //{ data: "Keg30_amount" },
//                //{
//                //    data: "Keg30_amount",
//                //    render: function (data, type, row, meta) {
//                //        // ADD function *30 litter
//                //        return data * 30
//                //    }
//                //},
//                //{ data: "Bottels_qty" },
//                //{
//                //    data: "Bottels_qty",
//                //    render: function (data, type, row, meta) {
//                //        // ADD function *0.33 litter bottle

//                //        return (data * 0.33).toFixed(2);

//                //    }
//                //},
//                //{
//                //    data: {},
//                //    render: function (data, type, row, meta) {
//                //        return data.Keg20_amount * 20 + data.Keg30_amount * 30 + data.Bottels_qty * 0.33
//                //    }
//                //},

//                //{
//                //    data: "Bottels_qty",
//                //    render: function (data, type, row, meta) {
//                //        // ADD function *24 bottles in box
//                //        return (data / 24).toFixed(0)
//                //    }
//                //},
//                //{
//                //    data: {},
//                //    render: function (data, type, row, meta) {
//                //        return data.Wort_volume - data.Keg20_amount * 20 - data.Keg30_amount * 30 - data.Bottels_qty * 0.33
//                //    }
//                //},
//                //{
//                //    data: {},
//                //    render: function (data, type, row, meta) {

//                //        return ((data.Wort_volume - data.Keg20_amount * 20 - data.Keg30_amount * 30 - data.Bottels_qty * 0.33) / data.Wort_volume).toFixed(2)

//                //    }
//                //},
//            ]
//        });
//    }

//    catch (err) {
//        alert(err);
//    }
//}
//function err(err) {
//    console.log(err);
//}

 //$.ajax({
    //    type: 'Get',
    //    url: '/CreateExcel/PostReportPartial?index=' + x,
    //    data: "",
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (returnValue) {
    //        window.location = '/CreateExcel/Download?file=' + returnValue;
    //    },
    //    error: err
    //});
    //$.ajax({
    //    //type: "GET",
    //    cache: false,
    //    url: '/CreateExcel/PostReportPartial/',
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    data: excelIdDoc,
    //    success: function (data) {
    //        alert("sdfv63vv45v45");
    //        var response = JSON.parse(data);
    //        window.location = '/CreateExcel/Download?fileGuid=' + response.FileGuid
    //            + '&filename=' + response.FileName;
    //    },
    //    error: err
    //});
    //$.ajax({
    //    type: "GET",
    //    url: '/CreateExcel/PostReportPartial/' + eval,
    //    xhrFields: {
    //        responseType: 'blob'
    //    },
    //    success: function (result) {
    //        console.log(result)
    //        alert("sfa");
    //        var blob = result;
    //        var downloadUrl = URL.createObjectURL(blob);
    //        var a = document.createElement("a");
    //        a.href = downloadUrl;
    //        a.download = "downloadFile.xls";
    //        document.body.appendChild(a);
    //        a.click();
    //    }
    //});
    //$.ajax({
    //    type: "POST",
    //    url: './api/CreateExcel', //call your controller and action
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    data: JSON.stringify(excelIdDoc),
    //    success: success,
    //    error: err
    // });
    //}).done(function (data) {

    //    //get the file name for download
    //    if (data.fileName != "") {
    //        //use window.location.href for redirect to download action for download the file
    //        window.location.href = "@Url.RouteUrl(New With {.Controller = \"CreateExcel\", .Action = \"Post\"})/?fileName=" + data.fileName;
    //    }
    //});