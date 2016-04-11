@echo off
cls
color 0D

:start
echo -----------------------------------
echo - Option 1 - Export Bin Files     -
echo - Option 2 - Import Bin File      -
echo - Option 3 - Clean up Files       -
echo -----------------------------------
echo - Option 9 - Exit Program         -
echo -----------------------------------
set /p choice=Your Option: 

if '%choice%'=='1' goto :export
if '%choice%'=='2' goto :import
if '%choice%'=='3' goto :cleanup
if '%choice%'=='9' goto :exit

cls
echo.
echo "%choice%" is not a valid option. Please try again.
echo.
goto start

:export
echo.

if '%client%'=='' goto setclient

echo.

echo Exporting Bin Files...
echo.
swfbinexport %client%.swf

echo.
echo Export Successful!

echo.
goto start

:import
echo.

if '%client%'=='' goto setclient

echo.

echo Importing Bin File...
swfbinreplace %client%.swf 104 %client%-104.bin

echo Imported Bin File Successfully!

echo.
goto start

:cleanup
echo.

if '%client%'=='' goto setclient

echo Cleaning up...
del /S /F /Q %client%-*.bin >NUL

echo.
echo Clean up complete!
echo.

goto start

:setclient
echo.
set /p client=Input client name:

goto check

:check
echo.
if '%choice%'=='1' goto :export
if '%choice%'=='2' goto :import
if '%choice%'=='3' goto :cleanup
if '%choice%'=='9' goto :exit
goto start

:exit
exit