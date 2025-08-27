import tkinter as tk
from tkinter import filedialog, messagebox
import subprocess
import os

def extrair_audio_ffmpeg(caminho_video):
    try:
        saida_mp3 = os.path.splitext(caminho_video)[0] + ".mp3"
        comando = [
            "ffmpeg",
            "-i", caminho_video,
            "-vn",  # sem vídeo
            "-ab", "192k",  # bitrate do áudio
            "-ar", "44100", # taxa de amostragem
            "-y",  # sobrescrever se existir
            saida_mp3
        ]
        subprocess.run(comando, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        print(f"Convertido: {saida_mp3}")
        return True
    except Exception as e:
        print(f"Erro ao processar {caminho_video}: {e}")
        return False

def selecionar_arquivos():
    root = tk.Tk()
    root.withdraw()

    arquivos = filedialog.askopenfilenames(
        title="Selecione arquivos WEBM",
        filetypes=[("Arquivos WEBM", "*.webm")]
    )

    if not arquivos:
        print("Nenhum arquivo selecionado.")
        return

    sucesso = 0
    falha = 0
    for arquivo in arquivos:
        if extrair_audio_ffmpeg(arquivo):
            sucesso += 1
        else:
            falha += 1

    messagebox.showinfo("Resultado", f"Áudios extraídos: {sucesso}\nFalhas: {falha}")

if __name__ == "__main__":
    selecionar_arquivos()
