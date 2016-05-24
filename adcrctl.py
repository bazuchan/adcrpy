#!/usr/bin/python3

import sys
import serial
import struct
import base64
import math
import time
from optparse import OptionParser

class ADCR_CRC(object):
    TABLE = [ 0x0000, 0x1081, 0x2102, 0x3183, 0x4204, 0x5285, 0x6306, 0x7387, 0x8408, 0x9489, 0xa50a, 0xb58b, 0xc60c, 0xd68d, 0xe70e, 0xf78f]
    def __init__(self):
        self.crc = 0xffff
    def update(self, x):
        for i in bytes(x):
            self.crc = (self.crc >> 4) ^ self.TABLE[(self.crc ^ i) & 0x000f]
            self.crc = (self.crc >> 4) ^ self.TABLE[(self.crc ^ (i >> 4)) & 0x000f]
        return self
    def digest(self):
        return self.crc

def fromcstr(x):
    if x.find(b'\0')==-1:
        return x.decode('utf-8')
    return x[:x.find(b'\0')].decode('utf-8')

class Channel(object):
    TYPES = {0:'P25', 1:'NXDN', 2:'DMR', 3:'dPMR'}
    RTYPES = dict(zip(TYPES.values(), TYPES.keys()))
    def __init__(self, *init):
        if len(init)==6:
            self.channo, self.chname, self.chtype, self.chscan, self.chflags, self.chfreq = init
        elif len(init)==1 and len(init[0]) == 40:
            self.channo = 0
            self.chname = fromcstr(init[0][0:16])
            (self.chtype, self.chscan, self.chflags, self.chfreq) = struct.unpack('BBHI', init[0][16:24])
        elif len(init)==2 and len(init[1]) == 40:
            self.channo = init[0]
            self.chname = fromcstr(init[1][0:16])
            (self.chtype, self.chscan, self.chflags, self.chfreq) = struct.unpack('BBHI', init[1][16:24])
        else:
            self.channo = 0
            self.chname = u''
            self.chtype = 0
            self.chscan = 0
            self.chflags = 0
            self.chfreq = 0
    def tobytes(self, withnum=False):
        r = b''
        if withnum:
            r += bytes(struct.pack('H', self.channo))
        chname = bytes(self.chname.encode('utf-8'))[:16]
        r += chname + (16-len(chname))*b'\0'
        r += bytes(struct.pack('BBHI', self.chtype, self.chscan, self.chflags, self.chfreq))
        r += b'\0'*16
        return r
    def __str__(self):
        return '%u: %uHz [%s] %s' % (self.channo, self.chfreq, self.TYPES[self.chtype], self.chname)

