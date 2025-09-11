const pedra = document.getElementById("pedra")
const papel = document.getElementById("papel")
const tesoura = document.getElementById("tesoura")

let escolha = "";

const botoes = document.querySelectorAll(".botao")

pedra.addEventListener("click", function () {
    let adversario = ["pedra", "papel", "tesoura"];
    let indice = Math.floor(Math.random() * adversario.length);
    let escolhaAdversario = adversario[indice];

    let escolha = "pedra";

    if (escolha === "pedra") {
        if (escolhaAdversario === "pedra") {
            alert("Empate!");
        } else if (escolhaAdversario === "papel") {
            alert("Você perdeu!");
        } else if (escolhaAdversario === "tesoura") {
            alert("Você ganhou!");
        }
    }
});
papel.addEventListener("click", function () {
    let adversario = ["pedra", "papel", "tesoura"];
    let indice = Math.floor(Math.random() * adversario.length);
    let escolhaAdversario = adversario[indice];

    let escolha = "papel";

    if (escolha === "papel") {
        if (escolhaAdversario === "pedra") {
            alert("Você ganhou!")
        }
        else if (escolhaAdversario === "papel"){
            alert("Empate!")
        }
        else if (escolhaAdversario === "tesoura"){
            alert("Você Perdeu!")
        }
    }
})
tesoura.addEventListener("click", function() {
    let adversario = ["pedra", "papel", "tesoura"];
    let indice = Math.floor(Math.random() * adversario.length);
    let escolhaAdversario = adversario[indice];

    let escolha = "tesoura";

    if (escolha === "tesoura") {
        if (escolhaAdversario === "pedra") {
            alert("Você Perdeu!")
        }
        else if (escolhaAdversario === "papel"){
            alert("Você ganhou!")
        }
        else if (escolhaAdversario === "tesoura"){
            alert("Empate!")
        }
    }
})


