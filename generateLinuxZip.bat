@ECHO off
::%1--->build type:release,server
::set buildType=%1
set batPath=%~dp0
set basePath=%batPath%
set adminPath=%basePath%Kooboo.Web\_Admin
set langPath=%basePath%Kooboo.Web\Lang

set koobooPath=%basePath%Kooboo.App.Standard\bin\release

::dotnet core file 
::kooboo publish path
set dllPath=%koobooPath%\PublishOutput

set zipFile=%koobooPath%\KoobooLinux.zip

::delete existed zipFile
if exist "%zipFile%" (del %zipFile%)


set copyBasePath=%koobooPath%\Kooboo
set copyFolder=%copyBasePath%\Kooboo
set copyAdminPath=%copyFolder%\_Admin
set copyDllPath=%copyFolder%
set copyLangPath=%copyFolder%\Lang
set copyRuntimesPath=%copyFolder%\runtimes


::copy _admin exclude kbtest .vscode mobileEditor
C:\Windows\System32\robocopy  %adminPath% %copyAdminPath% /e /xd kbtest .vscode mobileEditor Market
::copy kbtest 
C:\Windows\System32\robocopy  %adminPath%\kbtest %copyAdminPath%\kbtest /e
::copy dll,exe,config
C:\Windows\System32\robocopy  %dllPath% %copyDllPath% *.dll *.exe *.config *.json
::copy language
C:\Windows\System32\robocopy  %langPath% %copyLangPath%
::copy modules
C:\Windows\System32\robocopy  %basePath%Kooboo.Web\modules %copyFolder%\modules
::copy runtimes
C:\Windows\System32\robocopy  %dllPath%\runtimes %copyRuntimesPath% /e
::copy Kooboo.ServerUpgrade.runtimeconfig.json
Copy %copyDllPath%\Kooboo.App.runtimeconfig.json %copyDllPath%\Kooboo.App.Upgrade.runtimeconfig.json



set zipExePath=%batPath%\tools\7z\
::zip folder
%zipExePath%\7z.exe a -tzip %zipFile% %copyBasePath%\* -r

rd /s /q %copyBasePath%
