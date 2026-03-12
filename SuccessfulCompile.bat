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
   rar a -r -dh "CurlyPad.rar" "D:\CurlyPad\"
)

if exist "D:\CurlyPad\Help" (
   if exist "D:\styles.css" (
      CD /D "D:\CurlyPad\Help"
      xcopy /c /h /k /y "D:\styles.css"
   )
   if exist "D:\CurlyPad\CurlyPad\bin\Debug\net10.0-windows7.0" (
      CD /D "D:\CurlyPad\CurlyPad\bin\Debug\net10.0-windows7.0"
      mkdir "Help"
      CD /D "D:\CurlyPad\CurlyPad\bin\Debug\net10.0-windows7.0\Help"
      xcopy /c /h /k /y "D:\CurlyPad\Help\*.*"
   )
   if exist "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0" (
      CD /D "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0"
      mkdir "Help"
      CD /D "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0\Help"
      xcopy /c /h /k /y "D:\CurlyPad\Help\*.*"
   )
)
if exist "D:\CurlyPad\CurlyPad\bin\Debug\net10.0-windows7.0" (
   CD /D "D:\CurlyPad\CurlyPad\bin\Debug\net10.0-windows7.0"
   if exist "D:\aaaa speech productivity\7 Core\ExternalColorPicker\ExternalColorPicker\bin\Debug" (
      xcopy /c /h /k /y "D:\aaaa speech productivity\7 Core\ExternalColorPicker\ExternalColorPicker\bin\Debug\*.*"
   )
   if exist "D:\aaaa speech productivity\7 Core\ExternalFontPicker\bin\Debug" (
      xcopy /c /h /k /y "D:\aaaa speech productivity\7 Core\ExternalFontPicker\bin\Debug\*.*"
   )
   if exist "D:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Debug\net10.0-windows7.0" (
      xcopy /c /h /k /y "D:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Debug\net10.0-windows7.0\*.*"
   )
)
if exist "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0" (
   CD /D "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0"
   if exist "D:\aaaa speech productivity\7 Core\ExternalColorPicker\ExternalColorPicker\bin\Release" (
      xcopy /c /h /k /y "D:\aaaa speech productivity\7 Core\ExternalColorPicker\ExternalColorPicker\bin\Release\*.*"
   )
   if exist "D:\aaaa speech productivity\7 Core\ExternalFontPicker\bin\Release" (
      xcopy /c /h /k /y "D:\aaaa speech productivity\7 Core\ExternalFontPicker\bin\Release\*.*"
   )
   if exist "D:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Release\net10.0-windows7.0" (
      xcopy /c /h /k /y "D:\CurlyPad\CurlyPadCleaner\CurlyPadCleaner\bin\Release\net10.0-windows7.0\*.*"
   )
   del "D:\CurlyPad\CurlyPad\bin\Release\net10.0-windows7.0\*.pdb"
)
echo %datestr%
echo %dtStamp%
Exit 0
