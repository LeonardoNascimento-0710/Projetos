$ErrorActionPreference = "Stop"

$pasta = Join-Path $env:LOCALAPPDATA "PlayerPromocional\Piper\vozes"
$modelo = Join-Path $pasta "dii_pt-BR.onnx"
$configuracao = Join-Path $pasta "dii_pt-BR.onnx.json"

$urlModelo = "https://huggingface.co/OpenVoiceOS/pipertts_pt-BR_dii/resolve/main/dii_pt-BR.onnx?download=true"
$urlConfiguracao = "https://huggingface.co/OpenVoiceOS/pipertts_pt-BR_dii/resolve/main/dii_pt-BR.onnx.json?download=true"

Write-Host "Criando a pasta da voz feminina..."
New-Item -ItemType Directory -Path $pasta -Force | Out-Null

Write-Host "Baixando dii_pt-BR.onnx..."
Invoke-WebRequest -Uri $urlModelo -OutFile $modelo -UseBasicParsing

Write-Host "Baixando dii_pt-BR.onnx.json..."
Invoke-WebRequest -Uri $urlConfiguracao -OutFile $configuracao -UseBasicParsing

if (-not (Test-Path $modelo) -or (Get-Item $modelo).Length -lt 1000000) {
    throw "O arquivo do modelo não foi baixado corretamente."
}

if (-not (Test-Path $configuracao) -or (Get-Item $configuracao).Length -lt 1000) {
    throw "O arquivo de configuração não foi baixado corretamente."
}

Write-Host ""
Write-Host "Voz feminina instalada com sucesso em:"
Write-Host $pasta
Write-Host ""
Write-Host "ATENÇÃO: este modelo é fornecido por terceiros. Verifique a licença antes de redistribuí-lo junto com um produto comercial."
Read-Host "Pressione ENTER para fechar"
