import random

# Gerador de Personagens
def gerar_personagem():
    nomes = ["Aragorn", "Merlin", "Luna", "Thorin", "Sylvia"]
    racas = ["Humano", "Elfo", "Anão", "Orc", "Mago"]
    classes = ["Guerreiro", "Mago", "Ladino", "Bárbaro", "Clérigo"]
    
    return f"Nome: {random.choice(nomes)}, Raça: {random.choice(racas)}, Classe: {random.choice(classes)}"

# Gerador de Histórias com narrativa dinâmica
def gerar_historia():
    inicio = ["Em uma terra distante...", "Nas sombras da cidade de Eldoria...", "Em um castelo há muito esquecido..."]
    evento = ["um rei foi assassinado", "um dragão ameaça a vila", "uma seita secreta ressurgiu"]
    reviravolta = ["e um herói precisa surgir.", "mas há um traidor entre os aliados.", "e um artefato mágico pode ser a chave para a salvação."]
    
    return f"{random.choice(inicio)} {random.choice(evento)} {random.choice(reviravolta)}"

# Gerador de falas de NPCs
def falar_npc(nome):
    falas = [
        f"{nome}: 'Bem-vindo, viajante! O que deseja?'",
        f"{nome}: 'Ouvi rumores sobre um tesouro perdido...'",
        f"{nome}: 'Cuidado! Há perigos além destas muralhas.'"
    ]
    
    return random.choice(falas)

# Rolador de Dados
def rolar_dado(lados):
    return random.randint(1, lados)

# Gerenciador de Combate
def iniciar_combate():
    print("Iniciando combate...")
    iniciativa_jogador = rolar_dado(20)
    iniciativa_inimigo = rolar_dado(20)
    
    print(f"Iniciativa do jogador: {iniciativa_jogador}")
    print(f"Iniciativa do inimigo: {iniciativa_inimigo}")
    
    if iniciativa_jogador > iniciativa_inimigo:
        print("O jogador ataca primeiro!")
    else:
        print("O inimigo ataca primeiro!")

# Narrador de História
def narrar_historia():
    historia = gerar_historia()
    print(f"Narrador: {historia}")

# Função principal
def main():
    print(gerar_personagem())
    narrar_historia()
    print(falar_npc("Velho Sábio"))
    iniciar_combate()
    print(f"Rolando um D20: {rolar_dado(20)}")

if __name__ == "__main__":
    main()
