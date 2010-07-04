@echo off

rem -- Prevent creating "Fab.sln.cache" file --
set MSBuildUseNoSolutionCache=1

rem C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:Clean /p:Configuration=Release Fab.sln
C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:ReBuild /p:Configuration=Release Fab.sln
pause