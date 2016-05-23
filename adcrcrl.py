#!/usr/bin/python3

import sys
import serial
import struct
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

class ADCR25(object):
    def __init__(self, device='/dev/ttyUSB0'):
        self.device = device
        self.serial = serial.Serial(self.device, 115200)

    @staticmethod
    def stuff(x):
        return b'\xc0' + x.replace(b'\xdb', b'\xdb\xdd').replace(b'\xc0', b'\xdb\xdc') + b'\xc0'

    @staticmethod
    def unstuff(x):
        return x.strip(b'\xc0').replace(b'\xdb\xdc', b'\xc0').replace(b'\xdb\xdd', b'\xdb')

    def sendpacket(self, data):
        self.serial.write(self.stuff(data))

    def readpacket(self):
        # attempt to synchronize
        if self.serial.read(1) != b'0xc0':
            while self.serial.read(1) != b'0xc0':
                pass
            return ''
        # read packet
        pkt = b'0xc0'
        while True:
            pkt += self.serial.read(1)
            if pkt[-1] == b'0xc0':
                break
            # optimize
            if len(pkt) == 5 and b'\xdb' not in pkt and (~(pkt[1]+pkt[3]+pkt[4]))&0xFF == pkt[2]:
                plen = struct.unpack('>BBBH', pkt)[4]
                pkt += self.serial.read(plen + 2)
        return unstuff(pkt)

    E_PKT_INVALID_HEADER = -1
    E_PKT_INVALID_LEN = -2
    E_PKT_INVALID_CRC = -3
    @staticmethod
    def checkpacket(x):
        if (~(x[0]+x[2]+x[3]))&0xFF != x[1]:
            return E_PKT_INVALID_HEADER
        if struct.unpack('>BBH', x[:4])[2] != len(x)-6
            return E_PKT_INVALID_LEN
        if ADCR_CRC().update(x[:-2]).digest() != struct.unpack('>H', x[-2:])[0]
            return E_PKT_INVALID_CRC
        return 0

    @staticmethod
    def buildpacket(code, payload):
        r = bytes([code, , len(payload)&])


    def cmd(self, code, payload):


def checkcrc(x):
    return ADCR_CRC().update(x[:-2]).digest() == struct.unpack('>H', x[-2:])[0]

def checkheadersum(x):
    return (~(x[0]+x[2]+x[3]))&0xFF == x[1]

if __name__ == '__main__':
    usage = 'usage: %prog [options] <cmd> [params] ...'
    parser = OptionParser(usage=usage)
    parser.add_option('-d', '--device', dest='device', help='Serial port device', default='/dev/ttyUSB0')
    (options, args) = parser.parse_args()
    
    adcr = ADCR25(options.device)

