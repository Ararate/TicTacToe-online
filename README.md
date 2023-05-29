# TicTacToe online
Web API, позволяющее играть в игру крестики-нолики по сети

## Конечные точки

## Player

## /Register
Зарегистрировать аккаунт
|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|POST|application/json|application/json|Нет|

#### Параметры
|Наименование|Тип|Обязательный|Ограничение
|-|-|-|-|
|name|string|Да|2-100 символов|
|password|string|Да|6-100 символов|

#### Пример

Запрос:

    fetch("Player/Register", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                "name": "Ararate",
                "password": "Ararate"
            })
        });
        
## /Login
Войти в аккаунт и получить токены
|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|POST|application/json|application/json|Нет|

#### Параметры
|Наименование|Тип|Обязательный|Ограничение
|-|-|-|-|
|name|string|Да|2-100 символов|
|password|string|Да|6-100 символов|

#### Пример

Запрос:

    fetch("Player/Login", {
            method: "POST",
            headers: { "Accept": "application/json", "Content-Type": "application/json" },
            body: JSON.stringify({
                "name": "Ararate",
                "password": "Ararate"
            })
        });
Ответ:

     {"accessToken":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQXJhcmF0ZSIsImV4cCI6MTY4NTMwMzc5NywiaXNzIjoiVGljVGFjVG9lQXBpIiwiYXVkIjoiQ2xpZW50In0.dSjb_nNh9yWIdD5kWkK7hiugvopuAoism23uB4DeSS0",
     "refreshToken":"Kj/oWRnQc0O2AJI9mI/0wlWHZUOsEBdGhDjYaKcmZdA="}

## /RefreshTokens
Обновить токены

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|PUT|application/json|application/json|Нет|

#### Параметры
|Наименование|Тип|Обязательный|Ограничение
|-|-|-|-|
|accessToken|string|Да|Нет|
|refreshToken|string|Да|Нет|

#### Пример

Запрос:

    fetch("Player/RefreshTokens", {
                    method: "PUT",
                    headers: { "Accept": "application/json", "Content-Type": "application/json" },
                    body: JSON.stringify({
                        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQXJhcmF0ZSIsImV4cCI6MTY4NTMwMzc5NywiaXNzIjoiVGljVGFjVG9lQXBpIiwiYXVkIjoiQ2xpZW50In0.dSjb_nNh9yWIdD5kWkK7hiugvopuAoism23uB4DeSS0",
                        "refreshToken": "Kj/oWRnQc0O2AJI9mI/0wlWHZUOsEBdGhDjYaKcmZdA="
                    })
                });
Ответ:

     {"accessToken":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQXJhcmF0ZSIsImV4cCI6MTY4NTMwNDUwNCwiaXNzIjoiVGljVGFjVG9lQXBpIiwiYXVkIjoiQ2xpZW50In0.oVWeDBVo5arqkho0668x34zGXa07vwv-2WAz1sHRAac",
	 "refreshToken":"DjroWRnQc0O2AJI9mI/0wlWHZUOsEBdGhDjYaKcmZdA="}
	 
# Game

## /Create
Создать игру.

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|POST|application/json|application/json|Да|


#### Пример

Запрос:

    fetch("Game/Create", {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + sessionStorage.getItem("tokenKey")
                }
            });
Ответ:

     "Игра создана, ожидайте соперника"
	 
## /Delete
Удалить игру.

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|DELETE|application/json|application/json|Да|


#### Пример

Запрос:

    fetch("Game/Delete", {
                method: "DELETE",
                headers: { 
				"Accept": "application/json", 
				"Content-Type": "application/json", 
				"Authorization": "Bearer " + sessionStorage.getItem("tokenKey") }
            });
			
## /GetField
Возвращает поле игры.

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|GET|application/json|application/json|Да|


#### Пример

Запрос:

    fetch("Game/GetField", {
                method: "GET",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + sessionStorage.getItem("tokenKey")
                },
            });
Ответ:

     [[' ',' ',' '],['X',' ',' '],[' ','O',' ']]	
	 
## /JoinRoom
Присоединиться к игре. Возвращает имя того кто ходит первым.

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|POST|application/json|application/json|Да|

#### Параметры
|Наименование|Тип|Обязательный|Ограничение|Описание
|-|-|-|-|-|
|HostName|string|Да|Нет|Имя хоста к которому нужно присоединиться

#### Пример

Запрос:

    fetch("Game/JoinRoom", {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization":
                    "Bearer " + sessionStorage.getItem("tokenKey"),
                },
                body: JSON.stringify({"HostName": hostname})
            });
Ответ:

     {"Mover": "Ararate"}	

## /MakeMove
Сделать ход.

|Метод|Принимаемый формат|Возвращаемый формат|Нужна авторизация
|-|-|-|-|
|POST|application/json|application/json|Да|

#### Параметры
|Наименование|Тип|Обязательный|Ограничение|Описание
|-|-|-|-|-|
|X|string|Да|Нет|Координата Х
|Y|string|Да|Нет|Координата Y

#### Пример

Запрос:

    fetch("Game/MakeMove", {
                method: "POST",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": "Bearer " + sessionStorage.getItem("tokenKey")
                },
                body: JSON.stringify({
                    "X": x,
                    "Y": y
                })
            });

# SignalR методы клиента

## GameHub

## /GetGameData(dto)

Вызывается, когда соперник делает ход. В dto содержатся координаты по которым он сходил.

Пример DTO: 

	{"x":"0", "y":"1"}

Пример реализации

	hubConnection.on("GetGameData", (dto) => {
            document.getElementById(dto.x + "-" + dto.y).innerHTML = opponentSymb;
            message.innerHTML = "Сейчас ходит:" + dto.mover;
        });
		
## /FinishGame(dto)

Вызывается, когда игра завершена. В dto содержатся результаты игры.

Пример DTO: 

	{"gameResult":
	{
	"draw":"false", 
	winner:"player2", 
	"host":"Ararate", 
	"guest":"player2"
	}}

Пример реализации

	hubConnection.on("FinishGame", (dto) => {
            if (dto.gameResult.draw) {
                message.innerHTML = "Игра завершена! Ничья";
                return;
            }
            message.innerHTML = "Игра завершена! Победитель:" + dto.gameResult.winner;
            console.log(dto);
        });
		
## /GetGuest(dto)

Вызывается, когда к игре подключается гость. В dto содержится имя того кто ходит первый.

Пример DTO: 

	{"mover":"player2"}

Пример реализации

	hubConnection.on("GetGuest", (dto) => {
            sessionStorage.setItem("Mover", dto.mover);
            window.location.href = "Index.html";
        });
