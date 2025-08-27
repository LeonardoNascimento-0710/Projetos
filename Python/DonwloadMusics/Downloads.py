import yt_dlp
import os

def baixar_playlist_videos(url, pasta_destino):
    os.makedirs(pasta_destino, exist_ok=True)
    opcoes = {
        'format': 'bestvideo+bestaudio/best',
        'outtmpl': f'{pasta_destino}/%(playlist_index)03d - %(title)s.%(ext)s',
        'ignoreerrors': True,
        'noplaylist': False,
        'quiet': False,
        'no_warnings': True,
    }

    with yt_dlp.YoutubeDL(opcoes) as ydl:
        ydl.download([url])

if __name__ == "__main__":
    link = input("Cole o link da playlist do YouTube: ")
    pasta = input("Digite o caminho da pasta onde quer salvar os vídeos (ex: C:/Users/SeuUsuario/Downloads/minha_pasta): ")
    print(f"Baixando a playlist na pasta: {pasta}")
    baixar_playlist_videos(link, pasta)
    print("Download finalizado!")
