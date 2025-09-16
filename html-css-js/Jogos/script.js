//Geral
const pontos = document.getElementById("pontos")
let pontuacao = 0
//Jokenpo

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
            pontuacao-=1;
            pontos.innerText = pontuacao
        } else if (escolhaAdversario === "tesoura") {
            alert("Você ganhou!");
            pontuacao+=1;
            pontos.innerText = pontuacao
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
            pontuacao+=1
            pontos.innerText = pontuacao
        }
        else if (escolhaAdversario === "papel") {
            alert("Empate!")
        }
        else if (escolhaAdversario === "tesoura") {
            alert("Você Perdeu!")
            pontuacao-=1;
            pontos.innerText = pontuacao
        }
    }
})
tesoura.addEventListener("click", function () {
    let adversario = ["pedra", "papel", "tesoura"];
    let indice = Math.floor(Math.random() * adversario.length);
    let escolhaAdversario = adversario[indice];

    let escolha = "tesoura";

    if (escolha === "tesoura") {
        if (escolhaAdversario === "pedra") {
            alert("Você Perdeu!")
            pontuacao-=1;
            pontos.innerText = pontuacao
        }
        else if (escolhaAdversario === "papel") {
            alert("Você ganhou!")
            pontuacao+=1
            pontos.innerText = pontuacao
        }
        else if (escolhaAdversario === "tesoura") {
            alert("Empate!")
        }
    }
})

//adivinhar

const advnumero = document.getElementById("advinhos");
const advinhar = document.getElementById("btnAdvinhar");

advinhar.addEventListener("click", function () {
    let numsorteado = Math.floor(Math.random() * 11); 
    let numpalpite = Number(advnumero.value); 

    if (numpalpite === numsorteado) {
        alert("Você acertou! O número era " + numsorteado);
        pontuacao += 10 
        pontos.innerText = pontuacao;
        advnumero.value = "";
    } else {
        alert("Você errou! O número era " + numsorteado);
        pontuacao-=1;
        pontos.innerText = pontuacao;
        advnumero.value = ""; 
    }
});

