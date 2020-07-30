
function f1() {
    
        return false;

}
var getuser;
        $(document).ready(function () {

        $("#wrapped").submit(f1);

            loginUser = JSON.parse(localStorage["Login"]); //user from login page
          
            var listTT = loginUser.Typesofuser
            for (i in listTT) {
                if (listTT[i].NumType == 3) {
                    t = {
                        "Type": listTT[i].Type,
                        "NumType": listTT[i].NumType,
                    }
                    loginUser.Type = t;
                    localStorage["update"] = JSON.stringify(loginUser);
                    getuser = loginUser;
                }
            }

              console.log(loginUser);
            $("#helloAdmin").append("שלום " + getuser.FirstName + ",");
              dashboard(getuser);


         
           
        });
//-------------dashboard(getUser)-----------------------------------//
        function dashboard(getuser) {

        document.getElementById("resultJ").innerHTML = "";
    ajaxCall("PUT", "./api/User/judge", JSON.stringify(getuser), GETSuccessJ, GETErrorJ); // show all ready feeds
}

function GETSuccessJ(data) {
    localStorage["AllDoc"] = JSON.stringify(data);
    localData = data;
    str = "";
    if (data == "") { return; }
    else {


        for (i in data) {
            str += '<div id="' + data[i].NumMeeting + '" class="col-md-4 col-sm-6 search" style="direction:rtl" onclick="showFeed(this.id,' + data[i].NumDoc + ')"><div class="service_box"><div class="service_icon"></div><h3 value="' + data[i].NameMeeting + '" id="nameMeet">' +
                data[i].NameMeeting + '</h3><p><b>תאריך הצגה :</b>' +
                data[i].Date.split("T")[0] + '</p><p><b>שם הקורס :</b>' +
                data[i].DetailsCourseDep.NameCourse + '</p><p><b>מחלקה :</b>' +
                data[i].DetailsCourseDep.NameDepartment + '</p><p><b>שנה :</b> ' +
                data[i].YearMeeting + '</p></div></div>';
        }

        $('#resultJ').append(str);
    }
}

        function GETErrorJ(err) {
        console.log(err);
    }
idMeet = 0;
idD = 0;
//-----------------------------------------step 2 show all grops--------------------------------//
function showFeed(id,numD) {
    idMeet = id;
    idD = numD;


    var tem = document.getElementById(idMeet);
    var named = $(tem).find("#nameMeet").attr("value");
    $(".meetDoc").html('');
    $(".meetDoc").append(named);
   
    $("#first-slide").hide();
    $("#second-slide").fadeIn();

 
    document.getElementById("addGroups").innerHTML = "";

    ajaxCall("PUT", "./api/User/judge/Groups/" + id, JSON.stringify(getuser), GETSuccessG, GETErrorG);


}
var strGroup = "";
function GETSuccessG(data) { // all my groups that I need to judge after I selected a Meeting
    strGroup = "";
    for (i in data) {
        idforgroup = "group," + data[i].Group.NumGroup;
        strGroup += `<div id="${idforgroup}" class="col-md-3 col-sm-6" >
                       <div class="service_boxJ" >`;
        if (data[i].Sum != 0) {
            strGroup += `<div id="grade" value="${data[i].Sum}"  class="service_icon">${data[i].Sum}
                               </div>`;
        }
        else {
            strGroup += `<div id="grade" value="none"  class="service_icon">
                               </div>`;
        }
        strGroup += `<h3 id="projGroup" onclick="show(this.parentNode.parentNode.id)" value="${data[i].Group.NameProject}">${data[i].Group.NameProject}<br/><br/>${data[i].StartTime}-${data[i].EndTime}</h3>
                     <p><b id="proj">${data[i].Group.NameGroup}</b> </p>`;
        for (j in data[i].Group.ListStudent) {
            strGroup += `<p><b>${data[i].Group.ListStudent[j].FirstName}  ${data[i].Group.ListStudent[j].LastName}</b></p>`;
        }
        strGroup += `<hr>
        <p><b>מנחה: ${data[i].Group.Mentor.FirstName}  ${data[i].Group.Mentor.LastName}</b></p>
          <br>`;
        var link = data[i].Group.Link;
        if (link != "") {
            strGroup += `<br><a href="${link}" class="btn btn-defult2">הורד מצגת</a><br><br>`;
        }
        strGroup += `</div></div>`;
    }
    $('#addGroups').append(strGroup);


}
function GETErrorG(err) {
    console.log(err);
}

