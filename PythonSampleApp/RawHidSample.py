import hid
import time

# HIDを列挙
def enum_hid():
    # Check all  usb device
    for d in hid.enumerate(0, 0):
        keys = d.keys()
        #keys.sort()
        for key in keys:
            print ("%s : %s" % (key, d[key]))
        print ("")

# HIDを検索してパスをもらう
def findHIDPath(vid, pid, usagepage, usage):
    # Check all  usb device
    for d in hid.enumerate(0, 0):
        keys = d.keys()
        if (d['vendor_id'] == vid) and (d['product_id'] == pid) and (d['usage_page'] == usagepage) and (d['usage'] == usage):
            for key in keys:
                print ("%s : %s" % (key, d[key]))
            return d['path']

# Treadstone32のファームにRawHIDの機能を追加したので、そのVIDとPIDを指定
VENDOR_ID = 0xFEED
PRODUCT_ID = 0xDFA5

h = hid.device()
# VIDとPIDだと複数見つかるため、QMKから指示された設定を検索に使用
# → usagepage:0xFF60、usage:0x61
path = findHIDPath(VENDOR_ID, PRODUCT_ID, 0xFF60, 0x61)
# 取得した機器のパスでオープン
h.open_path(path)
h.get_input_report

# QMKのRawHid機能に送信可能なバッファ数は32バイトまで
# → usb_description.hにバッファ数の記載　#define RAW_EPSIZE 32
send_buf = [0] * 32

# コマンド入力
input_val = input()

# 配列にいれてく
enc_in_val = input_val.encode()
print(enc_in_val)
for i in range(0, len(send_buf)):
    if len(enc_in_val) > i:
        send_buf[i] = enc_in_val[i]

# バッファ先頭はUSBの仕様に定められた(?)ReportIDのため挿入。ReportIDはなんでもよさそう
send_buf.insert(0, 0)

# 送信バッファを表示して送信
print(send_buf)
ret = h.write(send_buf)

# 受信確認
recv = h.read(32)
# データは文字列なので復号（数値リスト→char変換→連結）
rp = map(chr, recv)
print(''.join(rp))
