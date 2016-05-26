#!/usr/bin/python3

import sys
import serial
import struct
import base64
import math
import time
import csv
import argparse
import os.path

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
    def __init__(self, *init):
        if len(init)==6:
            self.channo, self.chname, self.chmode, self.chscan, self.chflags, self.chfreq = init
        elif len(init)==1 and len(init[0]) == 40:
            self.channo = 0
            self.chname = fromcstr(init[0][0:16])
            (self.chmode, self.chscan, self.chflags, self.chfreq) = struct.unpack('BBHI', init[0][16:24])
        elif len(init)==2 and len(init[1]) == 40:
            self.channo = init[0]
            self.chname = fromcstr(init[1][0:16])
            (self.chmode, self.chscan, self.chflags, self.chfreq) = struct.unpack('BBHI', init[1][16:24])
        else:
            self.channo = -1
            self.chname = u''
            self.chmode = 0
            self.chscan = 0
            self.chflags = 0
            self.chfreq = 0
    def tobytes(self, withnum=False):
        r = b''
        if withnum:
            r += bytes(struct.pack('H', self.channo))
        chname = bytes(self.chname.encode('utf-8'))[:16]
        r += chname + (16-len(chname))*b'\0'
        r += bytes(struct.pack('BBHI', self.chmode, self.chscan, self.chflags, self.chfreq))
        r += b'\0'*16
        return r
    def __str__(self):
        return '%u: %uHz [%s] "%s" %u' % (self.channo, self.chfreq, ADCR25.MODES[self.chmode], self.chname, self.chscan)
    def tocsvline(self):
        return [self.channo, self.chname, self.chmode, self.chscan, self.chflags, self.chfreq]
    csvheader = ['CH Num', 'CH Name', 'Type', 'Scan', 'Flags', 'Frequency']

class Params(object):
    def __init__(self, bparams):
        if len(bparams) == 172:
            self.params = bparams
        else:
            raise ValueError('Wrong paramaters length')
    def decode(self):
        r = {}
        r['vol'] = self.params[0]
        r['mode'] = self.params[2]
        r['equalizer'] = list(self.params[4:12])
        r['freq'] = struct.unpack('I', self.params[12:16])[0]
        r['freq2'] = struct.unpack('I', self.params[16:20])[0]
        return r
    def tobytes(self):
        return self.params
    def set_volume(self, vol):
        self.params = struct.pack('B', vol) + self.params[1:]

