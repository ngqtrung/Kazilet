var connection;
var answerAt;
var questionId;
var turn;
var playerAnswer;
var createdAt;

var audioCorrect = new Audio();
audioCorrect.src = "/music/success.mp3";
audioCorrect.preload = 'auto';

var audioIncorrect = new Audio();
audioIncorrect.src = "/music/error.mp3";
audioIncorrect.preload = 'auto';

var audioSubmitted = new Audio();
audioSubmitted.src = "/music/new_answer.mp3";
audioSubmitted.preload = 'auto';

var audioApplause = new Audio();
audioApplause.src = "/music/applause.mp3";
audioApplause.preload = 'auto';

var audioCoinIncrease = new Audio();
audioCoinIncrease.src = "/music/coin_spill.mp3";
audioCoinIncrease.preload = 'auto';


async function getQuestion() {
    try {
        console.log(connection);
    } catch (err) {
        console.error(err.toString());
    }
}

function getStreakView(streak) {
    if (streak == 0) {
        return "+0";
    }
    else if (streak <= 2) {
        return "+50";
    }
    else if (streak <= 6) {
        return "+100";
    }
    else if (streak <= 9) {
        return "+150";
    }
    else {
        return "+250";
    }
}

function setCookie(name, value, seconds) {
    const date = new Date();
    date.setTime(date.getTime() + (seconds * 1000));
    const expires = `expires=${date.toUTCString()}`;
    document.cookie = `${name}=${value}; ${expires}; path=/`;
}

function getCookie(key) {
    const value = document.cookie;

    const cookies = value.split('; ').reduce((acc, cookie) => {
        const [k, v] = cookie.split('=');
        acc[k] = decodeURIComponent(v);
        return acc;
    }, {});

    return cookies[key];
}

function deleteCookie(name) {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
}

function sendAnswer(answer) {
    var AnswerDto = {
        QuestionId: questionId,
        PlayerAnswer: answer,
        Turn: turn,
        AnswerAt: new Date().toISOString(),
        CreatedAt: createdAt
    };
    var json = JSON.stringify(AnswerDto);
    clearInterval(countdownInterval);
    isStart = false;

    document.getElementById("question").style.display = "none";
    document.getElementById("waitresult").style.display = "flex";
    connection.invoke("Answer", json)
        .catch(err => console.error(err));

}




