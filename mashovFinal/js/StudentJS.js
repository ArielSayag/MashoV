function f1() {

    return false;

}
var getuser;
$(document).ready(function () {

    $("#wrapped").submit(f1);



    $("#myInput1").on("keyup", function () {
        var value = $(this).val();
        console.log(this);
        $('#result > div').hide();

        $('#result > div > div > h3:contains("' + value + '")').closest('#result > div').show();
        $('#result > div > div > p:contains("' + value + '")').closest('#result > div').show();


    });


    loginUser = JSON.parse(localStorage["Student"]);
    
    localStorage["update"] = JSON.stringify(loginUser);


    $("#hello").append("שלום, " + loginUser.FirstName);
    dashboard(loginUser.Id);
    //ajaxCall("GET", "./api/Doc", "", GETSuccessDep, GETErrorDep); // show all Dep
    //ajaxCall("GET", "./api/Doc/Year", "", GETSuccessYear, GETErrorYear); // show all hebYear

});
//-------------dashboard(getUser)-----------------------------------//
function dashboard(getuser) {

    document.getElementById("result").innerHTML = "";
    ajaxCall("GET", "./api/Student/" + getuser,"", GETSuccess, GETError); // show all ready feeds
}
var str = "";
var groupAll = [];
var all = [];
function GETSuccess(data) {
 
    all = data;
    str = ""

    for (i in data) {
       
        groupAll[i] = data[i].Group;
        var group = data[i].Group;
        var details = data[i].FeedBackMeet.DetailsCourseDep;
        var team = data[i].Group.ListStudent;
        str += `<div id="${i}" class="col-md-4 col-sm-6" onclick="showD(this.id)">
            <div class="service_boxJ">
             <div class="service_icon"></div>
              <h3>${details.NameCourse}</h3>
              <p><b>מחלקה :</b>${details.NameDepartment}</p>
              <p><b>שנה :</b> ${data[i].FeedBackMeet.YearMeeting}</p>
              <p><b>שם הפרויקט:</b> ${group.NameProject}</p>
              <p><b>הארגון :</b> ${group.NameOrganization}</p>
              
              <hr>
              <p><b>חברי הקבוצה :</b></p>`;
        for (var i = 0; i < team.length; i++) {
            str += `<p><b>${team[i].FirstName} ${team[i].LastName}</b></p>`;
        }
         str+=  `</div></div>`;
    }
    $("#result").append(str);

}
function GETError(err) {
    console.log(err);
}

function showD(index) {
    console.log(groupAll[index]);
    console.log(all[index]);
    ajaxCall("PUT", "./api/Student/" + loginUser.Id, JSON.stringify(all[index]), PutSuccess, PutError); 
}
var dataGroupMeet = [];
function PutSuccess(data) {
    
    document.getElementById("allMeet").innerHTML = "";
    $("#first-slide").hide();
    $("#second-slide").fadeIn();
    var str1 = "";
    for (i in data) {
        var idg = data[i].Group.NumGroup;
        var dataGroup = data[i].Group;
        var dataMeet = data[i].FeedBackMeet;
        var idMeet = "selected" + i;

        str1 += `<div id="${idMeet}" class="col-md-3 col-sm-6" >
                       <div class="service_boxJ">`;
        if (data[i].Sum != 0) {
            str1 += `<div id="grade" value="${dataGroup.FinalScore}"  class="service_icon">${dataGroup.FinalScore}</div>`;
        }
        else {
            str1 += `<div id="grade" value="none"  class="service_icon"></div>`;
        }
        str1 += `<h3 id="proj">${dataMeet.NameMeeting}</h3>
                     <p><b id=${dataGroup.Mentor.Email}>מנחה: ${dataGroup.Mentor.FirstName} ${dataGroup.Mentor.LastName}</b></p>
                     <p><b>תאריך: ${dataMeet.Date.split("T")[0]}</b></p>`;
            str1 += `<hr><p><b>שופטים:</b></p>`;
        for (j in data[i].JudgesGroup) {
            str1 += `<div class="row ex2">
                           <div class="col"  onclick="show(this.id,${data[i].Group.NumGroup},${idMeet})" id="${data[i].JudgesGroup[j].Judge.Email}">
                             <p><b>${data[i].JudgesGroup[j].Judge.FirstName}  ${data[i].JudgesGroup[j].Judge.LastName}</b></p>
                           </div>
                           <div class="col">`;
            if (data[i].JudgesGroup[j].SumScore == 0) {
                str1 += `<p><b>-</b></p>`;
            }
            else {
                str1 += `<p><b>${data[i].JudgesGroup[j].SumScore}</b></p>`;
            }
            str1 += `</div>
                  </div>`;
        }
        str1 += `<div class="upload-btn-wrapper">
                   <a class="btn btn-primary1">העלאת מצגת</a>
                   <input class="myfile" type="file" name="myfile" onchange="saveRR()"/>
                </div>
               </div>
             </div>`;

        dataGroupMeet.push(idMeet);
    }
    $('#allMeet').append(str1);
  
}