class ADCR25(object):
    MODES = {0:'P25', 1:'DMR', 2:'DMRTS1', 3:'DMRTS2', 4:'YSF', 5:'NXDN48', 6:'NXDN96'}
    RMODES = dict(zip(MODES.values(), MODES.keys()))
    def __init__(self, device='/dev/ttyUSB0', debug = False):
        self.device = device
        self.debug = debug
        self.params = None
        self.last = 0
        self.rssi_timeout = 1
        self.serial = serial.Serial(self.device, 115200)

    @staticmethod
    def stuff(x):
        return b'\xc0' + x.replace(b'\xdb', b'\xdb\xdd').replace(b'\xc0', b'\xdb\xdc') + b'\xc0'

    @staticmethod
    def unstuff(x):
        return x.strip(b'\xc0').replace(b'\xdb\xdc', b'\xc0').replace(b'\xdb\xdd', b'\xdb')

    @staticmethod
    def sortchannels(x):
        if x is list:
            x = dict(zip([ch.channo for ch in x], x))
        t = list(x.keys())
        t.sort()
        return [x[n] for n in t]

    def sendpacket(self, data):
        if self.debug:
            self.printpacket(data, 'T')
        self.serial.write(self.stuff(data))
        self.last = time.time()

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
                rssi = ADCR25.decode_rssi(x[4:])
                return 'RSSI ' + ', '.join(['%s: %s' % (i, rssi[i]) for i in ['inrx', 'freq', 'smode', 'dbm', 'uv', 'pl', 'encrypted', 'isgroup', 'src', 'dst']])
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

    def set_ram(self, chan):
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
        r = {}
        r['inrx'] = pkt[0] == 2
        r['mode'] = pkt[7]
        if r['mode'] == 255:
            r['mode'] = 1
        r['encrypted'] = ( r['mode'] == 0 and pkt[1] != 128 ) or ( r['mode'] in [1,2,3] and pkt[2]&8 ) or ( r['mode'] in [5,6] and pkt[1] !=0 )
        r['isgroup'] = ( r['mode'] == 0 and pkt[6] != 1 ) or ( r['mode'] in [1,2,3,4] and not pkt[2]&16 ) or ( r['mode'] in [5,6] and pkt[6]>>5 in [0,1] )
        r['dbm'] = struct.unpack('b', pkt[3:4])[0]
        r['smode'] = ADCR25.MODES[r['mode']]
        r['freq'] = struct.unpack('I', pkt[20:24])[0]
        if r['mode'] == 4:
            r['src'], r['dst'] = fromcstr(pkt[24:34]), fromcstr(pkt[34:44])
        else:
            r['src'], r['dst'] = ['%u'%i for i in struct.unpack('II', pkt[12:20])]
        r['pl'] = 0
        if len(pkt)>=44:
            pls = struct.unpack('IH', pkt[44:50])
            if pls[0]!=0:
                r['pl'] = math.floor(float(pls[1]) / float(pls[0]) * 100.0)
        r['uv'] = -99
        if len(pkt)>=50:
            uvs = struct.unpack('H', pkt[50:52])
            if uvs[0]!=0:
                r['uv'] = round(20.0 * math.log10(float(uvs[0]) / 65536.0))
        return r

    def get_params(self):
        r = self.cmd(4, b'\x00\x00')
        if r[0] == 4 and r[2] == 0 and r[3] == 172:
            return Params(r[4:176])
        return False

    def set_params(self, params):
        r = self.cmd(5, params.tobytes())
        if r[0] == 5 and r[2] == 0 and r[3] == 0:
            return True
        return False

    def get_info(self):
        BANDS = {1:'UHF', 4:'VHF', 5:'800MHz'}
        time.sleep(1)
        r = self.cmd(0, b'')
        if r[0] == 0 and r[2] == 0 and r[3] == 12:
            sn = base64.b16encode(r[4:12]).decode('utf-8')
            ver = '%u.%u' % struct.unpack('BB', r[12:14])
            band = struct.unpack('H', r[14:16])[0]
            return (sn, ver, BANDS[band])
        return ('','', 0)

    def scan_set_freq(self, freq, wt, mode):
        r = self.cmd(9, struct.pack('IHBB', freq, wt, mode, 0))
        if r[0] == 9 and r[2] == 0 and r[3] == 8:
            return True
        return False

    def scan(self, sfreq, efreq, step, timeout=800, modes=[0], cycles=1, printprogress=False):
        freqs = {}
        ipkt = self.buildpacket(0, b'')
        try:
            for cycle in range(0, cycles):
                for freq in range(sfreq, efreq+1, step):
                    if printprogress:
                        sys.stderr.write('Scanning frequency %u ... '%freq)
                        sys.stderr.flush()
                    for mode in modes:
                        pkt = self.buildpacket(9, struct.pack('IHBB', freq, int(timeout/10), mode, 0))
                        self.sendpacket(pkt)
                        while True:
                            if time.time()-self.last>self.rssi_timeout:
                                self.sendpacket(ipkt)
                            r = self.readpacket()
                            if r and r[0] == 1:
                                rssi = self.decode_rssi(r[4:])
                                if rssi['inrx']:
                                    rf, rm = rssi['freq'], rssi['mode']
                                    if rf in freqs.keys() and rm in freqs[rf].keys():
                                        freqs[rf][rm] = max(freqs[rf][rm], rssi['dbm'])
                                    elif rf in freqs.keys():
                                        freqs[rf][rm] = rssi['dbm']
                                    else:
                                        freqs[rf] = {rm:rssi['dbm']}
                            if r and r[0] == 9:
                                break
                    if printprogress:
                        if freq in freqs.keys():
                            sys.stderr.write('found ' + ', '.join(['%s: %d dbm'%(self.MODES[i], freqs[freq][i]) for i in freqs[freq].keys()])+'\n')
                        else:
                            sys.stderr.write('\n')
        except KeyboardInterrupt:
            sys.stderr.write('\n')
        return freqs

    def chanscan(self, channels, timeout=800, printprogress=False):
        ipkt = self.buildpacket(0, b'')
        while True:
            for chan in channels:
                if printprogress:
                    print('\rScanning %s'%chan, end='')
                    print("\033[K", end='')
                pkt = self.buildpacket(9, struct.pack('IHBB', chan.chfreq, int(timeout/10), chan.chmode, 0))
                self.sendpacket(pkt)
                while True:
                    if time.time()-self.last>self.rssi_timeout:
                        self.sendpacket(ipkt)
                    r = self.readpacket()
                    if r and r[0] == 1:
                        rssi = self.decode_rssi(r[4:])
                        if rssi['inrx']:
                            return (rssi['freq'], rssi['mode'])
                    if r and r[0] == 9:
                        break

    def write_csv(self, fname, channels):
        with open(fname, 'w', newline='') as csvfile:
            writer = csv.writer(csvfile, delimiter=';', quotechar='"', quoting=csv.QUOTE_NONNUMERIC)
            writer.writerow(Channel.csvheader)
            for chan in channels:
                writer.writerow(chan.tocsvline())

    def read_csv(self, fname, ignorenums=False):
        channels = {}
        with open(fname, newline='') as csvfile:
            reader = csv.reader(csvfile, delimiter=';', quotechar='"')
            if reader.__next__() != Channel.csvheader:
                return {}
            for row in reader:
                chan = Channel(int(row[0]), row[1], int(row[2]), int(row[3]), int(row[4]), int(row[5]))
                if chan.channo == -1:
                    continue
                if ignorenums:
                    chan.channo = len(channels)
                channels[chan.channo] = chan
        return channels

    def mem2csv(self, fname):
        channels = []
        for chno in range(0, 50):
            channels.append(self.get_mem(chno))
        self.write_csv(fname, channels)

    def csv2mem(self, fname, ignorenums=False):
        channels = self.read_csv(fname, ignorenums)
        if not channels:
            return 0
        r = 0
        for chno in channels.keys():
            if chno>=0 and chno<50:
                if self.set_mem(channels[chno]):
                    r += 1
        return r