class ADCR25(object):
    MODES = {0:'P25', 1:'DMR', 2:'DMRTS1', 3:'DMRTS2', 4:'YSF', 5:'NXDN48', 6:'NXDN96', 255:'DMR?'}
    def __init__(self, device='/dev/ttyUSB0', debug = False):
        self.device = device
        self.debug = debug
        self.params = None
        self.serial = serial.Serial(self.device, 115200)

    @staticmethod
    def stuff(x):
        return b'\xc0' + x.replace(b'\xdb', b'\xdb\xdd').replace(b'\xc0', b'\xdb\xdc') + b'\xc0'

    @staticmethod
    def unstuff(x):
        return x.strip(b'\xc0').replace(b'\xdb\xdc', b'\xc0').replace(b'\xdb\xdd', b'\xdb')

    def sendpacket(self, data):
        if self.debug:
            self.printpacket(data, 'T')
        self.serial.write(self.stuff(data))

    def readpacket(self):
        # attempt to synchronize
        if self.serial.read(1) != b'\xc0':
            while self.serial.read(1) != b'\xc0':
                pass
            return ''
        # read packet
        pkt = b'\xc0'
        while True:
            pkt += self.serial.read(1)
            if pkt[-1] == 0xc0:
                break
            # optimize
            if len(pkt) == 5 and 0xdb not in pkt and (~(pkt[1]+pkt[3]+pkt[4]))&0xFF == pkt[2]:
                plen = struct.unpack('>BBBH', pkt)[3]
                pkt += self.serial.read(plen + 2)
        rpkt = self.unstuff(pkt)
        if self.debug:
            self.printpacket(rpkt)
        if self.checkpacket(rpkt) != 0:
            return ''
        return rpkt

    E_PKT_INVALID_HEADER = -1
    E_PKT_INVALID_LEN = -2
    E_PKT_INVALID_CRC = -3
    @staticmethod
    def checkpacket(x):
        if (~(x[0]+x[2]+x[3]))&0xFF != x[1]:
            return E_PKT_INVALID_HEADER
        if struct.unpack('>BBH', x[:4])[2] != len(x)-6:
            return E_PKT_INVALID_LEN
        if ADCR_CRC().update(x[:-2]).digest() != struct.unpack('>H', x[-2:])[0]:
            return E_PKT_INVALID_CRC
        return 0

    @staticmethod
    def buildpacket(code, payload):
        r = bytes([code, (~(code+(len(payload)>>8)+(len(payload)&0xFF)))&0xFF, len(payload)>>8, len(payload)&0xFF])
        r += bytes(payload)
        r += bytes(struct.pack('>H', ADCR_CRC().update(r).digest()))
        return r

    @staticmethod
    def decodepacket(x):
        if x[0]==10:
            if x[2] == 0 and x[3] == 0:
                return 'Set freq ACK'
            else:
                return 'Set freq %u' % struct.unpack('I', x[4:8])[0]
        if x[0]==0:
            if x[2] == 0 and x[3] == 0:
                return 'Get hw info'
            else:
                return 'Get hw info, fw ver: %s, s/n: %s' % ('%u.%u' % struct.unpack('BB', x[12:14]), base64.b16encode(x[4:12]))
        if x[0]==2:
            if x[2] == 0 and x[3] == 2:
                return 'Get mem, chan %u' % struct.unpack('H', x[4:6])
            else:
                return 'Get mem resp: %s' % Channel(x[4:44])
        if x[0]==3:
            if x[2] == 0 and x[3] == 0:
                return 'Set mem ACK'
            else:
                return 'Set mem chan %u, data %s' % (struct.unpack('H', x[4:6])[0], Channel(x[6:46]))
        if x[0]==8:
            if x[2] == 0 and x[3] == 0:
                return 'Set chan ACK'
            else:
                return 'Set chan chan %u, data %s' % (struct.unpack('H', x[4:6])[0], Channel(x[6:46]))
        if x[0]==6:
            if x[2] == 0 and x[3] == 0:
                return 'Select chan ACK'
            else:
                return 'Select chan %u, flush %u' % struct.unpack('BB', x[4:6])
        if x[0]==9:
            if x[2] == 0 and x[3] == 8 and x[4:12]==b'\x00'*8:
                return 'Set scan ACK'
            else:
                return 'Set scan freq %u, wt %u, mode %u' % struct.unpack('IHBB', x[4:12])[:3]
        if x[0]==1:
            if x[2] == 0 and x[3] > 20:
                freq, inrx, smode, dbm, uv, pl, encrypted, src, dst = ADCR25.decode_rssi(x[4:])
                return 'RSSI freq %u, %s, signal: %u, pl: %u, uv: %u, rx: %u, encr: %u, src-dst: %s -> %s' % (freq, smode, dbm, pl, uv, inrx, encrypted, src, dst)
        return 'Code %u, Data %s' % (x[0], repr(x[4:-2]))

    @staticmethod
    def printpacket(pkt, direction='R'):
        sys.stderr.write('%sX> %s\n' % (direction, ADCR25.decodepacket(pkt)))
        #sys.stderr.write('%sXR> %s\n' % (direction, repr(pkt)))

    def reset_hw(self):
        # Erases memory too!!!
        pkt = self.buildpacket(54, struct.pack('BB', 170, 85))
        self.sendpacket(pkt)

    def reset_sw(self):
        # Hangs!!!
        pkt = self.buildpacket(52, b'')
        self.sendpacket(pkt)

    def cmd(self, code, payload):
        pkt = self.buildpacket(code, payload)
        self.sendpacket(pkt)
        while True:
            r = self.readpacket()
            if r and r[0] == code:
                return r

    def set_freq(self, freq, mode=0):
        r = self.cmd(10, struct.pack('IBB', freq, mode, 0))
        if r[0] == 10 and r[2] == 0 and r[3] == 0:
            return True
        return False

    def get_mem(self, channo):
        r = self.cmd(2, struct.pack('H', channo))
        if r[0] == 2 and r[2] == 0 and r[3] == 40:
            chan = Channel(channo, r[4:44])
            return chan
        return None

    def set_mem(self, chan):
        r = self.cmd(3, chan.tobytes(withnum=True))
        if r[0] == 3 and r[2] == 0 and r[3] == 0:
            return True
        return False

    def set_chan(self, chan):
        r = self.cmd(8, chan.tobytes(withnum=True))
        if r[0] == 8 and r[2] == 0 and r[3] == 0:
            return True
        return False

    def select_chan(self, channo, flush=0):
        r = self.cmd(6, struct.pack('BB', channo, flush))
        if r[0] == 6 and r[2] == 0 and r[3] == 0:
            return True
        return False

    @staticmethod
    def decode_rssi(pkt):
        inrx = pkt[0] == 2
        mode = pkt[7]
        encrypted = ( mode == 0 and pkt[1] != 128 ) or ( mode in [1,2,3] and pkt[2]&8 ) or ( mode in [5,6] and pkt[1] !=0 )
        isgroup = ( mode == 0 and pkt[6] != 1 ) or ( mode in [1,2,3,4] and not pkt[2]&16 ) or ( mode in [5,6] and pkt[6]>>5 in [0,1] )
        dbm = struct.unpack('b', pkt[3:4])[0]
        smode = ADCR25.MODES[mode]
        freq = struct.unpack('I', pkt[20:24])[0]
        if mode == 4:
            src, dst = fromcstr(pkt[24:34]), fromcstr(pkt[34:44])
        else:
            src, dst = ['%u'%i for i in struct.unpack('II', pkt[12:20])]
        pl = 0
        if len(pkt)>=44:
            pls = struct.unpack('IH', pkt[44:50])
            if pls[0]!=0:
                pl = math.floor(float(pls[1]) / float(pls[0]) * 100.0)
        uv = -99
        if len(pkt)>=50:
            uvs = struct.unpack('H', pkt[50:52])
            if uvs[0]!=0:
                uv = round(20.0 * math.log10(float(uvs[0]) / 65536.0))
        return (freq, inrx, smode, dbm, uv, pl, encrypted, src, dst)

    def get_params(self):
        r = self.cmd(4, b'\x00\x00')
        if r[0] == 4 and r[2] == 0 and r[3] == 172:
            self.params = r[4:]
            return True
        return False

    def set_params(self):
        if not self.params:
            return False
        r = self.cmd(5, self.params)
        if r[0] == 5 and r[2] == 0 and r[3] == 0:
            return True
        return False

    def get_info(self):
        BANDS = {1:'UHF', 4:'VHF', 5:'800MHz'}
        time.sleep(1)
        r = self.cmd(0, b'')
        if r[0] == 0 and r[2] == 0 and r[3] == 12:
            sn = base64.b16encode(r[4:12])
            ver = '%u.%u' % struct.unpack('BB', r[12:14])
            band = struct.unpack('H', r[14:16])[0]
            return (sn, ver, BANDS[band])
        return ('','', 0)

    def scan_set_freq(self, freq, wt, mode):
        r = self.cmd(9, struct.pack('IHBB', freq, wt, mode, 0))
        if r[0] == 9 and r[2] == 0 and r[3] == 8:
            return True
        return False

    def scan(self, sfreq, efreq, step, timeout=800, modes=[0]):
        freqs = {}
        for freq in range(sfreq, efreq+1, step):
            for mode in modes:
                pkt = self.buildpacket(9, struct.pack('IHBB', freq, int(timeout/10), mode, 0))
                self.sendpacket(pkt)
                while True:
                    r = self.readpacket()
                    if r and r[0] == 1:
                        rssi = self.decode_rssi(r[4:])
                        if rssi[1]:
                            f = (rssi[0], rssi[2])
                            if f in freqs.keys():
                                freqs[f] = max(freqs[f], rssi[3])
                            else:
                                freqs[f] = rssi[3]
                    if r and r[0] == 9:
                        break
        return freqs

