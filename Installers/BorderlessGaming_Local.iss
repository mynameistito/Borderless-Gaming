[Setup]
#define ReleaseDir "../BorderlessGaming/bin/Release"
#define MainProg ReleaseDir + "/BorderlessGaming.exe"
#define Major
#define Minor
#define Rev
#define Build
#define Version ParseVersion(MainProg, Major, Minor, Rev, Build)
#define AppVersion Str(Major)+"."+Str(Minor)+(Rev > 0 ? "."+Str(Rev) : "")
AppName=Borderless Gaming
AppPublisher=Andrew Sampson
AppCopyright=Copyright (C) 2014-2026 Andrew Sampson
DefaultDirName={autopf}\Borderless Gaming
DefaultGroupName=Borderless Gaming
OutputDir=./
DisableReadyMemo=yes
DisableReadyPage=yes
SetupIconFile=../BorderlessGaming_new.ico
Compression=lzma/ultra64
SolidCompression=yes
LicenseFile=../LICENSE
Uninstallable=yes
PrivilegesRequired=admin
DisableProgramGroupPage=yes
DirExistsWarning=no
ArchitecturesInstallIn64BitMode=x64compatible

AppVersion={#AppVersion}
VersionInfoVersion={#Version}
OutputBaseFilename=BorderlessGaming{#AppVersion}_setup
AppVerName=Borderless Gaming v{#AppVersion}
AppContact=Borderless Gaming on Github
AppComments=Play your favorite games in a borderless window; no more time-consuming Alt-Tabs!
AppPublisherURL=https://github.com/Codeusa/Borderless-Gaming
AppSupportURL=https://github.com/Codeusa/Borderless-Gaming/issues
AppUpdatesURL=https://github.com/Codeusa/Borderless-Gaming/releases/latest
UninstallDisplayName=Borderless Gaming
UninstallDisplayIcon={app}\BorderlessGaming.exe

[Messages]
BeveledLabel=Borderless Gaming {#AppVersion} Setup

[Languages]
Name: english; MessagesFile: compiler:Default.isl

[Files]
Source: {#ReleaseDir}/BorderlessGaming.exe; DestDir: {app}; Flags: ignoreversion
Source: {#ReleaseDir}/BorderlessGaming.exe.config; DestDir: {app}
Source: {#ReleaseDir}/BorderlessGaming.Logic.dll; DestDir: {app}
Source: {#ReleaseDir}/CommandLine.dll; DestDir: {app}
Source: {#ReleaseDir}/Microsoft.Win32.TaskScheduler.dll; DestDir: {app}
Source: {#ReleaseDir}/protobuf-net.dll; DestDir: {app}
Source: {#ReleaseDir}/Languages.zip; DestDir: {app}
Source: ../LICENSE; DestName: License.txt; DestDir: {app}
Source: ../README.md; DestName: Read Me.txt; DestDir: {app}

[Tasks]
Name: desktopicon; Description: {cm:CreateDesktopIcon}; GroupDescription: {cm:AdditionalIcons};

[Icons]
Name: {commondesktop}\Borderless Gaming; Filename: {app}\BorderlessGaming.exe; WorkingDir: {app}; Tasks: desktopicon
Name: {group}\Borderless Gaming; Filename: {app}\BorderlessGaming.exe; WorkingDir: {app}
Name: {group}\Uninstall Borderless Gaming; Filename: {uninstallexe}
Name: {group}\License Agreement; Filename: {app}\License.txt
Name: {group}\Read Me; Filename: {app}\Read Me.txt

[Run]
Description: Start Borderless Gaming; Filename: {app}\BorderlessGaming.exe; Flags: nowait postinstall skipifsilent shellexec

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then begin
    if MsgBox('Do you want to delete your Borderless Gaming settings and preferences as well?', mbConfirmation, MB_YESNO) = IDYES
    then begin
      DelTree(ExpandConstant('{userappdata}\Andrew Sampson\Borderless Gaming'), True, True, True)
    end;
  end;
end;
