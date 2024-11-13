; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
;-------------------------------------------------------------------------
; 以下は定義情報
;-------------------------------------------------------------------------
; プログラム名
#define MyAppName "biz-Stream バッチ印刷 (OSS)"
; プログラムのバージョン
#define MyAppVersion "1.0"
; 会社やサイトなど
#define MyAppPublisher "bizprint Project"
; プログラムを提供するURLなど
#define MyAppURL "https://github.com/biz-Stream/bizprint"
; プログラムの実行ファイル名
#define MyAppExeName "BatchPrintService.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
;-------------------------------------------------------------------------
; 基本的なインストーラの定義情報の設定
;-------------------------------------------------------------------------
AppId={{97DC484B-EBE2-48CB-926E-AADBC6D386EA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
;DefaultDirName={autopf}\brainsellers\BatchPrint
DefaultDirName={pf32}\brainsellers\BatchPrint
DefaultGroupName={#MyAppName}
;PrivilegesRequired=admin

;-------------------------------------------------------------------------
; ウィザード画面の表示に関わる定義
;-------------------------------------------------------------------------
; 開始ダイアログの表示設定
DisableStartupPrompt = yes
; ようこそ画面の表示設定
DisableWelcomePage = no 
; プログラムグループ設定画面の表示設定
DisableProgramGroupPage= yes
; ユーザー情報設定画面の表示設定
UserInfoPage = no
; インストール先の指定の表示設定
DisableDirPage = no 
; 準備完了画面の表示設定
DisableReadyPage = no
; インストール完了の表示設定
DisableFinishedPage = no

;-------------------------------------------------------------------------
; インストーラの各種定義
;-------------------------------------------------------------------------
; ライセンス合意の表示設定をする場合はこの設定が必要
;LicenseFile=C:\work\is\text\License.txt
; インストール前に読んでもらう注意事項等の表示を行う場合はこの設定が必要
;InfoBeforeFile=C:\work\is\text\InfoBefore.txt
; インストール後に読んでもらう注意事項等の表示を行う場合はこの設定が必要
;InfoAfterFile=C:\work\is\text\InfoAfter.txt
; コンパイル後のインストーラファイルの出力先フォルダ
;OutputDir=C:\work\is\output
; コンパイル後のインストーラファイルのファイル名(.exeの前)
OutputBaseFilename=BatchPrint-OSS-Setup
; インストール時のインストーラアイコンの設定
;SetupIconFile=C:\work\is\Setup.ico
; パスワード入力を表示する場合はこの設定が必要
;Password=1234567890
; 圧縮方式（このまま）
Compression=lzma
SolidCompression=yes
; アンインストーラの表示アイコン（プログラムの指定で可）
UninstallDisplayIcon={app}\{#MyAppExeName}


; "ArchitecturesAllowed=x64compatible" specifies that Setup cannot run
; on anything but x64 and Windows 11 on Arm.
;ArchitecturesAllowed=x64compatible
; "ArchitecturesInstallIn64BitMode=x64compatible" requests that the
; install be done in "64-bit mode" on x64 or Windows 11 on Arm,
; meaning it should use the native 64-bit Program Files directory and
; the 64-bit view of the registry.
;ArchitecturesInstallIn64BitMode=x64compatible
;DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
;OutputDir=C:\Users\murakami\Downloads
;WizardStyle=modern

;-------------------------------------------------------------------------
; インストーラの言語選択設定
; ここで複数言語の指定があるとウィザードの中で言語選択ができる
;-------------------------------------------------------------------------
[Languages]
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"

;-------------------------------------------------------------------------
; 追加タスクの選択
; ここでタスクが設定されていると追加タスクの選択画面が表示される
;-------------------------------------------------------------------------
[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

;-------------------------------------------------------------------------
; インストール対象のファイルの設定
; インストールするファイルを設定する
;-------------------------------------------------------------------------
[Files]
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
; 実行ファイル
Source: "..\BizPrintCommon\bin\Release\BizPrintCommon.dll"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\BatchPrintService\bin\Release\BatchPrintService.exe"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\BizPrintHealthChecker\bin\Release\BizPrintHealthChecker.exe"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\DLL\AxInterop.AcroPDFLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DLL\Interop.AcroPDFLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DLL\Ionic.Zip.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DLL\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DLL\Ionic.Zip-LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\DLL\log4net-LICENSE.txt"; DestDir: "{app}"; Flags: ignoreversion

; 設定ファイル
Source: "..\BatchPrintService\Config\BatchPrintService_log_ja-JP.txt"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion
Source: "..\BatchPrintService\Config\BatchPrintService.xml"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion
Source: "..\BatchPrintService\Config\BatchPrintService_logConfig.xml"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion
Source: "..\BatchPrintService\Config\ErrorDetail_ja-JP.txt"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion

Source: "..\BizPrintHealthChecker\Config\BizPrintHealthChecker_log_ja-JP.txt"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion
Source: "..\BizPrintHealthChecker\Config\BizPrintHealthChecker.xml"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion
Source: "..\BizPrintHealthChecker\Config\BizPrintHealthChecker_logConfig.xml"; DestDir: "{commonappdata}\brainsellers\BatchPrint"; Flags: ignoreversion

;-------------------------------------------------------------------------
; インストール対象のディレクトリの設定
; インストールするディレクトリを設定する
;-------------------------------------------------------------------------
[Dirs]
Name: "{app}"; Permissions: authusers-full
Name: "{commonappdata}\brainsellers\BatchPrint"; Permissions: authusers-full
Name: "{commonappdata}\brainsellers\BatchPrint\log"; Permissions: everyone-full
Name: "{commonappdata}\brainsellers\BatchPrint\tmp"; Permissions: everyone-full

;-------------------------------------------------------------------------
; スタートメニューやデスクトップにショートカットアイコンを登録する設定
;-------------------------------------------------------------------------
[Icons]
;Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\GitHub"; Filename: "https://github.com/biz-Stream/bizprint"; WorkingDir: "{autoprograms}"
Name: "{group}\設定フォルダ"; Filename: "{commonappdata}\brainsellers\BatchPrint"; WorkingDir: "{autoprograms}"
Name: "{group}\{#MyAppName}のアンインストール"; Filename: "{uninstallexe}"; IconFilename: "{uninstallexe}"
;Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

;-------------------------------------------------------------------------
; レジストリ登録の設定
; uninsdeletevalue: アンインストール時に追加した値のみ削除
; uninsdeletekey: アンインストール時にサブキーごと削除
;-------------------------------------------------------------------------
[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue

;-------------------------------------------------------------------------
; インストール時に実行するプログラムの設定
; 例: Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
;-------------------------------------------------------------------------
[Run]
; バッチ印刷を実行
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall

;-------------------------------------------------------------------------
; アンインストール時に実行するプログラムの設定
;-------------------------------------------------------------------------
[UninstallRun]

;-------------------------------------------------------------------------
; カスタムスクリプトのコード定義
;-------------------------------------------------------------------------
[Code]
const
  Base64Chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/';

var
  // カスタムページ
  CustomPage: TInputQueryWizardPage;
  // カスタムページの入力フィールド(ポート番号、SPPパスワード、Base64エンコード済みSPPパスワード)
  PortNo, SppPass, SppPassEncoded: String;

// インストール済みかどうか判定する関数
function IsInstalled: Boolean;
var
  InstallPath: String;
begin
  // 既定のインストールパスにある特定のファイルの存在を確認
  InstallPath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\BatchPrintService.xml');  // インストール済みを確認するファイル
  if FileExists(InstallPath) then
  begin
    // ファイルが存在すればインストール済みとみなす
    Result := True;
  end
  else
  begin
    // ファイルが存在しない場合はインストールされていない
    Result := False;
  end;
end;

// Base64エンコード関数
function EncodeBase64(const Input: String): String;
var
  I, J, A, B, C: Integer;
  Output: String;
  Remainder: Integer;
begin
  SetLength(Output, ((Length(Input) + 2) div 3) * 4);
  J := 1;
  I := 1;
  while I <= Length(Input) do
  begin
    A := Ord(Input[I]);
    B := 0;
    C := 0;
    if I + 1 <= Length(Input) then
      B := Ord(Input[I + 1]);
    if I + 2 <= Length(Input) then
      C := Ord(Input[I + 2]);
    
    Output[J] := Base64Chars[(A shr 2) + 1];
    Output[J + 1] := Base64Chars[((A and 3) shl 4) + (B shr 4) + 1];
    if I + 1 <= Length(Input) then
      Output[J + 2] := Base64Chars[((B and $0F) shl 2) + (C shr 6) + 1]
    else
      Output[J + 2] := '=';
    if I + 2 <= Length(Input) then
      Output[J + 3] := Base64Chars[(C and $3F) + 1]
    else
      Output[J + 3] := '=';
    
    I := I + 3;  // 手動で3を加算
    J := J + 4;  // 手動で4を加算
  end;
  Result := Output;
end;

// コマンドライン引数を取得する関数
function GetCustomParam(const ParamName: String): String;
var
  i: Integer;
  Param, ParamValue: String;
begin
  Result := '';
  // コマンドライン引数をループして目的のパラメータを探す
  for i := 1 to ParamCount do
  begin
    Param := ParamStr(i);
    if Pos(ParamName + '=', Param) = 1 then
    begin
      // =の後ろの値を取得
      ParamValue := Copy(Param, Length(ParamName) + 2, MaxInt);
      if ParamValue <> '' then
      begin
        Result := ParamValue;
        Exit;
      end;
    end;
  end;
end;

// タスクスケジューラへ登録する関数
procedure RegisterScheduledTask();
var
  ExePath: String;
  ResultCode: Integer;
begin
  ExePath := ExpandConstant('{app}\BizPrintHealthChecker.exe');
  if not Exec('schtasks', '/create /tn "BizPrintHealthChecker-BatchPrint" /tr "\"' + ExePath + '\" BatchPrintService" /ru "INTERACTIVE" /sc minute /mo 5 /f', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Log('タスクの登録に失敗しました。エラーコード: ' + IntToStr(ResultCode));
    MsgBox('タスクの登録に失敗しました。エラーコード: ' + IntToStr(ResultCode), mbError, MB_OK);
  end
  else
  begin
    Log('タスクを登録しました。');
    //MsgBox('タスクを登録しました。');
  end;
end;

// タスクスケジューラから削除する関数
procedure UnregisterScheduledTask();
var
  ResultCode: Integer;
begin
  if not Exec('schtasks', '/delete /tn "BizPrintHealthChecker-BatchPrint" /f', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Log('タスクの削除に失敗しました。エラーコード: ' + IntToStr(ResultCode));
    MsgBox('タスクの削除に失敗しました。エラーコード: ' + IntToStr(ResultCode), mbError, MB_OK);
  end
  else
  begin
    Log('タスクを削除しました。');
    //MsgBox('タスクを削除しました。', mbInformation, MB_OK);
  end;
end;

// プロセスが実行中か確認する関数
function IsAppRunning(const FileName: string): Boolean;
var
  FWMIService: Variant;
  FSWbemLocator: Variant;
  FWbemObjectSet: Variant;
begin
  Result := false;
  FSWbemLocator := CreateOleObject('WBEMScripting.SWBEMLocator');
  FWMIService := FSWbemLocator.ConnectServer('', 'root\CIMV2', '', '');
  FWbemObjectSet := FWMIService.ExecQuery(Format('SELECT Name FROM Win32_Process Where Name="%s"',[FileName]));
  Result := (FWbemObjectSet.Count > 0);
  FWbemObjectSet := Unassigned;
  FWMIService := Unassigned;
  FSWbemLocator := Unassigned;
end;

procedure InitializeWizard();
var
  // Acrobatのパス
  AcrobatPath: String;
  // Acrobatのバージョン
  Major, Minor, Revision, Build: Word;
begin
  // 既にインストール済みの場合、メッセージを表示してインストールを中止
  if IsInstalled then
  begin
    Log('このアプリケーションは既にインストールされています。');
    MsgBox('このアプリケーションは既にインストールされています。', mbError, MB_OK);
    Abort;
  end;

  // Acrobat Readerのパスを取得
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\AcroRd32.exe', '', AcrobatPath) then
  begin
    //MsgBox('Install Path: ' + AcrobatPath, mbInformation, MB_OK);
  end;  
  
  // Acrobatのパスを取得
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Acrobat.exe', '', AcrobatPath) then
  begin
    //MsgBox('Install Path: ' + AcrobatPath, mbInformation, MB_OK);
  end;
  
  if AcrobatPath = '' then
  begin
    MsgBox('Acrobat DC または Acrobat Reader DC がインストールされていません。', mbError, MB_OK);
    Abort;
  end;
  
  // Acrobatのファイルバージョンを取得
  GetVersionComponents(AcrobatPath, Major, Minor, Revision, Build);
  if IntToStr(Major) <> '' then
  begin
    //MsgBox('ファイルのメジャーバージョンは: ' + IntToStr(Major), mbInformation, MB_OK)
  end
  else
  begin
    MsgBox('ファイルバージョンが取得できませんでした。', mbError, MB_OK);
    Abort;  
  end;
  
  // バージョン14以下はエラー終了
  if Major <= 14 then
  begin
    MsgBox('Acrobat または Acrobat Reader のバージョンDC以上が必要です。', mbError, MB_OK);
    Abort;
  end;
  
  // Create the page
  CustomPage := CreateInputQueryPage(wpSelectDir, 'ポート番号とSPPパスワードの設定', 'ポート番号とSPPパスワードを入力してください。', '');

  // Add items (False means it's not a password edit)
  CustomPage.Add('ポート番号:', False);
  CustomPage.Add('SPPパスワード:', False);

  // Set initial values (optional)
  CustomPage.Values[0] := '3000';
  CustomPage.Values[1] := '';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  PortNum: Integer;
begin
  Log('NextButtonClick(' + IntToStr(CurPageID) + ') called');

  Result := True;

  // 現在のページがカスタムページかどうかを確認
  if CurPageID = CustomPage.ID then
  begin
    // MsgBox('NextButtonClick:' #13#13 'カスタムページから次へボタンが押されました。', mbInformation, MB_OK);

    if WizardSilent then
    begin
      Log('WizardSilent: True'); 
      // サイレントインストール時、コマンドライン引数から値を取得
      PortNo := GetCustomParam('PortNo');
      SppPass := GetCustomParam('SppPass');
      if PortNo = '' then PortNo := '3000';
    end
    else
    begin
      Log('WizardSilent: False'); 
      // テキストボックスの入力値を取得
      PortNo  := CustomPage.Values[0];
      SppPass := CustomPage.Values[1];
    end;

    Log('PortNo: ' + PortNo);
    Log('SppPass: ' + SppPass);
        
    // 入力が空の場合はエラー表示
    if PortNo = '' then
    begin
      MsgBox('ポート番号を入力してください。', mbError, MB_OK);
      Result := False;  // ページ遷移をキャンセル
      Exit;
    end;
    
    // ポート番号の数値チェック
    PortNum := StrToIntDef(PortNo, 0);
    if PortNum = 0 then
    begin
      MsgBox('ポート番号が正しくありません。', mbError, MB_OK);
      Result := False;  // ページ遷移をキャンセル
      Exit;
    end;

    // ポート番号の範囲チェック
    if (PortNum < 1024) or (PortNum > 65535) then
    begin
      MsgBox('ポート番号が正しくありません。', mbError, MB_OK);
      Result := False;  // ページ遷移をキャンセル
      Exit;
    end;
  end;
end;

function BackButtonClick(CurPageID: Integer): Boolean;
begin
  Log('BackButtonClick(' + IntToStr(CurPageID) + ') called');
  Result := True;
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpReady then
  begin
    //MsgBox('NextButtonClick:' #13#13 'CurPageChanged', mbInformation, MB_OK); 

    // ReadyMemo にカスタム情報を追加して表示
    WizardForm.ReadyMemo.Lines.Add('');
    WizardForm.ReadyMemo.Lines.Add('ポート番号: ' + PortNo);
    WizardForm.ReadyMemo.Lines.Add('SPPパスワード: ' + SppPass);
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  // XMLファイルパス
  XMLFilePath: String;
  // XMLファイルのコンテンツ
  XMLContent: TStringList;
  // 一時的にXMLコンテンツを文字列として格納
  TempString: String;
  // TmpFolderのパス
  TmpPath: String;
begin
  Log('CurStepChanged(' + IntToStr(Ord(CurStep)) + ') called');
  if CurStep = ssPostInstall then
  begin
    // XMLファイルのパスを設定（ProgramData配下）
    XMLFilePath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\BatchPrintService.xml');

    // TStringListを使用してXMLファイルを読み込む
    XMLContent := TStringList.Create;
    try
      if FileExists(XMLFilePath) then
      begin
        // ファイルを読み込む
        XMLContent.LoadFromFile(XMLFilePath);

        // XMLのテキスト内容を一時的な文字列変数に格納
        TempString := XMLContent.Text;
        
        // ポート番号を置換
        StringChangeEx(TempString, '<entry key="port" type="string">3000</entry>', '<entry key="port" type="string">' + PortNo + '</entry>', True);

        // SPPパスワードをBase64にエンコード
        SppPassEncoded := EncodeBase64(SppPass);

        // SPPパスワードを置換
        StringChangeEx(TempString, '<entry key="sppPass" type="string"></entry>', '<entry key="sppPass" type="string">' + SppPassEncoded + '</entry>', True);
        
        // tmpFolderPathを置換
        TmpPath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\tmp');
        StringChangeEx(TempString, '<entry key="tmpFolderPath" type="string">C:\ProgramData\brainsellers\BatchPrint\tmp</entry>', '<entry key="tmpFolderPath" type="string">' + TmpPath + '</entry>', True);
                     
        // 変更後の内容をTStringListに戻す
        XMLContent.Text := TempString;

        // 変更された内容をXMLファイルに保存
        XMLContent.SaveToFile(XMLFilePath);
      end
      else
        MsgBox('XMLファイルが見つかりません。', mbError, MB_OK);
    finally
      XMLContent.Free;  // TStringListのメモリを解放
    end;
    
    // XMLファイルのパスを設定（ProgramData配下）
    XMLFilePath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\BatchPrintService_logConfig.xml');

    // TStringListを使用してXMLファイルを読み込む
    XMLContent := TStringList.Create;
    try
      if FileExists(XMLFilePath) then
      begin
        // ファイルを読み込む
        XMLContent.LoadFromFile(XMLFilePath);

        // XMLのテキスト内容を一時的な文字列変数に格納
        TempString := XMLContent.Text;
       
        // ログファイルパスを置換
        TmpPath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\log');
        StringChangeEx(TempString, '<File value="C:\ProgramData\brainsellers\BatchPrint\log\BatchPrintService.log"></File>', '<File value="' + TmpPath + '\BatchPrintService.log"></File>', True);
                     
        // 変更後の内容をTStringListに戻す
        XMLContent.Text := TempString;

        // 変更された内容をXMLファイルに保存
        XMLContent.SaveToFile(XMLFilePath);
      end
      else
        MsgBox('XMLファイルが見つかりません。', mbError, MB_OK);
    finally
      XMLContent.Free;  // TStringListのメモリを解放
    end;
    
    // XMLファイルのパスを設定（ProgramData配下）
    XMLFilePath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\BizPrintHealthChecker_logConfig.xml');

    // TStringListを使用してXMLファイルを読み込む
    XMLContent := TStringList.Create;
    try
      if FileExists(XMLFilePath) then
      begin
        // ファイルを読み込む
        XMLContent.LoadFromFile(XMLFilePath);

        // XMLのテキスト内容を一時的な文字列変数に格納
        TempString := XMLContent.Text;
       
        // ログファイルパスを置換
        TmpPath := ExpandConstant('{commonappdata}\brainsellers\BatchPrint\log');
        StringChangeEx(TempString, '<File value="C:\ProgramData\brainsellers\BatchPrint\log\BizPrintHealthChecker.log"></File>', '<File value="' + TmpPath + '\BizPrintHealthChecker.log"></File>', True);
                     
        // 変更後の内容をTStringListに戻す
        XMLContent.Text := TempString;

        // 変更された内容をXMLファイルに保存
        XMLContent.SaveToFile(XMLFilePath);
      end
      else
        MsgBox('XMLファイルが見つかりません。', mbError, MB_OK);
    finally
      XMLContent.Free;  // TStringListのメモリを解放
    end;
    
    // タスクスケジューラへ登録
    RegisterScheduledTask();
  end;
end;

// アンインストール時の処理
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  AppPath: String;
  ResultCode: Integer;
begin
  //MsgBox('CurUninstallStepChanged', mbError, MB_OK);
  
  if CurUninstallStep = usUninstall then
  begin
    //MsgBox('usUninstall', mbError, MB_OK);
  
    if IsAppRunning('BatchPrintService.exe') then
    begin
      Log('BatchPrintSerivce.exeが起動中のためプロセスを終了します。');
      //MsgBox('BatchPrintSerivce.exeが起動中のためプロセスを終了します。', mbError, MB_OK);
      
      AppPath := ExpandConstant('{#MyAppExeName}');

      // BatchPrintService.exeのプロセス終了
      Exec('taskkill.exe', '/f /im ' + '"' + AppPath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode)
      Log('ResultCode: ' + IntToStr(ResultCode));
      
      // プロセス終了に失敗した場合
      if (ResultCode <> 0) then
      begin
        Log('BatchPrintSerivce.exeが起動中です。タスクトレイから終了し、再度インストーラを実行してください。');
        MsgBox('BatchPrintSerivce.exeが起動中です。タスクトレイから終了し、再度インストーラを実行してください。', mbError, MB_OK);
        Abort;
      end;
    end;
  end;
  
  if CurUninstallStep = usPostUninstall then
  begin
    //MsgBox('usPostUninstall', mbError, MB_OK);
    
    // タスクスケジューラから削除
    UnregisterScheduledTask();
  end;
  
end;

procedure DeinitializeUninstall();
begin
  //MsgBox('DeinitializeUninstall', mbError, MB_OK);
end;

