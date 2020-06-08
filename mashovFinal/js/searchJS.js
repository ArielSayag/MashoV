$(document).ready(function () {


    ajaxCall("GET", "./api/Doc", "", GETSuccessDep, GETErrorDep); // show all Dep
    ajaxCall("GET", "./api/Doc/Year", "", GETSuccessYear, GETErrorYear); // show all hebYear
    ajaxCall("GET", "./api/Doc/Cours", "", GETSuccessCours, GETErrorCours); // show all Cours
    ajaxCall("GET", "./api/Doc/Scala", "", GETSuccessScala, GETErrorScala); // show all Scala
   


});

//---------------------show all Dep from drop down list------------------------------------//

function GETSuccessDep(data) {
    console.log(data);
    var arrNamesDep = "<option value=''>בחר מחלקה</option>";

    for (k in data) {

        arrNamesDep += `<option value=${data[k].NameDepartment}>${data[k].NameDepartment} </option>`;
    }
    var arrExcel= "<option value=''>בחר מחלקה</option>";

    for (k in data) {

        arrExcel += `<option value=${data[k].NumDepartment}>${data[k].NameDepartment} </option>`;
    }

    $('#Department').append(arrNamesDep);
    $('#browsers1').append(arrNamesDep);
    $('#DepartmentExcel').append(arrExcel);

}
function GETErrorDep(err) { console.log(err); }
//---------------------show all Courses from drop down list------------------------------------//

function GETSuccessCours(data) {
    console.log(data);
    var arrNamesCours = "<option value=''>בחר קורס</option>";

    for (k in data) {

        arrNamesCours += `<option value = ${data[k].NumCourse}>${data[k].NameCourse} </option>`;
    }

    $('#Course').append(arrNamesCours);


}
function GETErrorCours(err) { console.log(err); }
///------------------------show all year-------------------------------------------------//
function GETSuccessYear(data) {
    console.log(data);
    //var arrNames = "<option value=''>ב</option>";
    var arrNames = [];
    for (k in data) {

        arrNames += `<option value = ${data[k]}>${data[k]} </option>`;
    }

    $('#year').append(arrNames);
}
function GETErrorYear(err) { console.log(err); }
var listScal = [];
///------------------------show all Scala-------------------------------------------------//
function GETSuccessScala(data) {
    console.log(data);
    listScal = data;
    var arrNamesScala = "<option value=''>בחר סקאלה</option>";

    for (k in data) {

        arrNamesScala += `<option value = ${data[k].NumScala}>${data[k].NameScala} </option>`;
    }

    $('#scala').append(arrNamesScala);
    $('#scala1').append(arrNamesScala);
}
function GETErrorScala(err) { console.log(err); }

