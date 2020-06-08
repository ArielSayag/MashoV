
function goNextPage(indexNum) {
    var listTT = getuser.Typesofuser;
    var bool = false;

    for (i in listTT) {

        if ((listTT[i].Type == "Mentor") && (indexNum == 4)) {
            bool = true;
            window.location.href = "index4.html"
        }
        if ((listTT[i].Type == "Manager") && (indexNum == 2)) {
            bool = true;
            window.location.href = "index2.html";

        }
        if ((listTT[i].Type == "Judge") && (indexNum == 3)) {
            bool = true;
            window.location.href = "index3.html"
        }

    }
    if (!bool) {
        swal("אינך רשאי להיכנס לדף זה");
    }


}