function PutError(err) {
    console.log(err);
}

function show(judgeID, numG, idDiv) {
    var temp = document.getElementById(idDiv);
    var nameProj = $(temp).find("#proj").val();

    document.getElementById("nameDoc").innerHTML = nameProj;
    //document.getElementById("judgeAndProject").innerHTML = "project name, judge";

    t = {
        "NumType": 3,
        "Type": "Judge",
    }
    u = {
        "Email": judgeID,
        "Type": t,
    }
    ajaxCall("PUT", "./api/Criteria/Group/" + numG, JSON.stringify(u), PUTSuccessTest, PUTErrorTest);
}
var crit1 = "";
var dataT = [];
function PUTSuccessTest(dataTest1) {//all the Meet Doc 
    $("#second-slide").hide();
    $("#third-slide").fadeIn();
    document.getElementById("allCrit").innerHTML = "";

    if (dataTest1.AllCrit.length == 0) {
        $(".visibleElement").css('visibility', 'hidden');

        var str = "<div class='row justify-content-center'>אין מישוב לקבוצה זו כרגע</div>";
        $("#allCrit").append(str);
    }
    else {
        $(".visibleElement").css('visibility', 'visible');
        dataT = dataTest1;
        listTestCrit = [];

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
                                    <p class="form-control" style="height:200px">${dataTest1.AllCrit[i].Note}</p>
                                </div></div><h5 id="numS" value="${dataTest1.AllCrit[i].TypeCrit}">ציון:${dataTest1.AllCrit[i].NameScala}</h5>`;

            crit1 += scalaFunction(dataTest1.AllCrit[i].TypeCrit);

            crit1 += `</div></div>`;

            listTestCrit.push(critID);

        }
        $("#allCrit").append(crit1);
        firstTimeValue();
    }
}
function PUTErrorTest(err) {
    console.log(err);
}


//const SAddapiUrl = `http://proj.ruppin.ac.il/api/Content/AddContent`
//function saveRR() {
//    const AddapiUrl = `${Server_Url}Content/AddContent`

//    fetch(AddapiUrl, {
//        method: 'post',
//        body: JSON.stringify(),
//        headers: new Headers({
//            'Content-Type': 'application/json; charset=UTF-8',
//        })
//    }).then((result) => {
//        console.log('Success:', result.status);
//        if (result.status < 200 && result.status > 300) {
//            result.json().then(data => {
//                alert(data);
//            });
//        }
//        else {//שלב 2 העלאת התוכן עצמו
//            console.log(ContentName)
//            console.log(`${Server_Url}Content/UploadContent/${GlobalUser.Email.split("@", 1)}/${ContentName}`)
//            const UPapiUrl = `${Server_Url}Content/UploadContent/${GlobalUser.Email.split("@", 1)}/${ContentName}`
//            //const SUPapiUrl=`http://proj.ruppin.ac.il/igroup20/prod/api/Content/UploadContent/${GlobalUser.Email.split("@", 1)}/${ContentName}`
//            fetch(UPapiUrl, {
//                method: 'post',
//                body: formData,
//                mode: 'no-cors',
//                contentType: false,
//                processData: false,
//                headers: new Headers({
//                    'Content-Type': 'application/json; charset=UTF-8',
//                })

//            }).then((result) => {
//                return result.json()
//            }
//        }

//    }
//}

/////---------------------select  file--------------------------///
function processSelectedFiles(fileInput) {
    x = fileInput;
    var files = fileInput.files;
    var filesName = "";
    for (var i = 0; i < files.length; i++) {
        filesName = files[i].name;
    }


    var data = new FormData();
    var file = $(".myfile").get(0).files[0];
    data.append("myfile", file);
    data.append("idG", idg);

    $.ajax({
        type: "POST",
        url: "./api/PowerPoint/UploadPowerPoint",
        contentType: false,
        processData: false,
        data: data,
        success: PPSuccess,
        error: PPError
    });
}
function PPSuccess(data) {
    console.log(data);
}
function PPError(err) {
    console.log(err);
}
//----------------------------------------------------------------------------------------------------------------------------------//
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
     
