#!/usr/bin/python3

import sys
import re
import struct
from adcrctl import ADCR_CRC, ADCR25

def dump2packets(data):
    r = []
    while True:
        start = data.find(b'<201')
        if start == -1:
            break
        m = re.search(b'^<201\d{11}\.\d{3} ([RT])X>', data[start:])
        if not m:
            break
        x = m.group(1)
        end = data[start+1:].find(b'<201')
        if end == -1:
            r.append((x, data[start+24:]))
            return r
        end += start+1
        m = re.search(b'^<201\d{11}\.\d{3} ([RT])X>', data[end:])
        if not m:
            sys.stderr.write('rare case, data will be broken\n')
            return []
        r.append((x, data[start+24:end]))
        data = data[end:]
    return r

def checklen(x):
    return struct.unpack('>BBH', x[:4])[2] == len(x)-6

def checkcrc(x):
    return ADCR_CRC().update(x[:-2]).digest() == struct.unpack('>H', x[-2:])[0]

def checkheadersum(x):
    return (~(x[0]+x[2]+x[3]))&0xFF == x[1]

def unstuff(x):
    x = x.replace(b'\xdb\xdc', b'\xc0').replace(b'\xdb\xdd', b'\xdb')
    if not checklen(x):
        sys.stderr.write('Check len fail: %d vs %d\n' % (struct.unpack('>BBH', x[:4])[2], len(x)-6))
    if not checkheadersum(x):
        sys.stderr.write('Header sum fail\n')
    if not checkcrc(x):
        sys.stderr.write('Check sum fail\n')
    return x

def splitpkt(x):
    return x.strip(b'\xc0').split(b'\xc0\xc0')

def decodepacket(x):
    return 'Code %u, Data %s' % (x[0], repr(x[4:-2]))

if __name__ == '__main__':
    df = open(sys.argv[1], 'rb').read()
    pkts = dump2packets(df)
    for i in pkts:
        for j in splitpkt(i[1]):
            print('%s: %s' % (i[0].decode('utf-8'), decodepacket(unstuff(j))))