def checkcrc(x):
    return ADCR_CRC().update(x[:-2]).digest() == struct.unpack('>H', x[-2:])[0]

def checkheadersum(x):
    return (~(x[0]+x[2]+x[3]))&0xFF == x[1]

class MyHelpFormatter(argparse.HelpFormatter):
    def __init__(self, *kc, **kv):
        kv['width'] = 1000
        kv['max_help_position'] = 1000
        super(MyHelpFormatter, self).__init__(*kc, **kv)

if __name__ == '__main__':
    commands = ['info', 'recv', 'tune', 'scan', 'list', 'chan', 'scanmem', 'edit', 'mem2csv', 'csv2mem', 'csv2ram']

    usage = '%(prog)s [-d device] <command> [command options] ...'
    p = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter, add_help=False)
    p.add_argument('command', help='One of: ' + ', '.join(commands), nargs='?')
    p.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
    p.add_argument('-h', '--help', dest='cmd', help='Show help for specified command', nargs='?', default=argparse.SUPPRESS)
    args = p.parse_args([i for i in sys.argv if i in ['-d', '--device', '-h', '--help']+commands])
    if not args.command and not ( args.__contains__('cmd') and args.cmd ):
        p.print_help()
        sys.exit(0)

    curcmd = 'scan'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s [scan options] <start freq MHz> <end freq MHz>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('start', help='Start frequency in MHz', type=float)
        parser.add_argument('end', help='End frequency in MHz', type=float)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-t', '--step', help='Scan step in KHz [default: %(default)s]', type=float, default=12.5)
        parser.add_argument('-w', '--wait', help='Time to spend on each frequency and mode [default: %(default)s]', type=int, default=800)
        parser.add_argument('-c', '--cycles', help='Number of times loop through frequencies [default: %(default)s]', type=int, default=1)
        parser.add_argument('-m', '--modes', help='Comma separated list of communication modes to scan [default: %(default)s]', default='P25,DMR,YSF,NXDN')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Also write results to CSV file <file>')
        parser.add_argument('-a', '--append', action='store_true', help='Do not overwrite CSV file but append found result', default=False)
        args = parser.parse_args()

        modes = []
        for mode in args.modes.split(','):
            modes.extend({'P25':[0], 'DMR':[1,255], 'YSF':[4], 'NXDN':[5,6]}[mode])
        adcr = ADCR25(args.device)
        res = adcr.scan(int(args.start*10**6), int(args.end*10**6), int(args.step*10**3), timeout=args.wait, modes=modes, cycles=args.cycles, printprogress=True)
        print('Found:')
        if args.append and args.csvfile and os.path.exists(args.csvfile):
            channels = adcr.read_csv(args.csvfile)
        else:
            channels = {}
        for freq in res.keys():
            for mode in res[freq].keys():
                print('%u %s %d dbm' % (freq, ADCR25.MODES[mode], res[freq][mode]))
                if args.csvfile and (freq, mode) not in [(ch.chfreq, ch.chmode) for ch in channels.values()]:
                    channels[len(channels)] = Channel(len(channels), '%u %s' % (freq,ADCR25.MODES[mode]), mode, 1, 0, freq)
        if args.csvfile:
            print('Saving scan results to %s'%args.csvfile)
            adcr.write_csv(args.csvfile, ADCR25.sortchannels(channels))
        sys.exit(0)

    curcmd = 'tune'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s <freq MHz> <mode>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('freq', help='frequency in MHz', type=float)
        parser.add_argument('mode', help='communication mode [default: %(default)s]', nargs='?', default='P25', choices=ADCR25.RMODES.keys())
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        adcr.set_freq(int(args.freq*10**6), ADCR25.RMODES[args.mode])
        print('Tuned to %3.9g Mhz, %s' % (args.freq, args.mode))
        sys.exit(0)

    curcmd = 'chan'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] [-f csv file] %s <channel number>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('channo', help='channel number', type=int)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Use CSV file instead of receiver memory')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        if args.csvfile:
            channels = adcr.read_csv(args.csvfile)
            if not args.channo in channels:
                sys.stderr.write('No such channel: %u\n'%args.channo)
                sys.exit(1)
            chan = channels[args.channo]
            adcr.set_freq(chan.chfreq, chan.chmode)
        else:
            if args.channo>=0 and args.channo<50:
                chan = adcr.get_mem(args.channo)
                adcr.select_chan(args.channo)
            else:
                sys.stderr.write('No such channel: %u\n'%args.channo)
                sys.exit(1)
        print('Set channel to: %s'%chan)
        sys.exit(0)

    curcmd = 'info'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        (sn, ver, band) = adcr.get_info()
        par = adcr.get_params().decode()
        print('Band: %s, Serial Number: %s, Firmware version: %s' % (band, sn, ver))
        print('Current frequency: %3.9g Mhz, Mode: %s, Volume: %u' % (par['freq']/1000000.0, adcr.MODES[par['mode']], par['vol']))
        sys.exit(0)

    curcmd = 'mem2csv'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s <-f csv file>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Dump memory contents to CSV file', required=True)
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        adcr.mem2csv(args.csvfile)
        print('Dumping memory contents to file: %s'%args.csvfile)
        sys.exit(0)

    curcmd = 'csv2mem'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s <-f csv file>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Load CSV file into memory', required=True)
        parser.add_argument('-i', '--ignore-channel-numbers', dest='ignore', help='Ignore channel numbers from CSV file', action='store_true', default=False)
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        adcr.csv2mem(args.csvfile, args.ignore)
        print('Loading file: %s to memory'%args.csvfile)
        sys.exit(0)

    curcmd = 'csv2ram'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s <-f csv file>' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Load CSV file into RAM', required=True)
        parser.add_argument('-i', '--ignore-channel-numbers', dest='ignore', help='Ignore channel numbers from CSV file', action='store_true', default=False)
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        adcr.csv2ram(args.csvfile, args.ignore)
        print('Loading file: %s to memory'%args.csvfile)
        sys.exit(0)

    curcmd = 'list'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] [-f csv file] %s' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Use CSV file instead of receiver memory')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        if args.csvfile:
            channels = adcr.read_csv(args.csvfile)
        else:
            channels = {}
            for chno in range(0, 50):
                channels[chno] = adcr.get_mem(chno)
        print('Channels:')
        for ch in ADCR25.sortchannels(channels):
            print(ch)
        sys.exit(0)

    curcmd = 'edit'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] [-f csv file] %s <channel number> <freq MHz> mode [name]' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('channo', help='channel number', type=int)
        parser.add_argument('freq', help='frequency in MHz', type=float)
        parser.add_argument('mode', help='communication mode', choices=ADCR25.RMODES.keys())
        parser.add_argument('name', help='channel name', nargs='?')
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Use CSV file instead of receiver memory')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        if args.name:
            name = args.name
        else:
            name = '%u %s' % (int(args.freq*10**6), args.mode)
        if args.csvfile:
            channels = adcr.read_csv(args.csvfile)
            channels[args.channo] = Channel(args.channo, name, adcr.RMODES[args.mode], 1, 0, int(args.freq*10**6))
            adcr.write_csv(args.csvfile, channels.values())
        else:
            if args.channo>=0 and args.channo<50:
                adcr.set_mem(Channel(args.channo, name, adcr.RMODES[args.mode], 1, 0, int(args.freq*10**6)))
            else:
                sys.stderr.write('No such channel: %u\n'%args.channo)
                sys.exit(1)
        print('Channel %u set to %3.9g MHz %s' % (args.channo, args.freq, args.mode))
        sys.exit(0)

    curcmd = 'recv'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] %s' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        ipkt = adcr.buildpacket(0, b'')
        try:
            while True:
                if time.time()-adcr.last>adcr.rssi_timeout:
                    adcr.sendpacket(ipkt)
                r = adcr.readpacket()
                if not r or r[0] != 1:
                    continue
                rssi = adcr.decode_rssi(r[4:])
                print('\r%3.9gMHz %s %ddbm' % (rssi['freq']/1000000.0, rssi['smode'], rssi['dbm']), end='')
                if rssi['inrx']:
                    print('  %s -> %s' % (rssi['src'], rssi['dst']), end='')
                print("\033[K", end='')
        except KeyboardInterrupt:
            pass
        sys.exit(0)

    curcmd = 'scanmem'
    if args.command == curcmd or ( args.__contains__('cmd') and args.cmd == curcmd ):
        usage = '%%(prog)s [-d device] [-f csv file] %s [options]' % curcmd
        parser = argparse.ArgumentParser(usage=usage, formatter_class=MyHelpFormatter)
        parser.add_argument('command', help=curcmd)
        parser.add_argument('-d', '--device', help='Serial port device', default='/dev/ttyUSB0')
        parser.add_argument('-f', '--csv-file', dest='csvfile', help='Use CSV file instead of receiver memory')
        parser.add_argument('-w', '--wait', help='Time (ms) to spend on each frequency and mode [default: %(default)s]', type=int, default=500)
        parser.add_argument('-l', '--listen', help='Time (s) after signal is gone to start scaning again [default: %(default)s]', type=int, default=15)
        args = parser.parse_args()

        adcr = ADCR25(args.device)
        ipkt = adcr.buildpacket(0, b'')
        if args.csvfile:
            channels = adcr.read_csv(args.csvfile)
        else:
            channels = {}
            for chno in range(0, 50):
                channels[chno] = adcr.get_mem(chno)

        toscan = [ch for ch in ADCR25.sortchannels(channels) if ch.chscan]
        print('Scanning channels:')
        for ch in toscan:
            print(ch)
        print()

        try:
            while True:
                (freq, mode) = adcr.chanscan(toscan, args.wait, True)
                lstime = time.time()
                adcr.set_freq(freq, mode)
                while True:
                    t = time.time()
                    if t-lstime>args.listen:
                        break
                    if t-adcr.last>adcr.rssi_timeout:
                        adcr.sendpacket(ipkt)
                    r = adcr.readpacket()
                    if not r or r[0] != 1:
                        continue
                    rssi = adcr.decode_rssi(r[4:])
                    print('\r%3.9gMHz %s %ddbm' % (rssi['freq']/1000000.0, rssi['smode'], rssi['dbm']), end='')
                    if rssi['inrx']:
                        lstime = time.time()
                        print(' %s -> %s' % (rssi['src'], rssi['dst']), end='')
                    else:
                        print('  left %us'%int(args.listen+lstime-time.time()), end='')
                    print("\033[K", end='')
        except KeyboardInterrupt:
            pass
        sys.exit(0)

