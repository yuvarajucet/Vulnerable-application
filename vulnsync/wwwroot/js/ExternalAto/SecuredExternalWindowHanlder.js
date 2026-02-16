function openPoPUpWindow() {
    const popupWindow = window.open('/secured/oauth/login?state=uiejlkana9u3jsadfbc&code=8393822', 'popupWindow', 'width=600,height=600');
    
    window.addEventListener("message", (event) => {
        if (event.origin !== window.location.origin) return;
        if(event.data != null)
        {
            let userData = JSON.parse(event.data);
            autoSubmitForm(userData);
            popupWindow.close();
        }
    })
}


function autoSubmitForm(data) {

    const form = document.createElement("form");
    form.method = "POST"; 
    form.action = "/secured/externalato/oauth/callback"; 

    form.style.display = "none";

   
    Object.keys(data).forEach(key => {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = key;
        input.value = data[key];
        form.appendChild(input);
    });

    document.body.appendChild(form);

    form.submit();
}


function addNewUser(userData) {
    $.ajax({
        url: '/secured/ExternalAto/oauth/callback',
        type: 'GET',
        data: userData,
        contentType: 'application/json',
    });
}