def checkcrc(x):
    return ADCR_CRC().update(x[:-2]).digest() == struct.unpack('>H', x[-2:])[0]

def checkheadersum(x):
    return (~(x[0]+x[2]+x[3]))&0xFF == x[1]

if __name__ == '__main__':
    usage = 'usage: %prog [options] <cmd> [params] ...'
    parser = OptionParser(usage=usage)
    parser.add_option('-d', '--device', dest='device', help='Serial port device', default='/dev/ttyUSB0')
    (options, args) = parser.parse_args()

    adcr = ADCR25(options.device, debug=True)
    print(adcr.scan(451800000, 452000000, 25000, 2000, [0,1,255]))
    #adcr.scan_set_freq(451825000, 80, 0)
    #adcr.scan_set_freq(451825000, 80, 1)
    #adcr.scan_set_freq(451825000, 80, 4)
    #adcr.reset_sw()
    #print(adcr.get_params())
    #print(adcr.set_params())
    adcr.select_chan(0)
    print(adcr.get_info())
    #print(adcr.readpacket())
    #print(adcr.readpacket())
    #print(adcr.readpacket())
    #adcr.set_freq(451825000)
    ch = Channel(0, 'test', 0, 1, 0, 451825000)
    #print(adcr.get_mem(1))
    #q = adcr.code4()
    #print(struct.unpack('H', q[:2]))
    #print(struct.unpack('I', q[12:16]))
    #print(struct.unpack('I', q[16:20]))
    #print(struct.unpack('I', q[20:24]))
    #print(struct.unpack('I', q[24:28]))
    #print(struct.unpack('I', q[28:32]))
    #print(struct.unpack('BB', q[0][8:10]))
    #adcr.set_mem(ch)
    #for i in range(0, 50):
    #    print(adcr.get_mem(i))
    #r = adcr.cmd(0, b'')