//-------------------------------step 3 show all Survey for group----------------------//
var localGroup;
var newOrNot;
function show(id) {
 
    localGroup = id.split(",")[1];

    var tem = document.getElementById(id);
    newOrNot = $(tem).find("#grade").attr('value');
    
    var nameGroupSelected = $(tem).find("#projGroup").attr('value');
    document.getElementById("selectedGroup").innerHTML = nameGroupSelected;

    $("#allCrit").html('');
    getuser.NumDoc = idD;
    if (newOrNot != "none") {

    
        ajaxCall("PUT", "./api/Criteria/Group/" + localGroup, JSON.stringify(getuser), PUTSuccessTest, PUTErrorTest);
       
    }
    else {
       
        ajaxCall("GET", "./api/Criteria/judge/" + idMeet, "", GETSuccessTest, GETErrorTest);
    }
    $("#second-slide").hide();
    $("#third-slide").fadeIn();


}
        var crit = "";
        listTestCrit = [];
        var dataT;
var count;

function GETSuccessTest(dataTest) {//all the Meet Doc 
    listTestCrit = [];
    dataT = dataTest;
    crit = "";
    //document.getElementById("allCrit").innerHTML = "";
    for (i in dataTest.AllCrit) {
        count = i;
        critID = 'crit' + i;
        crit += `<div id="${critID}" class="row pt-4 box">
    <div class="col-sm-8 mx-auto text-right">
        <h5 id="nameC" name="${dataTest.AllCrit[i].NumCrit}">${dataTest.AllCrit[i].NameCrit}</h5>
        <h6>${dataTest.AllCrit[i].DescriptionCrit}</h6>
        <h5>משקל:${dataTest.AllCrit[i].WeightCrit}%</h5>
        <div class="col-sm-10 text-right">
            <div class="form-group">
                <textarea name="" id="note" class="form-control" placeholder="הערתך.."></textarea>
            </div></div><h5 class="">ציון:${dataTest.AllCrit[i].NameScala}</h5>`;
        crit += scalaFunction(dataTest.AllCrit[i].TypeCrit);
        crit += `</div></div > `;

        listTestCrit.push(critID);
    }
    $("#allCrit").append(crit);
   
}

function GETErrorTest(err) {
    console.log(err);
}

                    //-------------------------------if I select Group NOT at the first time----------------------------//
var crit1="";
function PUTSuccessTest(dataTest1) {//all the Meet Doc 
    console.log(dataTest1);
    dataT = dataTest1;
    listTestCrit = [];
    crit1 = "";
    for (i in dataTest1.AllCrit) {

        count = i;
        critID = 'crit' + i;
        crit1 += `<div id="${critID}" class="row pt-4 ">
                        <div class="col-sm-8 mx-auto text-right">
                            <h5 id="nameC" name="${dataTest1.AllCrit[i].NumCrit}">${dataTest1.AllCrit[i].NameCrit}</h5>
                            <h6>${dataTest1.AllCrit[i].DescriptionCrit}</h6>
                            <h5>משקל:${dataTest1.AllCrit[i].WeightCrit}%</h5>
                            <div class="col-sm-10 text-right">
                                <div class="form-group">
                                    <textarea name="" id="note" class="form-control" style="height:200px">${dataTest1.AllCrit[i].Note}</textarea>
                                </div></div><h5 id="numS" value="${dataTest1.AllCrit[i].TypeCrit}">ציון:${dataTest1.AllCrit[i].NameScala}</h5>`;

        crit1 += scalaFunction(dataTest1.AllCrit[i].TypeCrit);

        crit1 += `</div></div>`;

        listTestCrit.push(critID);

    }
    $("#allCrit").append(crit1);
    firstTimeValue();
}

   
                          
function PUTErrorTest(err) {
    console.log(err);
}
//----------------------------step 4 -save to data--------------------------------------------//

function saveScoreToData(boolianSave) {
   
    for (var i = 0; i < listTestCrit.length; i++) {

        var temp = document.getElementById(listTestCrit[i]);

       
        var note = temp.querySelector("#note").value; //need validation of 500 char
        if (note.includes("'")) {
            note = note.replace("'", "`");
        }
        if (note.includes("\"")) {
            note = note.replace("\"", "`");
        }
        dataT.AllCrit[i].Note = note;

        if (dataT.AllCrit[i].TypeCssName != null) {
            var radios = temp.getElementsByClassName(dataT.AllCrit[i].TypeCssName);
            console.log(radios);
            for (var j = 0; j < radios.length; j++) {
                if (radios[j].checked) {
                    dataT.AllCrit[i].Score = calculationScore(radios[j].value, dataT.AllCrit[i].WeightCrit);
                    dataT.AllCrit[i].ValueCrit = radios[j].id;
                    break;
                }
            }
        }
        else {
            var y = `slider${i}`;
            dataT.AllCrit[i].ValueCrit = y;
        }
    }

    g = {
        "NumGroup": localGroup,
    }

    groupM = {
        "numMeeting": idMeet,
        "Group": g,

    }
    full = {
        "CritDoc": dataT,
        "GroupM": groupM,
        "StatusFull": boolianSave,
        "Sum": sumScore(dataT.AllCrit),
        "judes": getuser,

    }
    if (newOrNot == "none") {
        ajaxCall("POST", "./api/Doc", JSON.stringify(full), POSTSuccessfull, POSTErrorfull);
    }
    else {
        ajaxCall("PUT", "./api/Doc/update", JSON.stringify(full), POSTSuccessfullU, POSTErrorfullU);
    }
}
                   
   //------------------------------Auxiliary functions-------------------------//
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

