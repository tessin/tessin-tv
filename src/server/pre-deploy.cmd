@echo off

if not "%VSCMD_VER%"=="" (
	goto VsDevCmd
)

set "VSCMD_START_DIR=%CD%"

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" (
	call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsDevCmd.bat" %*
	goto VsDevCmd
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat" (
	call "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\Common7\Tools\VsDevCmd.bat" %*
	goto VsDevCmd
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" (
	call "C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\Tools\VsDevCmd.bat" %*
	goto VsDevCmd
)

:VsDevCmd

msbuild ../../tessin-tv.sln  /nologo /p:Configuration=Release /clp:ErrorsOnly;Summary

del bin\Release\net461\local.settings.json

rmdir /s /q bin\Release\net461\bin\de
rmdir /s /q bin\Release\net461\bin\es
rmdir /s /q bin\Release\net461\bin\fr
rmdir /s /q bin\Release\net461\bin\it
rmdir /s /q bin\Release\net461\bin\ja
rmdir /s /q bin\Release\net461\bin\ko
rmdir /s /q bin\Release\net461\bin\ru
rmdir /s /q bin\Release\net461\bin\zh-Hans
rmdir /s /q bin\Release\net461\bin\zh-Hant

rmdir /s /q bin\Release\net461\bin\runtimes\osx
rmdir /s /q bin\Release\net461\bin\runtimes\unix
