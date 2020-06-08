function f1() {

    return false;

}
var getuser;
$(document).ready(function () {

    $("#wrapped").submit(f1);

   

    loginUser = JSON.parse(localStorage["Login"]); //user from login page
    //check if manager
    var listTT = loginUser.Typesofuser
    for (i in listTT) {
        if (listTT[i].NumType == 4) {
            t = {
                "Type": listTT[i].Type,
                "NumType": listTT[i].NumType,
            }
            loginUser.Type = t;
            localStorage["update"] = JSON.stringify(loginUser);
            getuser = loginUser;
        }
    }
    
    $("#helloMentor").append("שלום, " + getuser.FirstName);
    dashboard(getuser);
    //ajaxCall("GET", "./api/Doc", "", GETSuccessDep, GETErrorDep); // show all Dep
    //ajaxCall("GET", "./api/Doc/Year", "", GETSuccessYear, GETErrorYear); // show all hebYear

});
//-------------dashboard(getUser)-----------------------------------//
function dashboard(getuser) {

    document.getElementById("result").innerHTML = "";
    ajaxCall("PUT", "./api/User/Mentor", JSON.stringify(getuser), GETSuccessM, GETErrorM); // show all ready feeds
}

function GETSuccessM(data) {
  
    localData = data;
    str = "";
    if (data == "") { return; }
    else {

        console.log(data);

        for (i in data) {
            str += '<div id="' + data[i].NumMeeting + '" class="col-md-4 col-sm-6 search" style="direction:rtl" onclick="showFeed(this.id,'+data[i].NumDoc+')"><div class="service_box"><div class="service_icon"></div><h3 value="' +data[i].NameMeeting+'" id="nameMeet">' +
                data[i].NameMeeting + '</h3><p><b>תאריך הצגה :</b>' +
                data[i].Date.split("T")[0] + '</p><p><b>שם הקורס :</b>' +
                data[i].DetailsCourseDep.NameCourse + '</p><p><b>מחלקה :</b>' +
                data[i].DetailsCourseDep.NameDepartment + '</p><p><b>שנה :</b> ' +
                data[i].YearMeeting + '</p></div></div>';
        }

        $('#result').append(str);
    }
}

function GETErrorM(err) {
    console.log(err);
}
idMeet = 0;
idD = 0;
//-----------------------------------------step 2 show all grops--------------------------------//
function showFeed(id,idDoc) {
    idMeet = id;
    idD = idDoc;
   
    //document.getElementsByClassName("meetDoc").innerHTML = "";
    var tem = document.getElementById(idMeet);
    var named = $(tem).find("#nameMeet").attr("value");
    $(".meetDoc").html('');
    $(".meetDoc").append(named);

    $("#first-slide").hide();
    $("#second-slide").fadeIn();

    document.getElementById("addGroups").innerHTML = "";

    ajaxCall("PUT", "./api/User/Mentor/Groups/" + id, JSON.stringify(getuser), PUTSuccessG, PUTErrorG);


}
var datagroupMentor = [];
function PUTSuccessG(data) { // all my groups that I need to Mentor after I selected a Meeting
    datagroupMentor = data;
    var strGroup = "";
    for (i in data) {
       
        strGroup += `<div  class="col-md-3 col-sm-6"><i class="fa fa-edit editGroup btn" onclick="editGroup(${i})"></i>
                       <div class="service_boxJ">`;
        if (data[i].Sum != 0) {
            strGroup += `<div id="grade" value="${data[i].Sum}"  class="service_icon">${data[i].Sum}</div>`;
        }
        else {
            strGroup += `<div id="grade" value="none"  class="service_icon"></div>`;
        }
        strGroup += `<h3 id="projGroup">${data[i].Group.NameProject}<br/><br/>${data[i].StartTime}-${data[i].EndTime}</h3>
                     <p><b id="proj">${data[i].Group.NameGroup}</b></p>`;

        for (j in data[i].Group.ListStudent) {
            strGroup += `<p><b>${data[i].Group.ListStudent[j].FirstName}  ${data[i].Group.ListStudent[j].LastName}</b></p>`;
        }
        strGroup += `<hr><p><b>שופטים:</b></p>`;
        for (j in data[i].JudgesGroup) {
            strGroup += `<div class="row ex2">
                           <div class="col"  onclick="show(this.id,${data[i].Group.NumGroup},${data[i].Sum},${i})" id="${data[i].JudgesGroup[j].Judge.Email}">
                             <p><b>${data[i].JudgesGroup[j].Judge.FirstName}  ${data[i].JudgesGroup[j].Judge.LastName}</b></p>
                           </div>
                           <div class="col">
                             <p><b>${data[i].JudgesGroup[j].SumScore}</b></p>
                           </div>
                        </div>`;
        }

        var link = data[i].Group.Link;
        if (link != "") {
            strGroup += `<br><a href="${link}" class="btn btn-defult2">הורד מצגת</a><br><br>`;
                   }
        strGroup += `</div></div>`;
    }
    $('#addGroups').append(strGroup);
   

}
function PUTErrorG(err) {
    console.log(err);
}
var total = 0;
function show(judgeID, numG, totalScore,i1) {
    total = totalScore;

    var np = datagroupMentor[i1].Group.NameProject;
 document.getElementById("namegroupselected").innerHTML = "";

    $("#namegroupselected").append(np);
   


    t = {
        "NumType": 3,
        "Type": "Judge",
    }
    u = {
        "Email": judgeID,
        "Type": t,
        "NumDoc": idD,
    }
    ajaxCall("PUT", "./api/Criteria/Group/" + numG, JSON.stringify(u), PUTSuccessTest, PUTErrorTest);
}
var crit1="";
var dataT = [];

