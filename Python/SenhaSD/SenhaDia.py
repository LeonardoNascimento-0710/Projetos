from datetime import datetime, timedelta
import ctypes

# Hora e data atual
agora = datetime.now()

# Subtrair 1 hora e 1 dia
ajustada = agora - timedelta(hours=1, days=1)

# Calcular o mês anterior
mes = ajustada.month - 1
ano = ajustada.year
if mes == 0:
    mes = 12
    ano -= 1

# Gerar a senha no formato HHDDMM
senha = f"{ajustada.strftime('%H')}{ajustada.strftime('%d')}{mes:02d}"

# Exibir em uma mensagem (caixa de diálogo do Windows)
ctypes.windll.user32.MessageBoxW(0, senha, "Senha do Dia", 0)
