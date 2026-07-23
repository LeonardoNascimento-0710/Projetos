@echo off
setlocal EnableExtensions
chcp 65001 >nul
title Player Promocional - Instalador completo do Piper e vozes

rem ============================================================
rem PLAYER PROMOCIONAL
rem Instalador completo para CMD
rem
rem Requisito:
rem - Python ja instalado no Windows
rem - O comando "py" precisa estar disponivel
rem ============================================================

set "VOZ_MASC=pt_BR-cadu-medium"
set "VOZ_FEM=dii_pt-BR"

set "PASTA_PIPER=%LOCALAPPDATA%\PlayerPromocional\Piper"
set "PASTA_VOZES=%PASTA_PIPER%\vozes"
set "PASTA_TESTES=%PASTA_PIPER%\testes"

set "MODELO_MASC=%PASTA_VOZES%\%VOZ_MASC%.onnx"
set "CONFIG_MASC=%PASTA_VOZES%\%VOZ_MASC%.onnx.json"

set "MODELO_FEM=%PASTA_VOZES%\%VOZ_FEM%.onnx"
set "CONFIG_FEM=%PASTA_VOZES%\%VOZ_FEM%.onnx.json"

set "TESTE_MASC=%PASTA_TESTES%\teste_voz_masculina.wav"
set "TESTE_FEM=%PASTA_TESTES%\teste_voz_feminina.wav"

set "URL_MODELO_FEM=https://huggingface.co/OpenVoiceOS/pipertts_pt-BR_dii/resolve/main/dii_pt-BR.onnx?download=true"
set "URL_CONFIG_FEM=https://huggingface.co/OpenVoiceOS/pipertts_pt-BR_dii/resolve/main/dii_pt-BR.onnx.json?download=true"

cls
echo ============================================================
echo  PLAYER PROMOCIONAL - INSTALADOR COMPLETO DE VOZES
echo ============================================================
echo.
echo Este instalador vai:
echo  1. Verificar o Python
echo  2. Preparar o pip
echo  3. Instalar ou atualizar o Piper
echo  4. Baixar a voz masculina
echo  5. Baixar a voz feminina
echo  6. Gerar dois audios de teste
echo.

set "FORCAR=N"
set /p "FORCAR=Deseja baixar novamente as vozes, mesmo se ja existirem? (S/N): "
if /I "%FORCAR%"=="S" (
    set "FORCAR=1"
) else (
    set "FORCAR=0"
)

echo.
echo [1/8] Verificando o Python...

where py.exe >nul 2>&1
if errorlevel 1 goto ERRO_PYTHON

py -3 --version
if errorlevel 1 goto ERRO_PYTHON

echo.
echo [2/8] Criando as pastas...

if not exist "%PASTA_VOZES%" mkdir "%PASTA_VOZES%"
if errorlevel 1 goto ERRO_PASTA

if not exist "%PASTA_TESTES%" mkdir "%PASTA_TESTES%"
if errorlevel 1 goto ERRO_PASTA

echo Pasta das vozes:
echo %PASTA_VOZES%

echo.
echo [3/8] Preparando e atualizando o pip...

py -3 -m ensurepip --upgrade
if errorlevel 1 goto ERRO_PIP

py -3 -m pip install --upgrade pip setuptools wheel
if errorlevel 1 goto ERRO_PIP

echo.
echo [4/8] Instalando ou atualizando o Piper...

py -3 -m pip install --upgrade piper-tts
if errorlevel 1 goto ERRO_PIPER

py -3 -c "import piper; print('Piper carregado com sucesso.')"
if errorlevel 1 goto ERRO_PIPER

echo.
echo [5/8] Instalando a voz masculina: %VOZ_MASC%

if "%FORCAR%"=="1" (
    del /F /Q "%MODELO_MASC%" 2>nul
    del /F /Q "%CONFIG_MASC%" 2>nul
)

if exist "%MODELO_MASC%" if exist "%CONFIG_MASC%" (
    echo A voz masculina ja esta instalada.
) else (
    py -3 -m piper.download_voices --data-dir "%PASTA_VOZES%" "%VOZ_MASC%"
    if errorlevel 1 goto ERRO_VOZ_MASC
)

if not exist "%MODELO_MASC%" goto ERRO_VOZ_MASC
if not exist "%CONFIG_MASC%" goto ERRO_VOZ_MASC

echo Voz masculina instalada com sucesso.

echo.
echo [6/8] Instalando a voz feminina: %VOZ_FEM%

if "%FORCAR%"=="1" (
    del /F /Q "%MODELO_FEM%" 2>nul
    del /F /Q "%CONFIG_FEM%" 2>nul
)

if exist "%MODELO_FEM%" (
    echo O modelo feminino ja esta instalado.
) else (
    call :BAIXAR_ARQUIVO "%URL_MODELO_FEM%" "%MODELO_FEM%" "modelo da voz feminina"
    if errorlevel 1 goto ERRO_VOZ_FEM
)

if exist "%CONFIG_FEM%" (
    echo A configuracao feminina ja esta instalada.
) else (
    call :BAIXAR_ARQUIVO "%URL_CONFIG_FEM%" "%CONFIG_FEM%" "configuracao da voz feminina"
    if errorlevel 1 goto ERRO_VOZ_FEM
)

if not exist "%MODELO_FEM%" goto ERRO_VOZ_FEM
if not exist "%CONFIG_FEM%" goto ERRO_VOZ_FEM

powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "try { Get-Content -LiteralPath '%CONFIG_FEM%' -Raw -Encoding UTF8 | ConvertFrom-Json | Out-Null; exit 0 } catch { Write-Host $_.Exception.Message; exit 1 }"

if errorlevel 1 goto ERRO_JSON_FEM

echo Voz feminina instalada com sucesso.

echo.
echo [7/8] Gerando o teste da voz masculina...

del /F /Q "%TESTE_MASC%" 2>nul

py -3 -m piper ^
    --data-dir "%PASTA_VOZES%" ^
    -m "%VOZ_MASC%" ^
    -f "%TESTE_MASC%" ^
    -- "Teste da voz masculina do Player Promocional."

if errorlevel 1 goto ERRO_TESTE_MASC
if not exist "%TESTE_MASC%" goto ERRO_TESTE_MASC

echo Teste masculino criado:
echo %TESTE_MASC%

echo.
echo [8/8] Gerando o teste da voz feminina...

del /F /Q "%TESTE_FEM%" 2>nul

py -3 -m piper ^
    --data-dir "%PASTA_VOZES%" ^
    -m "%VOZ_FEM%" ^
    -f "%TESTE_FEM%" ^
    -- "Teste da voz feminina do Player Promocional."

if errorlevel 1 goto ERRO_TESTE_FEM
if not exist "%TESTE_FEM%" goto ERRO_TESTE_FEM

echo Teste feminino criado:
echo %TESTE_FEM%

echo.
echo ============================================================
echo  INSTALACAO CONCLUIDA COM SUCESSO
echo ============================================================
echo.
echo Pasta das vozes:
echo %PASTA_VOZES%
echo.
echo Arquivos da voz masculina:
echo %MODELO_MASC%
echo %CONFIG_MASC%
echo.
echo Arquivos da voz feminina:
echo %MODELO_FEM%
echo %CONFIG_FEM%
echo.
echo Audios de teste:
echo %TESTE_MASC%
echo %TESTE_FEM%
echo.
echo O Player Promocional ja pode usar as duas vozes.
echo.
pause
exit /b 0


:BAIXAR_ARQUIVO
set "DOWNLOAD_URL=%~1"
set "DOWNLOAD_DESTINO=%~2"
set "DOWNLOAD_DESCRICAO=%~3"
set "DOWNLOAD_TEMP=%DOWNLOAD_DESTINO%.download"

del /F /Q "%DOWNLOAD_TEMP%" 2>nul

echo Baixando %DOWNLOAD_DESCRICAO%...

where curl.exe >nul 2>&1
if errorlevel 1 goto DOWNLOAD_POWERSHELL

curl.exe ^
    -L ^
    --fail ^
    --retry 3 ^
    --retry-delay 2 ^
    -o "%DOWNLOAD_TEMP%" ^
    "%DOWNLOAD_URL%"

if errorlevel 1 (
    del /F /Q "%DOWNLOAD_TEMP%" 2>nul
    exit /b 1
)

goto DOWNLOAD_FINALIZAR


:DOWNLOAD_POWERSHELL
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "$ErrorActionPreference='Stop'; $ProgressPreference='SilentlyContinue'; Invoke-WebRequest -Uri '%DOWNLOAD_URL%' -OutFile '%DOWNLOAD_TEMP%' -MaximumRedirection 10"

if errorlevel 1 (
    del /F /Q "%DOWNLOAD_TEMP%" 2>nul
    exit /b 1
)


:DOWNLOAD_FINALIZAR
if not exist "%DOWNLOAD_TEMP%" exit /b 1

for %%A in ("%DOWNLOAD_TEMP%") do (
    if %%~zA LSS 1000 (
        del /F /Q "%DOWNLOAD_TEMP%" 2>nul
        exit /b 1
    )
)

move /Y "%DOWNLOAD_TEMP%" "%DOWNLOAD_DESTINO%" >nul
if errorlevel 1 exit /b 1

exit /b 0


:ERRO_PYTHON
echo.
echo ============================================================
echo  ERRO: PYTHON NAO ENCONTRADO
echo ============================================================
echo.
echo O comando "py" nao foi encontrado.
echo.
echo Reinstale o Python e marque estas opcoes:
echo  - Add Python to PATH
echo  - Install launcher for all users
echo.
goto FIM_ERRO


:ERRO_PASTA
echo.
echo ERRO: nao foi possivel criar as pastas do Piper.
goto FIM_ERRO


:ERRO_PIP
echo.
echo ERRO: nao foi possivel preparar ou atualizar o pip.
goto FIM_ERRO


:ERRO_PIPER
echo.
echo ERRO: nao foi possivel instalar ou carregar o Piper.
goto FIM_ERRO


:ERRO_VOZ_MASC
echo.
echo ERRO: nao foi possivel instalar a voz masculina.
goto FIM_ERRO


:ERRO_VOZ_FEM
echo.
echo ERRO: nao foi possivel baixar a voz feminina.
goto FIM_ERRO


:ERRO_JSON_FEM
echo.
echo ERRO: o arquivo JSON da voz feminina esta invalido.
goto FIM_ERRO


:ERRO_TESTE_MASC
echo.
echo ERRO: o Piper nao conseguiu gerar o teste masculino.
goto FIM_ERRO


:ERRO_TESTE_FEM
echo.
echo ERRO: o Piper nao conseguiu gerar o teste feminino.
goto FIM_ERRO


:FIM_ERRO
echo.
echo A instalacao nao foi concluida.
echo.
pause
exit /b 1
