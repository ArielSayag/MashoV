
function lastTimeDoc() {
    alert(selectCours + "," + selectDep);

    cd = {
        "NumDepartment": selectDep,
        "NumCourse": selectCours,

    }

    ajaxCall("PUT", "./api/Criteria/Last", JSON.stringify(cd), PUTSuccessLast, PUTErrorLast);
     
}
var strLastDoc = "";
var arrlastDoc = [];
var listLastCrit = [];
function PUTSuccessLast(data) {

    document.getElementById("last").innerHTML = "";
    $("#fifth-slide").fadeIn();
    $("#fourth-slide").hide();
    $("#third-slide").hide();
    
    arrlastDoc = data;
    var docname = "";
    strLastDoc = "";
    for (k in data) {

        docname += `<option value = ${k}>${data[k].Doc.NameDoc},${data[k].Doc.DetailsMeet.YearMeeting} </option>`;
    }

    $('#lastDoc').append(docname);

    strLastDoc += `<div class="box2">`;
    var critLast = data[0].AllCrit;
    for (j in critLast) {
        var strL = "last" + j;
        var count = parseInt(j) + 1;
        strLastDoc += helpstr(strL, count, critLast[j]);
        listLastCrit.push(strL);
    }
    strLastDoc += `</div>`;
  
    $('#last').append(strLastDoc);

}

function PUTErrorLast(err) {
    console.log(err);
}

function editselectedDoc() {
    listLastCrit = [];
    var val = $("#lastDoc").val();
    var critLast = arrlastDoc[val].AllCrit;
    document.getElementById("last").innerHTML = "";
    var strLastDoc = "";
    var strL = "";

    strLastDoc += `<div class="box2">`;
    for (j in critLast) {
        strL = "last" + j;
        var count = parseInt(j) + 1;
        strLastDoc += helpstr(strL, count, critLast[j]);
        listLastCrit.push(strL);
    }
    strLastDoc += `</div>`;
    
    $('#last').append(strLastDoc);
}

function helpstr(strL, count, critLast) {
    var strLast = "";
    strLast = `<div id="${strL}" class="box3"><div class="row pt-5">
                        <div class="col-sm-6 mx-auto">
                        <div  class="form-group">קריטריון מס ${count})
                        <h5 id="fdataname1" class="feedback1-show" name="${critLast.NumCrit}" value="${critLast.NameCrit}">${critLast.NameCrit}</h5>
                        </div>
                        <div class="form-group">
                         <h6 id="fdatadec1" class="feedback1-show" value="${critLast.DescriptionCrit}">תיאור:${critLast.DescriptionCrit}</h6>                       
                        </div>
                        </div>
                        </div>
                        <div class="row pt-4">
                        <div class="col-sm-5 offset-6 m-auto ">
                        <div class="form-group">
                        <h7 id="weight2" class="feedback1-show" value="${critLast.WeightCrit}" >משקל: ${critLast.WeightCrit}%</h7>
                        </div>
                        <div class="form-group">
                        <select class="form-control"  id="scala2">
                        <option  value="${critLast.TypeCrit}">${critLast.NameScala}</option>
                        </select>
                        </div>
                        </div>
                        </div>
                        </div>`;
    return strLast;

}
var listCrit1 = [];

function saveLastDoc() {
    for (i in listLastCrit) {
        var temp = document.getElementById(listLastCrit[i]);

        //tempArr[0] = temp.querySelector("#critname").value; // find the id from all div
        //tempArr[1] = temp.querySelector("#description").value;
        var weight = $(temp).find("#weight2").attr('value');
        var idcrit = $(temp).find("#fdataname1").attr('name');
        var scala = temp.querySelector("#scala2").value;

        //for (k in tempArr) {
        //    tempArr[k] = tempString(tempArr[k]);
        //}

        //if (weight != 0) {

        crit = {
            "NumCrit": idcrit,
            "WeightCrit": weight,
            "typeCrit": scala,
        }
            listCrit1.push(crit);

        //}

    }


    newDoc.Status = true;
    newDoc.TotalWeight = 100;
    critsInDoc = {
        "AllCrit": listCrit1,
        "Doc": newDoc,

    }

    ajaxCall("POST", "./api/Criteria/LastnewDoc", JSON.stringify(critsInDoc), postSuccessCrit, postErrorCrit);
}

