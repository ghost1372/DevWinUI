@echo off
setlocal

:: Set the base folder path
set BASE_DIR=%cd%

:: Loop through all directories and subdirectories
for /r "%BASE_DIR%" %%d in (.) do (
    if exist "%%d\bin" (
        echo Deleting folder: "%%d\bin"
        rmdir /s /q "%%d\bin"
    )
    if exist "%%d\obj" (
        echo Deleting folder: "%%d\obj"
        rmdir /s /q "%%d\obj"
    )
)

echo Done.