function sumScore(listScore) {//final score for doc
    var sum = 0;
    for (x in listScore) {
        sum += listScore[x].Score;
    }
    return sum;
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

    var finalScore = calculationScore(prec, dataT.AllCrit[index].WeightCrit);//update the score in dataT
    dataT.AllCrit[index].Score = finalScore;

}
function calculationScore(score, weight) {// score for crit
    var score = (weight / 100) * score;
    return score;

}
                
function POSTSuccessfull(data) {
    swal('הנתונים נשמרו בהצלחה').then(function () {
        location.reload();
        showFeed(idMeet);
        //$("#first-slide").hide();
        //$("#second-slide").fadeIn();
    });

}
function POSTErrorfullU(err) {
    console.log(err);
}
function POSTSuccessfullU(data) {
    swal('הנתונים נשמרו בהצלחה').then(function () {
        location.reload();
        $("#first-slide").hide();
        $("#second-slide").fadeIn();
    });
    //swal('הנתונים נשמרו בהצלחה');
    //dashboard(getuser);

}
function POSTErrorfull(err) {
    console.log(err);
}
var str = "";


function scalaFunction(numScala) { //what Scala type I need
    var j = count;
    if (numScala == 1) {//בוצע או לא בוצע

        dataT.AllCrit[j].TypeCssName = 'customRadioInline';
            str = `<div class="row">
    <div class="col-2 "></div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineNotDone" value="0" name="customRadioInline" class="custom-control-input customRadioInline">
            <label class="custom-control-label text-right" for="customRadioInlineNotDone">לא בוצע</label>
        </div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineNot" value="50" name="customRadioInline" class="custom-control-input customRadioInline">
            <label class="custom-control-label text-right" for="customRadioInlineNot">בוצע חלקית</label>
        </div>
        <div class="custom-control custom-radio custom-control-inline">
            <input type="radio" id="customRadioInlineDone" value="100" name="customRadioInline" class="custom-control-input customRadioInline">
              <label class="custom-control-label text-right" for="customRadioInlineDone">בוצע</label>
        </div>
    </div>`;
        

    }
    else if (numScala == 2) {//כן.לא

        dataT.AllCrit[j].TypeCssName = 'customRadioInline1';
            str = `<div class="row">
                <div class="col-2 "></div>
                <div class="custom-control custom-radio custom-control-inline">
                    <input type="radio" id="customRadioInlineYes" value="100" name="customRadioInline1" class="custom-control-input customRadioInline1">
                        <label class="custom-control-label text-right" for="customRadioInlineYes">כן</label>
                              </div>
                    <div class="custom-control custom-radio custom-control-inline">
                        <input type="radio" id="customRadioInlineNo" value="0" name="customRadioInline1" class="custom-control-input customRadioInline1">
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
                              <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="${tempValue}" min="0" max="100">
                               <span id="score${j}" class="range-slider__value">${tempValue}</span>
                            </div></div>`;
        }
        else {
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                              <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="0" min="0" max="100">
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
                               <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})"  type="range" value="${prec}" min="0" max="10">
                                <span id="score${j}" class="range-slider__value">${prec}</span>
                       </div></div>`;
        }
        else {
            str = `<div class="col-sm-10 col-12">
                             <div style="direction: ltr;" class="range-slider m-auto pb-3">
                               <input id="slider${j}" class="range-slider__range" onchange="selectedValue(this.id,${j})" type="range" value="0" min="0" max="10">
                                <span id="score${j}" class="range-slider__value">0</span>
                       </div></div>`;
        }
    }
    else if (numScala == 5) {//בדיד 1-5

        dataT.AllCrit[j].TypeCssName = 'customRadioInlineUpFive';
        str = `<div class="row"><div class="col-2"></div>`;

        for (var i = 1; i <= 5; i++) {
                str += `<div class="custom-control custom-radio custom-control-inline">
                               <input type="radio" id="customRadioInline${i}" value="${i * 20}" name="customRadioInlineUpFive" class="custom-control-input customRadioInlineUpFive">
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
                        <input type="radio" id="customRadioInlineT${i}" value="${i * 10}" name="customRadioInlineUptoTen" class="custom-control-input customRadioInlineUptoTen">
                        <label class="custom-control-label" for="customRadioInlineT${i}">${i}</label>
                     </div>`;
        }
        str += `</div>`;
    }
    return str;
}
   