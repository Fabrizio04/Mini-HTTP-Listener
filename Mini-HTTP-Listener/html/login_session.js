const form = document.getElementById("testloginsession");

form.addEventListener("submit", (e) => {
    e.preventDefault();

    const request = {
        method: 'POST',
        headers: {
            'Accept': 'application/json, text/plain, */*',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            username: document.getElementById("username").value,
            password: document.getElementById("password").value
        })
    };

    //console.log(request.body);

    fetch('/login.php', request).then(res => res.json())
    .then(res => {

        if(res["Result"] == "ok"){
            location.reload();
        }
        else{
            document.getElementById("message").textContent = 'Username or Password wrong!';
            document.getElementById("message").style.color = "red";
        }
            
        console.log(res);
    });

});