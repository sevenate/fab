@echo off

rem -- Prevent creating "Fab.sln.cache" file --
rem set MSBuildUseNoSolutionCache=1

rem C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:Clean /p:Configuration=Release Fab.sln
rem C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /t:ReBuild /p:Configuration=Release Fab.sln

C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:Configuration=Release build.targets
pause