function connect(token) {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7024/signalrServer?token=" + token, {
            accessTokenFactory: () => token,
            skipNegotiation: true,  // skipNegotiation as we specify WebSockets
            transport: signalR.HttpTransportType.WebSockets
        })
        .build();

    connection.start().then(function () {
        connection.on("UserJoined", function (username) {
            console.log(username);
            getAvatars();
            try {
                var playerDiv = document.getElementById("playerDiv");
                if (playerDiv) {
                    var myUsername = document.getElementById("usernameInputText").innerHTML;
                    var totalPlayers = Number(document.getElementById("no-players").innerHTML);

                    let parentDiv = $("<div></div>")
                        .addClass("flex mr-4 items-center flex-row p-3  text-center animate-pulse")
                    let player = $(`
    <div class="relative mr-2 w-10 h-10 overflow-hidden bg-gray-100 rounded-full dark:bg-gray-600">
        <img id=${myUsername == username.username ? `playerLobbyAvatar` : username.username + "avatar"} class="h-12 w-12 rounded-full" src=${username.avatar} />
    </div>
    <div class="text-lg mt-2">
        ${myUsername == username.username ? `${username.username} (You)` : username.username}
    </div>`);
                    if (myUsername == username.username) {
                        document.cookie = `kazilet_gameplay_username=${username.username}`
                    }
                    parentDiv.append(player);
                    playerDiv.append(parentDiv.get(0));
                    document.getElementById("no-players").innerHTML = totalPlayers + 1;
                }
            } catch (error) {
                console.log(error);
            }
        });


        connection.on("UserLeft", function (username) {
            connection.invoke("CheckSubmitted")
                .catch(err => console.error(err));
        });

        connection.on("GetQuestion", function (question) {
            document.getElementById("numberSubmitted").innerHTML = "00 Answer";
            if (isStart == true) {
                count = 4;
                countDisplay.textContent = "Are you ready?";
                audio.play();
            }
            else {
                count = 3;
                countDisplay.textContent = "3";
            }
            isRunning = false;

            createdAt = new Date().toISOString();
            var json = JSON.parse(question);
            questionId = json.QuestionDto.Id;
            document.getElementById("gQuestion").innerHTML = json.QuestionDto.Content;
            for (var i = 0; i < json.gameplayAdds.length; i++) {
                if (json.gameplayAdds[i].Username == getCookie("kazilet_gameplay_username")) {
                    document.getElementById("pointStreak").innerHTML = getStreakView(json.gameplayAdds[i].AnswerStreak);
                    if (json.gameplayAdds[i].AnswerStreak >= 10) {
                        document.getElementById("pointStreak").style.width = "100%";
                    }
                    else {
                        document.getElementById("pointStreak").style.width = (json.gameplayAdds[i].AnswerStreak / 10 * 100) + "%";
                    }
                    countdownValue = json.gameplayAdds[i].TimeLimit;
                    totalTime = json.gameplayAdds[i].TimeLimit;
                    progressBar.innerHTML = countdownValue + 's';
                    progressBar.style.width = "100%";
                    if (!progressBar.classList.contains("bg-green-500")) {
                        progressBar.classList.add("bg-green-500");
                    }
                    if (progressBar.classList.contains("bg-red-600")) {
                        progressBar.classList.remove("bg-red-600");
                    }
                    turn = json.gameplayAdds[i].CurrentQuestion;
                    document.getElementById("questionStatus").innerHTML = json.gameplayAdds[i].CurrentQuestion + " / " + json.gameplayAdds[i].TotalQuestion;
                    document.getElementById("streak").innerHTML = json.gameplayAdds[i].AnswerStreak;
                    document.getElementById("gPoints").innerHTML = json.gameplayAdds[i].Point + " points";
                }
            }
            startCountdown();
            document.getElementById("gAnswers").innerHTML = "";
            for (var i = 0; i < json.QuestionDto.Answers.length; i++) {
                var str = json.QuestionDto.Answers[i].Content;
                var s = "ABCDEFGHIJKLMNOP";
                str = s.charAt(i) + ". " + str;
                var html;
                if (i == 0) {
                    html = `
                <div id="json.QuestionDto.Answers[i].Id" onclick="sendAnswer(${json.QuestionDto.Answers[i].Id})" style="background:#ff3131; height: 100px" class="cursor-pointer relative w-full flip-card text-lg flex justify-center items-center border-2 border-solid border-red-500 shadow rounded-lg" id="flipCard">
                        <!-- Mặt trước -->
                        ${str}
                    </div>`;
                }
                else if (i == 1) {
                    html = `
                <div id="json.QuestionDto.Answers[i].Id" onclick="sendAnswer(${json.QuestionDto.Answers[i].Id})" style="background:#01e32a; height: 100px" class="cursor-pointer relative w-full h-full flip-card text-lg flex justify-center items-center border-2 border-solid border-green-500 shadow rounded-lg" id="flipCard">
                        <!-- Mặt trước -->
                        ${str}
                    </div>`;
                }
                else if (i == 2) {
                    html = `
                <div id="json.QuestionDto.Answers[i].Id" onclick="sendAnswer(${json.QuestionDto.Answers[i].Id})" style="background:#5271ff; height: 100px" class="cursor-pointer relative w-full flip-card text-lg flex justify-center items-center border-2 border-solid border-blue-600 shadow rounded-lg" id="flipCard">
                        <!-- Mặt trước -->
                        ${str}
                    </div>`;
                }
                else if (i == 3) {
                    html = `
                <div id="json.QuestionDto.Answers[i].Id" onclick="sendAnswer(${json.QuestionDto.Answers[i].Id})" style="background:#fe7f00; height: 100px" class="cursor-pointer relative w-full h-full flip-card text-lg flex justify-center items-center border-2 border-solid border-yellow-800 shadow rounded-lg" id="flipCard">
                        <!-- Mặt trước -->
                        ${str}
                    </div>`;
                }
                else {
                    html = `
                <div id="json.QuestionDto.Answers[i].Id" onclick="sendAnswer(${json.QuestionDto.Answers[i].Id})" style="background:#fe7f00; height: 100px" class="cursor-pointer relative w-full h-full flip-card text-lg flex justify-center items-center border-2 border-solid border-yellow-800 shadow rounded-lg" id="flipCard">
                        <!-- Mặt trước -->
                        ${str}
                    </div>`;
                }
                document.getElementById("gAnswers").innerHTML += html;

            }
            document.getElementById("lobby").style.display = "none";
            document.getElementById("ranking").style.display = "none";
            document.getElementById("countdown").style.display = "flex";
        });

        connection.on("Submitted", function (result) {
            audioSubmitted.play();
            var s = (result < 10 ? "0" + result + " Answers" : result + " Answers");
            document.getElementById("numberSubmitted").innerHTML = s;
        });

        connection.on("GetResult", function (resultJson) {
            var result = JSON.parse(resultJson);
            for (var i = 0; i < result.length; i++) {
                if (result[i].Username == getCookie("kazilet_gameplay_username")) {
                    document.getElementById("waitresult").style.display = "none";
                    if (result[i].IsCorrect == true) {
                        audioCorrect.play();
                        document.getElementById("background").style.backgroundColor = "#66BE39"
                        document.getElementById("correctresult").style.display = "flex";
                        document.getElementById("resultStreak").innerHTML = result[i].Streak;
                        document.getElementById("resultPoint").innerHTML = result[i].Score;
                        if (result[i].Place == 1) {
                            document.getElementById("resultPlaceC").innerHTML = "You're in 1st place";
                        }
                        else {
                            document.getElementById("resultPlaceC").innerHTML = "You're in " + result[i].Place + "th place";
                        }
                    }
                    else {
                        audioIncorrect.play();
                        document.getElementById("background").style.backgroundColor = "#FD3355"
                        document.getElementById("incorrectresult").style.display = "flex";
                        if (result[i].Place == 1) {
                            document.getElementById("resultPlaceI").innerHTML = "You're in 1st place";
                        }
                        else {
                            document.getElementById("resultPlaceI").innerHTML = "You're in " + result[i].Place + "th place";
                        }
                    }

                }
            }
        });

        connection.on("GetReport", function (resultJson) {
            document.getElementById("background").style.backgroundColor = "#471b43"

            var result = JSON.parse(resultJson);
            var str = "ABCDEFGHIJKLMNOP";
            var ele = document.getElementById("gReportResult");
            ele.innerHTML = "";
            var total = 0;
            for (var i = 0; i < result.length; i++) {
                total += result[i].No;
            }
            for (var i = 0; i < result.length; i++) {
                var percent = result[i].No / total;
                var html = "";
                if (i == 0) {
                    html = `
                    <div class="flex flex-col items-center">
                <span class="mt-2 text-sm font-semibold">${result[i].No + " (" + (percent * 100).toFixed(1) + "%)"}</span>
                <div  class=" w-16 rounded-t-lg animate-height" style="--target-height: ${300 * percent}px; animation-delay: 0.1s;background:#ff3131"></div>
                <div class="mt-2 text-sm font-semibold flex items-center">
                    <div class="mr-2">${str[i]}</div>
                    <div>
                        <img class="h-4 w-4 ${result[i].IsCorrect == true ? "filter-green" : "filter-red"}" src=${result[i].IsCorrect == true ? "/images/correct.png" : "/images/incorrect.png"} />
                    </div>
                </div>
            </div>`;
                }
                else if (i == 1) {
                    html = `
                    <div class="flex flex-col items-center">
                <span class="mt-2 text-sm font-semibold">${result[i].No + " (" + (percent * 100).toFixed(1) + "%)"}</span>
                <div class="w-16 rounded-t-lg animate-height" style="--target-height: ${300 * percent}px; animation-delay: 0.1s;background:#01e32a"></div>
                <div class="mt-2 text-sm font-semibold flex items-center">
                    <div class="mr-2">${str[i]}</div>
                    <div>
                        <img class="h-4 w-4 ${result[i].IsCorrect == true ? "filter-green" : "filter-red"}" src=${result[i].IsCorrect == true ? "/images/correct.png" : "/images/incorrect.png"} />
                    </div>
                </div>
            </div>
                    `;

                }
                else if (i == 2) {
                    html = `
                    <div class="flex flex-col items-center">
                <span class="mt-2 text-sm font-semibold">${result[i].No + " (" + (percent * 100).toFixed(1) + "%)"}</span>
                <div  class="w-16 rounded-t-lg animate-height" style="--target-height: ${300 * percent}px; animation-delay: 0.1s;background:#5271ff"></div>
                <div class="mt-2 text-sm font-semibold flex items-center">
                    <div class="mr-2">${str[i]}</div>
                    <div>
                        <img class="h-4 w-4 ${result[i].IsCorrect == true ? "filter-green" : "filter-red"}" src=${result[i].IsCorrect == true ? "/images/correct.png" : "/images/incorrect.png"} />
                    </div>
                </div>
            </div>`;

                }
                else if (i == 3) {
                    html = `
                    <div class="flex flex-col items-center">
                <span class="mt-2 text-sm font-semibold">${result[i].No + " (" + (percent * 100).toFixed(1) + "%)"}</span>
                <div class=" w-16 rounded-t-lg animate-height" style="--target-height: ${300 * percent}px; animation-delay: 0.1s;background:#fe7f00"></div>
                <div class="mt-2 text-sm font-semibold flex items-center">
                    <div class="mr-2">${str[i]}</div>
                    <div>
                        <img class="h-4 w-4 ${result[i].IsCorrect == true ? "filter-green" : "filter-red"}" src=${result[i].IsCorrect == true ? "/images/correct.png" : "/images/incorrect.png"} />
                    </div>
                </div>
            </div>
                    `;
                }
                else {
                    html = `
                    <div class="flex flex-col items-center">
                <span class="mt-2 text-sm font-semibold">150</span>
                <div class="bg-yellow-500 w-16 rounded-t-lg animate-height" style="--target-height: ${300 * percent}px; animation-delay: 0.1s;"></div>
                <div class="mt-2 text-sm font-semibold flex items-center">
                    <div class="mr-2">${str[i]}</div>
                    <div>
                        <img class="h-4 w-4 ${result[i].IsCorrect == true ? "filter-green" : "filter-red"}" src=${result[i].IsCorrect == true ? "/images/correct.png" : "/images/incorrect.png"} />
                    </div>
                </div>
            </div>
                    `;
                }
                ele.innerHTML += html;
            }
            document.getElementById("report").style.display = "flex";
            document.getElementById("incorrectresult").style.display = "none";
            document.getElementById("correctresult").style.display = "none";
        });

        connection.on("GetRanking", function (resultJson) {

            oldArr = [];
            newArr = [];
            var result = JSON.parse(resultJson);
            for (var i = 0; i < result.oldRank.length; i++) {
                oldArr.push({
                    avatar: result.oldRank[i].Avatar,
                    username: result.oldRank[i].Username,
                    point: result.oldRank[i].Score
                })
            }
            for (var i = 0; i < result.newRank.length; i++) {
                newArr.push({
                    avatar: result.newRank[i].Avatar,
                    username: result.newRank[i].Username,
                    point: result.newRank[i].Score
                })
            }
            document.getElementById("report").style.display = "none";
            document.getElementById("ranking").style.display = "flex";
            renderTable(oldArr);

            // Sau 2 giây, sẽ cập nhật bảng với dữ liệu mới và thêm animation
            setTimeout(() => {
                updateTableWithAnimation();
            }, 500);
        });

        connection.on("ChangeAvatar", async function (data) {
            console.log(data);
            if (document.getElementById(data.username + "avatar") != null) {
                document.getElementById(data.username + "avatar").src = data.avatar;
            }
            getAvatars();
        });

        connection.on("GetFinalReport", async function (result) {
            audio.src = "";
            audioApplause.play();
            const apiUrl = "https://localhost:7024/api/Gameplay/final-report?code=" + result + "&username=" + getCookie("kazilet_gameplay_username");
            try {
                const response = await fetch(apiUrl, {
                    method: 'GET'
                })
                if (response.ok) {

                    const data = await response.json();
                    document.getElementById("finalPlayerAvatar").src = data.avatar;
                    document.getElementById("gUsername").innerHTML = data.username;
                    document.getElementById("finalResultPoint").innerHTML = data.score;
                    var percentCorrect = data.correctAnswer / data.totalQuestion * 100;
                    document.getElementById("correctBar").style.width = percentCorrect + "%";
                    document.getElementById("correctBar").innerHTML = percentCorrect + "%";
                    document.getElementById("finalPlace").innerHTML = data.place + " / " + data.totalPlayers;
                    document.getElementById("finalCorrect").innerHTML = data.correctAnswer;
                    document.getElementById("finalIncorrect").innerHTML = data.totalQuestion - data.correctAnswer;
                    document.getElementById("finalDuration").innerHTML = data.duration + "s";
                    document.getElementById("finalStreak").innerHTML = data.highestStreak;
                    document.getElementById("detailReport").innerHTML = "";
                    var str = "ABCDEFGHIJKLMNOP";
                    for (var i = 0; i < data.playerResponses.length; i++) {
                        var start = `<div class="flex flex-col bg-white mb-4 border-2 border-solid border-gray-100 shadow rounded-lg p-4"><div class="flex flex-col mr-2" style="width: 100%">`;
                        var html = `<div class="mb-2">
                            ${data.playerResponses[i].questionDto.content}
                        </div><div class="flex flex-col">`;
                        var end = `</div></div></div>`;

                        for (var j = 0; j < data.playerResponses[i].questionDto.answers.length; j++) {
                            if (data.playerResponses[i].questionDto.answers[j].isCorrect == true) {
                                html += `<div style="color: #66BE39">
                                <strong>${str[j]}.&nbsp;</strong>
                                <span>${data.playerResponses[i].questionDto.answers[j].content}</span>
                            </div>`;
                            }
                            else if (data.playerResponses[i].questionDto.answers[j].id == data.playerResponses[i].playerAnswer && data.playerResponses[i].questionDto.answers[j].isCorrect == false) {
                                html += `<div style="color: #FD3355">
                                <strong>${str[j]}.&nbsp;</strong>
                                <span>${data.playerResponses[i].questionDto.answers[j].content}</span>
                            </div>`;
                            }
                            else {
                                html += `<div>
                                <strong>${str[j]}.&nbsp;</strong>
                                <span>${data.playerResponses[i].questionDto.answers[j].content}</span>
                            </div>`;
                            }
                        }
                        document.getElementById("detailReport").innerHTML += (start + html + end);
                    }
                    document.getElementById("finalRankingTbody").innerHTML = "";
                    for (var i = 0; i < data.finalRanking.length; i++) {
                        var image = "";
                        if (i == 0) {
                            image = "/images/first.png";
                        }
                        else if (i == 1) {
                            image = "/images/second.png";
                        }
                        else if (i == 2) {
                            image = "/images/third.png";
                        }
                        var html = `<tr>
                                <td width="100px">${i + 1}</td>
                                <td width="400px">
                                    <div class="flex items-center flex-row text-center animate-pulse">
                                        <div class="relative mr-2 w-10 h-10 overflow-hidden bg-gray-100 rounded-full dark:bg-gray-600">
                                            <img class="h-12 w-12 rounded-full" src=${data.finalRanking[i].avatar} />

                                        </div>
                                        <div class="text-lg text-left mr-4">
                                            ${data.finalRanking[i].username}
                                        </div>
                                        <div class="mt-2">
                                            <img src=${image} class="h-8 w-8" />
                                        </div>
                                    </div>
                                </td>
                                <td width="200px">
                                    <div class="flex items-center justify-start rounded-full">
                                        <div class="h-6 w-6 mr-2">
                                            <img src="/images/coin.png" />
                                        </div>
                                        <div style="color: yellow" class="font-bold" >
                                            ${data.finalRanking[i].score} points
                                        </div>
                                    </div>
                                </td>
                            </tr>`;
                        document.getElementById("finalRankingTbody").innerHTML += html;
                    }
                    document.getElementById("ranking").style.display = "none";
                    document.getElementById("background").style.height = "";
                    document.getElementById("final").style.display = "flex";
                    document.getElementById("header").style.display = "block";
                }
            } catch (error) {
                console.log(error);
            }
        });


    }).catch(function (err) {
        return console.error(err.toString());
    });
}