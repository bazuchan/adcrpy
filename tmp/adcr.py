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

def unstuff(x):
    x = ADCR25.unstuff(x)
    err = ADCR25.checkpacket(x)
    if err == -2:
        sys.stderr.write('Check len fail: %d vs %d\n' % (struct.unpack('>BBH', x[:4])[2], len(x)-6))
    elif err == -1:
        sys.stderr.write('Header sum fail\n')
    elif err == -3 :
        sys.stderr.write('Check sum fail\n')
    return x

def splitpkt(x):
    return x.strip(b'\xc0').split(b'\xc0\xc0')

if __name__ == '__main__':
    df = open(sys.argv[1], 'rb').read()
    pkts = dump2packets(df)
    for i in pkts:
        for j in splitpkt(i[1]):
            print('%s: %s' % (i[0].decode('utf-8'), ADCR25.decodepacket(unstuff(j))))

