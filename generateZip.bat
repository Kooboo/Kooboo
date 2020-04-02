@ECHO off
::%1--->build type:release,server
set buildType=Release
set batPath=%1
set adminPath=%batPath%Kooboo.Web\_Admin

set koobooPath=%batPath%Kooboo.App

set dllPath=%koobooPath%\bin\%buildType%
set langPath=%batPath%Kooboo.Web\Lang

set zipFile=%dllPath%\Kooboo.zip
set fileCompressPath=%batPath%\published\

::delete existed zipFile
if exist "%zipFile%" (del %zipFile%)

set copyBasePath=%koobooPath%\bin\%buildType%\Kooboo
set copyFolder=%copyBasePath%\Kooboo
set copyAdminPath=%copyFolder%\_Admin
set copyDllPath=%copyFolder%
set copyLangPath=%copyFolder%\Lang


::reset _admin fold if minifierAmdinPath exists
set minifierAmdinPath=%batPath%Kooboo.Web\Minifier\_Admin
if exist "%minifierAmdinPath%" ( set adminPath=%minifierAmdinPath%)

::copy _admin exclude kbtest .vscode mobileEditor
C:\Windows\System32\robocopy  %adminPath% %copyAdminPath% /e /xd kbtest .vscode mobileEditor Market
::copy kbtest 
C:\Windows\System32\robocopy  %adminPath%\kbtest %copyAdminPath%\kbtest /e
::Kooboo.exe
C:\Windows\System32\robocopy  %dllPath% %copyDllPath% Kooboo.exe
::Kooboo.Upgrade.exe
C:\Windows\System32\robocopy  %dllPath% %copyDllPath%\Upgrade  Kooboo.Upgrade.exe 
::copy language
C:\Windows\System32\robocopy  %langPath% %copyLangPath%
::copy modules
C:\Windows\System32\robocopy  %batPath%Kooboo.Web\modules %copyFolder%\modules
::copy sqlite
C:\Windows\System32\robocopy  %koobooPath%\bin\%buildType%\x64 %copyDllPath%\x64

rd /s /q %copyAdminPath%\Scripts\lib\vs
:: delete minifier admin
if exist "%minifierAmdinPath%" ( rd /s /q %batPath%Kooboo.Web\Minifier)


set zipExePath=%batPath%\tools\7z\
::zip folder
%zipExePath%\7z.exe a -tzip %zipFile% %copyBasePath%\* -r

rd /s /q %copyBasePath%
