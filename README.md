# RawHIDSampleAppForQMK

QMK のRaw HID機能にPythonとC#(.net)からアクセスする最小限の動作確認サンプルアプリです。  

[![thumnail](https://pbs.twimg.com/ext_tw_video_thumb/1316339661606772736/pu/img/6EjbYjd2cww-kjU1?format=jpg&name=900x900)](https://twitter.com/marksard/status/1316339717886021633)  
（画像クリックで動画ページへ）

## QMKファームウェアの準備

ターゲットとなる任意のキーボードの任意のキーマップに以下のコードを加えてmakeし、ファームウェアをキーボードに書き込む。  

QMK Firmware 0.10.18で確認。  

rules.mk  

```makefile

# RAW HID有効化
RAW_ENABLE = yes

# QMK Toolboxにデバッグ文字列を
# 表示する場合は以下も追加する
CONSOLE_ENABLE = yes

```

keymap.c

```c

// これをincludeするとraw_hid_receiveとraw_hid_sendが使える
#include "raw_hid.h"

void keyboard_post_init_user(void) {
  // CONSOLE_ENABLE = yesでこれを有効にしたらこれもtrueにしとく
  debug_enable = true;
}

// 返送用の適当なバッファ
char hello[32] = "Hello, QMK Raw HID world!     /0";

// 受信イベント関数
void raw_hid_receive(uint8_t *data, uint8_t length) {

  // print文はCONSOLE_ENABLE = yesの時有効
  print("raw_hid_receive\n");
  // 32固定でやってくるのでなくてもOKのハズ
  if (length == 32) {
    print("length 32\n");
    if (data[0] == 'h') {
      print("recieve command: h\n");
      // LEDの光り方を変更
      rgblight_step_noeeprom();

      // 文字を打鍵したことにも出来ちゃう
      // SEND_STRING(hello);
    }
  }

  // 無理やり文字列を押し込めて送信するテスト
  raw_hid_send((uint8_t*)hello, 32);

// テスト用：受信したものをそのまま返送する
//   raw_hid_send(data, length);
}

```

## pythonアプリ

PCから実行するpythonアプリはhidapiライブラリが必要なのでpipからインストールする。  

python 3.8系で確認。  

``` pip install hidapi ```

このプロジェクトのpythonフォルダにあるqmk_rawhid_test.pyの、VENDOR_IDとPRODUCT_IDをターゲットとなる任意のキーボードフォルダのルートか、rev2などのフォルダにあるconfig.hの値に合わせる。
  
### 実施

qmk_rawhid_test.pyを実行する。  

``` python qmk_rawhid_test.py ```

うまく行けばキー入力待ちになるので、hを入力してEnterする。  
うまく行けばキーボードのLEDが変化し、コンソール画面に

``` Hello, QMK Raw HID world!     /0 ```

が表示される（キーボードのRAW HIDから送信された文字列です）  

## WPF C#(.net)アプリ

このプロジェクトのdotnetフォルダにあるソリューションを開きます。  

HidLibraryをnuget経由で使用していますが、nugetパッケージがDLされない場合は一度削除して入れ直してください。
.net 4.7.1で確認  
HidLIbraryはver 3.3.40で確認  

ビルドして起動したらVENDOR_IDとPRODUCT_IDをターゲットとなる任意のキーボードフォルダのルートか、rev2などのフォルダにあるconfig.hの値合わせて、Startボタンを押します。  

うまく行けばキーボードのLEDが変化し、startボタンの右のエリアに  

``` Hello, QMK Raw HID world!　```

が表示される（キーボードのRAW HIDから送信された文字列です）  

Good Luck!  
