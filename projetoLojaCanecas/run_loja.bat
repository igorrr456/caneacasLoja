@echo off
cd /d "%~dp0"
echo Iniciando a Loja de Canecas (perfil http, porta 5200)...
dotnet run --launch-profile http
pause