function postSuccessCrit(data) {
    console.log(data);
    swal("נתונים הועברו בהצלחה");
    dashboard(getuser); 
    $("#third-slide").hide();
    $("#fourth-slide").hide();
    $("#first-slide").fadeIn(); // back to dash
}

function postErrorCrit(err) {
    console.log(err);

}

function editLadsDocToNew() {

    for (i in listLastCrit) {
        
        var temp = document.getElementById(listLastCrit[i]);

        addCrit[0] = $(temp).find("#fdataname1").attr('value');
        addCrit[1] = $(temp).find("#fdatadec1").attr('value');
        var idc = $(temp).find("#fdataname1").attr('name');


            strAnat = 'anat' + count++;

            straddCrit = `<div class="box" id="${strAnat}">
                                                <div class="row pt-5">
                                                <div class="col-sm-6 mx-auto">
                                                <div id="idc" value="${idc}" class="form-group">קריטריון מס ${count})
                                                <input type="text" class="form-control" id="critname" value="${addCrit[0]}" required>
                                                </div>
                                                <div class="form-group">
                                                <textarea type="text" class="form-control ckeditor"  id="description" >${addCrit[1]}</textarea>
                                                </div>
                                                </div>
                                                </div>
                                                <div class="row pt-4">
                                                <div class="col-sm-5 offset-6 m-auto ">
                                                <div class="form-group">
                                                <select class="form-control" id="weight" onchange="showTotalWeight(${strAnat})">
                                                <option value="0">בחר משקל</option>
                                                <option value="5">5%</option>
                                                <option value="10">10%</option>
                                                <option value="15">15%</option>
                                                <option value="20">20%</option>
                                                <option value="25">25%</option>
                                                <option value="30">30%</option>
                                                <option value="35">35%</option>
                                                <option value="40">40%</option>
                                                <option value="45">45%</option>
                                                <option value="50">50%</option>
                                                </select>
                                                </div>
                                                <div class="form-group">
                                                <select class="form-control" id="scala">
                                                <option value="0">בחר סקאלה</option>
                                                <option value="1">כן/לא</option>
                                                <option value="2">בוצע/לא בוצע</option>
                                                <option value="3">רציף 1-100</option>
                                                <option value="4">1-10 רציף</option>
                                                <option value="5">1-5 בדיד</option>
                                                <option value="6">1-10 בדיד</option>
                                                </select>
                                                </div>
                                                </div>
                                                </div>
                                                <div class="row pt-3">
                                                <div class="col-sm-4 m-auto">
                                                <a class="btn btn-defult text-light m-3 fourth-slide-hide-btn"  onclick="deleteCrit(${strAnat})" href="javascript:void(0)"> מחק</a>
                                                </div>
                                                </div>
                                                <div class="row">
                                                <div class="col-sm-6 mx-auto">
                                                <div class="form-group">
                                                <select class="form-control" onclick="addSecendTime(${strAnat})" id="add-criteria-btn">
                                                <option selected disabled>הוסף קריטריון </option>
                                                <option value="Choose From List">בחר מרשימה</option>
                                                <option value="New">חדש</option>
                                                </select>
                                                </div>
                                                </div>
                                                </div>
                                                </div>
                                                </div>`;


          
        $("#addHere").append(straddCrit);
        console.log(straddCrit);
        listDiv.push(strAnat);
       
    }
 ckReplace();
  $("#fourth-slide").fadeIn();
  $("#fifth-slide").hide();
}


function goback() {
    if (newDocFromExcel == 1) {
        $("#third-slide").fadeIn();
        $("#fourth-slide").fadeIn();
        $("#fifth-slide").hide();
    }
    else {
        $("#fifth-slide").hide();
        $("#fourth-slide").fadeIn();
    }
}