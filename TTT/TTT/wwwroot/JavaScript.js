document.getElementById("reg").addEventListener("click", async e => {
    e.preventDefault();
    document.getElementById("name-error").innerHTML = "";
    document.getElementById("password-error").innerHTML = "";
    document.getElementById("result").innerHTML = "";
    const response = await fetch("Player/Register", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify ({
            "name": document.getElementById("name").value,
            "password": document.getElementById("password").value
        })
    });
    const data = await response.json();
    if (response.ok === true) {
        document.getElementById("result").InnerHTML = "Регистрация завершена успешно";
    }
    else
    {
        if (data?.errors?.Name?.[1])
            document.getElementById("name-error").innerHTML = data.errors.Name[0];
        if (data?.errors?.Password?.[1])
            document.getElementById("password-error").innerHTML = data.errors.Password[0];
        if (data?.errors == undefined)
            document.getElementById("result").innerHTML = data;
    }
});

document.getElementById("login").addEventListener("click", async e => {
    e.preventDefault();
    document.getElementById("name-error").innerHTML = "";
    document.getElementById("password-error").innerHTML = "";
    document.getElementById("result").innerHTML = "";
    const response = await fetch("Player/Login", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            "name": document.getElementById("name").value,
            "password": document.getElementById("password").value
        })
    });
    const data = await response.json();
    if (response.ok === true) {
        document.getElementById("result").innerHTML = data;
    }
    else
    {
        if (data?.errors?.Name?.[1])
            document.getElementById("name-error").innerHTML = data.errors.Name[0];
        if (data?.errors?.Password?.[1])
            document.getElementById("password-error").innerHTML = data.errors.Password[0];
        if (data?.errors == undefined)
            document.getElementById("result").innerHTML = data;
    }
});