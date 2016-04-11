@echo off
set /p wmap="Enter map name: "
copy %wmap%.wmap Json2wmapConv\bin\Debug
Json2wmapConv\bin\Debug\Json2wmapConv %wmap%.wmap %wmap%.jm something
echo Decompiled map!
pause