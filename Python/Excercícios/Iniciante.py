#exercício 1 (print)
print("Hello World")

#exercício 2 (imput+soma+print)
num1 = int(input("digite o primeiro número: "))
num2 = int(input("digite o segundo número: "))
soma = num1 + num2
print("A soma dos números é", soma)

#exercício 3 (par ou ímpar, if/else)
num = int(input("Digite um número: "))
if num % 2 == 0:
    print("é um número par!")
else:
    print("é um número ímpar!")

#exercício 4 (calculadora simples)
valor1 = float(input("digite o primeiro número: "))
valor2 = float(input("digite o segundo número: "))
adicao = valor1 + valor2
subtracao = valor1 - valor2
multiplicacao = valor1 * valor2 
divisao = valor1 / valor2
print("A soma dos números é:",adicao,"a subtração: ",subtracao,"a multiplicação: ",multiplicacao,"e a divisão: ",divisao)

#exercício 5 (tabuada com FOR)
tabuada = int(input("Digite o numero que deseja ver a tabuada (1 ao 10): "))
for i in range(1,11):
    resultado = tabuada * i
    print("Resultado: ", resultado)