function PUTSuccessTest(dataTest1) {//all the Meet Doc 
 $("#second-slide").hide();
    $("#third-slide").fadeIn();

 
    $("#allCrit").html("");
    $("#viewGrade").html(`ציון השופט:  ${total}`);


    if (dataTest1.AllCrit.length == 0) {
        $(".visibleElement").css('visibility', 'hidden');

        var str = "<div class='row justify-content-center'>אין מישוב לקבוצה זו כרגע</div>";
        $("#allCrit").append(str);
    }
    else {
        $(".visibleElement").css('visibility', 'visible');
        dataT = dataTest1;
        listTestCrit = [];
        crit1 = "";
        for (i in dataTest1.AllCrit) {
            var num = parseInt( i) + 1;
            count = i;
            critID = 'crit' + i;
            crit1 += `<div id="${critID}" class="row pt-4 box" disabled>
                        <div class="col-sm-8 mx-auto text-right">${num})
                            <h5 id="nameC" name="${dataTest1.AllCrit[i].NumCrit}">${dataTest1.AllCrit[i].NameCrit}</h5>
                            <h6>${dataTest1.AllCrit[i].DescriptionCrit}</h6>
                            <h5>משקל:${dataTest1.AllCrit[i].WeightCrit}%</h5>
                            <div class="col-sm-10 text-right">
                                <div class="form-group">
                                    <p class="form-control" style="height:200px">${dataTest1.AllCrit[i].Note}</p>
                                </div></div><h5 id="numS" value="${dataTest1.AllCrit[i].TypeCrit}">ציון:${dataTest1.AllCrit[i].NameScala}</h5>`;

            crit1 += scalaFunction(dataTest1.AllCrit[i].TypeCrit);

            crit1 += `</div></div>`;

            listTestCrit.push(critID);

        }
        $("#allCrit").append(crit1);
       
    }
    if (dataTest1.AllCrit.length != 0) {firstTimeValue();}
    
}
function PUTErrorTest(err) {
    console.log(err);
}

//-----------------------when I want to update details gropp------------------------------------//
var editGselected = [];
function editGroup(index) {

    $("#updatePopupM").toggle();
    $("#titleEdit").html('');

     editGselected = datagroupMentor[index];
    var titel = "עדכון פרטי קבוצה:   " + editGselected.Group.NameGroup
    $("#titleEdit").append(titel);
    $("#nameProj").val(editGselected.Group.NameProject);
    $("#nameOrg").val(editGselected.Group.NameOrganization);
    $("#typeProj").val(editGselected.Group.ProjectType);
    $("#startTime").val(editGselected.StartTime);
    $("#endTime").val(editGselected.EndTime);

}

function updateDetails(bool) {


    if (bool) {
      
        group = {
            "NameGroup": editGselected.Group.NameGroup,
            "NumGroup": editGselected.Group.NumGroup,
            "NameProject": $("#nameProj").val(),
            "NameOrganization": $("#nameOrg").val(),
            "ProjectType": $("#typeProj").val(),
            "Mentor": getuser,
            "BeforeLastProg": editGselected.Group.NameProject,
            "BeforeLastOrg": editGselected.Group.NameOrganization,
        }
        gm = {
            "Group": group,
            "StartTime": $("#startTime").val(),
            "EndTime": $("#endTime").val(),
        }
        ajaxCall("PUT", "./api/User/updateGroupMentor", JSON.stringify(gm), successUpdateG, errUpdateG)
    }
 
    $("#updatePopupM").toggle();
}
function successUpdateG(data) {
    console.log(data);
    swal("פרטי הקבוצה עודכנו בהצלחה").then(function () {
        location.reload();
    });
}

