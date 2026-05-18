chcp 65001
set datestr=%date:~10,4%-%date:~4,2%-%date:~7,2%
SET HOUR=%time:~0,2%
SET dtStamp9=0%time:~1,1%_%time:~3,2%_%time:~6,2%
SET dtStamp24=%time:~0,2%_%time:~3,2%_%time:~6,2%

if "%HOUR:~0,1%" == " " (SET dtStamp=%dtStamp9%) else (SET dtStamp=%dtStamp24%)

if exist "A:\" (
   mkdir "A:\CurlyPad"
   CD /D "A:\CurlyPad"
   mkdir %datestr%
   CD %datestr%
   mkdir %dtStamp%
   CD %dtStamp%
   xcopy /e /c /h /k /y /i "E:\CurlyPad\"
)

if exist "E:\CurlyPad\Help" (
   if exist "E:\styles.css" (
      CD /D "E:\CurlyPad\Help"
      xcopy /c /h /k /y "E:\styles.css"
   )
   if exist "E:\CurlyPad\CurlyPad\bin\Debug\net7.0-windows7.0" (
      CD /D "E:\CurlyPad\CurlyPad\bin\Debug\net7.0-windows7.0"
      mkdir "Help"
      CD /D "E:\CurlyPad\CurlyPad\bin\Debug\net7.0-windows7.0\Help"
      xcopy /c /h /k /y "E:\CurlyPad\Help\*.*"
   )
   if exist "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0" (
      CD /D "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0"
      mkdir "Help"
      CD /D "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0\Help"
      xcopy /c /h /k /y "E:\CurlyPad\Help\*.*"
   )
)
if exist "E:\CurlyPad\CurlyPad\bin\Debug\net7.0-windows7.0" (
   CD /D "E:\CurlyPad\CurlyPad\bin\Debug\net7.0-windows7.0"
   if exist "E:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Debug\net7.0-windows7.0" (
      xcopy /c /h /k /y "E:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Debug\net7.0-windows7.0\*.*"
   )
)
if exist "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0" (
   CD /D "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0"
   if exist "E:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Release\net7.0-windows7.0" (
      xcopy /c /h /k /y "E:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Release\net7.0-windows7.0\*.*"
   )
   del "E:\CurlyPad\CurlyPad\bin\Release\net7.0-windows7.0\*.pdb"
)
echo %datestr%
echo %dtStamp%
Exit 0
