function appendToDisplay(value) {
    document.getElementById('display').value += value;
}

// Função para limpar o display
function clearDisplay() {
    document.getElementById('display').value = '';
}

// Função para deletar o último caractere no display
function deleteLast() {
    var currentValue = document.getElementById('display').value;
    document.getElementById('display').value = currentValue.slice(0, -1);
}

// Função para calcular o resultado
function calculateResult() {
    try {
        var result = eval(document.getElementById('display').value);
        document.getElementById('display').value = result;
    } catch (e) {
        document.getElementById('display').value = 'Erro';
    }
}