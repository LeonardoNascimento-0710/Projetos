import random

dado = ""
quantidade = 0

while dado != "d6" and dado != "d20" and dado != "d100":
    dado = input("Digite o tipo de dado a ser rolado (d6, d20, d100): ")
    if dado == "d6" or dado == "d20" or dado == "d100":
        # Perguntar quantos dados o usuário deseja rolar
        while True:
            try:
                quantidade = int(input(f"Quantos dados {dado} você quer rolar? "))
                if quantidade <= 0:
                    print("Por favor, insira um número válido de dados.")
                else:
                    break
            except ValueError:
                print("Por favor, insira um número inteiro válido.")
        
        for _ in range(quantidade):
            if dado == "d6":
                numero = random.randint(1, 6)
                print(f"Resultado: {numero}")
            elif dado == "d20":
                numero = random.randint(1, 20)
                print(f"Resultado: {numero}")
            elif dado == "d100":
                numero = random.randint(1, 100)
                print(f"Resultado: {numero}")
        
        resposta = input("Deseja jogar os dados novamente?: ")
        if resposta == "S" or resposta == "s" or resposta == "sim" or resposta == "SIM":
            dado = ""  
    else:
        print("Digite um dado válido!")
