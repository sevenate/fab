@echo off

C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:Deploy /p:Configuration=Release deploy.targets
pause