function errUpdateG(err) {
    console.log(err);
    swal("פרטי הקבוצה לא עודכנו");
}
//------------------------------------------------help function for print crit on screen-------------------------//
function firstTimeValue() {
    for (var i = 0; i < listTestCrit.length; i++) {

        var temp = document.getElementById(listTestCrit[i]);
        var x = $(temp).find("#numS").attr('value');
        if (x == 3 || x == 4) {
            var y = `slider${i}`;
            selectedValue(y, i);
        }
        else {
            var radios = temp.getElementsByClassName(dataT.AllCrit[i].TypeCssName);

            for (var j = 0; j < radios.length; j++) {
                if (radios[j].id == dataT.AllCrit[i].ValueCrit) {

                    radios[j].checked = true;
                }

            }
        }
    }

}
function selectedValue(id, index) { // value of sliders


    var idScore = 'score' + index;
    var slider = document.getElementById(id);
    var output = document.getElementById(idScore);

    output.innerHTML = slider.value;

    slider.oninput = function () {
        output.innerHTML = this.value;

    }
    var prec;
    if (slider.max == 10) {
        prec = slider.value * 10;
        var val = `linear-gradient(90deg, rgb(26, 188, 156) ${prec}%, rgb(215, 220, 223) ${prec}.1%)`;
    } else {
        prec = slider.value;
        var val = `linear-gradient(90deg, rgb(26, 188, 156) ${slider.value}%, rgb(215, 220, 223) ${slider.value}.1%)`;
    }
    $(slider).css('background', val);

}
function scalaFunction(numScala) { //what Scala type I need
    var j = count;
    if (numScala == 1) {//בוצע או לא בוצע

        dataT.AllCrit[j].TypeCssName = 'customRadioInline';
        str = `<div class="row" >
    <div class="col-2 "></div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineNotDone" readonly value="0" name="customRadioInline" class="custom-control-input customRadioInline" disabled>
            <label class="custom-control-label text-right" for="customRadioInlineNotDone">לא בוצע</label>
        </div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineNot" readonly value="50" name="customRadioInline" class="custom-control-input customRadioInline" disabled>
            <label class="custom-control-label text-right" for="customRadioInlineNot">בוצע חלקית</label>
        </div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineDone" readonly value="100" name="customRadioInline" class="custom-control-input customRadioInline" disabled>
              <label class="custom-control-label text-right" for="customRadioInlineDone">בוצע</label>
        </div>
    </div>`;


    }
    else if (numScala == 2) {//כן.לא

        dataT.AllCrit[j].TypeCssName = 'customRadioInline1';
        str = `<div class="row">
                <div class="col-2 "></div>
                <div class="custom-control custom-radio custom-control-inline">
                    <input type="radio" id="customRadioInlineYes" value="100" name="customRadioInline1" class="custom-control-input customRadioInline1" disabled>
                        <label class="custom-control-label text-right" for="customRadioInlineYes">כן</label>
                              </div>
                    <div class="custom-control custom-radio custom-control-inline">
                        <input type="radio" id="customRadioInlineNo" value="0" name="customRadioInline1" class="custom-control-input customRadioInline1" disabled>
                            <label class="custom-control-label text-right" for="customRadioInlineNo">לא</label>
                        </div>
                    </div>`;
    }
    else if (numScala == 3) {//רציף 1-100
        var temp = `slider${j}`;

        if ((dataT.AllCrit[j].ValueCrit != null) && (dataT.AllCrit[j].ValueCrit == temp)) {
            var tempValue = ((dataT.AllCrit[j].Score * 100) / dataT.AllCrit[j].WeightCrit);
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                              <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="${tempValue}" min="0" max="100" disabled>
                               <span id="score${j}" class="range-slider__value">${tempValue}</span>
                            </div></div>`;
        }
        else {
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                              <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="0" min="0" max="100" disabled>
                               <span id="score${j}" class="range-slider__value">0</span>
                            </div></div>`;
        }
    }
    else if (numScala == 4) {//רציף 1-10
        var temp = `slider${j}`;
        var prec;
        if ((dataT.AllCrit[j].ValueCrit != null) && (dataT.AllCrit[j].ValueCrit == temp)) {
            var tempValue = ((dataT.AllCrit[j].Score * 100) / dataT.AllCrit[j].WeightCrit);
            prec = tempValue / 10;
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                               <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})"  type="range" value="${prec}" min="0" max="10" disabled>
                                <span id="score${j}" class="range-slider__value">${prec}</span>
                       </div></div>`;
        }
        else {
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                               <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="0" min="0" max="10" disabled>
                                <span id="score${j}" class="range-slider__value">0</span>
                       </div></div>`;
        }
    }
    else if (numScala == 5) {//בדיד 1-5

        dataT.AllCrit[j].TypeCssName = 'customRadioInlineUpFive';
        str = `<div class="row"><div class="col-2"></div>`;

        for (var i = 1; i <= 5; i++) {
            str += `<div class="custom-control custom-radio custom-control-inline">
                               <input type="radio" id="customRadioInline${i}" value="${i * 20}" name="customRadioInlineUpFive" class="custom-control-input customRadioInlineUpFive" disabled>
                               <label class="custom-control-label" for="customRadioInline${i}">${i}</label>
                              </div>`;

        }
        str += `</div>`;


    }
    else if (numScala == 6) {//בדיד 1-10

        dataT.AllCrit[j].TypeCssName = 'customRadioInlineUptoTen';
        str = ` <div class="row"><div class="col-2"></div>`;

        for (var i = 1; i <= 10; i++) {

            str += `<div class="custom-control1 custom-radio custom-control-inline">
                        <input type="radio" id="customRadioInlineT${i}" value="${i * 10}" name="customRadioInlineUptoTen" class="custom-control-input customRadioInlineUptoTen" disabled>
                        <label class="custom-control-label" for="customRadioInlineT${i}">${i}</label>
                     </div>`;
        }
        str += `</div>`;
    }
    return str;
}
