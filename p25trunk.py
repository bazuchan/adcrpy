#!/usr/bin/python

import struct

class P25Trunk(object):
    def __init__(self):
        pass

    def decodeContolChannel(self, code, data):
        if code == 13:
            self.decodeTSBK(data)
        elif code == 14:
            self.decodePDU(data)
        elif code == 16:
            self.decodeEvent(data)

    @staticmethod
    def uint24r(short, byte):
        return byte | (short<<8)

    def decodeTSBK(self, data):
        b = data[0] & 63
        if data[1] == 144:
            if b == 2:
                #tt_grp_vc_grant(byte srv, ushort chanT, ushort chanR, ushort group, uint src)
                (srv, tx, group, src1, src2) =  struct.unpack('>BHHHB', data[2:10])
                self.tt_grp_vc_grant(srv, tx, 0, group, self.uint24r(src1, src2))
            elif b == 3:
                #tt_grp_vc_grant_updt(ushort chanT, ushort chanR, ushort group)
                (tx1, group1, tx2, group2) = struct.unpack('>HHHH', data[2:6])
                self.tt_grp_vc_grant_updt(tx1, 0, group1)
                self.tt_grp_vc_grant_updt(tx2, 0, group2)
            return
        elif data[1] or data[0] & 64:
            return

        if b == 52:
            #tt_iden_up(byte id, byte bw, int txo, uint cs, uint bf)
            tid = data[2] >> 4
            bw = data[2] & 15
            txo = (data[3] << 6) | ((data[4] >> 2) & 63)
            cs = (((data[4] & 3) << 8) | data[5]) * 125
            bf = (struct.unpack('>I', data[6:10])[0] * 5) & 0xFFFFFFFF
            sign = 1 - ((txo & 0x2000) >> 12)
            txo &= ~0x2000
            txo = sign * (txo * cs)
            self.tt_iden_up(tid, bw, txo, cs, bf)
        elif b == 61:
            tid = data[2] >> 4;
            bw = (((data[2] & 15) << 5) | (data[3] >> 3)) * 125
            txo = ((data[3] & 7) << 6) | ((data[4] >> 2) & 63)
            cs = (((data[4] & 3) << 8) | data[5]) * 125
            bf = (struct.unpack('>I', data[6:10])[0] * 5) & 0xFFFFFFFF
            sign = 1 - ((txo & 0x2000) >> 12)
            txo &= ~0x2000
            txo = sign * (txo * cs)
            self.tt_iden_up(tid, bw, txo, cs, bf)
        elif b == 0:
            (srv, tx, group, src1, src2) =  struct.unpack('>BHHHB', data[2:10])
            self.tt_grp_vc_grant(srv, tx, 0, group, self.uint24r(src1, src2))
        elif b == 2:
            (tx1, group1, tx2, group2) = struct.unpack('>HHHH', data[2:10])
            self.tt_grp_vc_grant_updt(tx1, 0, group1)
            self.tt_grp_vc_grant_updt(tx2, 0, group2)
        elif b == 3:
            (tx, rx, group) = struct.unpack('>HHH', data[4:10])
            self.tt_grp_vc_grant_updt(tx, rx, group)
        elif b == 4:
            #tt_uu_vc_grant(ushort chanT, ushort chanR, uint tgt_adr, uint src_adr)
            (tx, dst1, dst2, src1, src2) = struct.unpack('>HHBHB', data[2:10])
            self.tt_uu_vc_grant(tx, 0, self.uint24r(dst1, dst2), self.uint24r(src1, src2))
        elif b == 6:
            #tt_uu_vc_grant_updt(ushort chanT, ushort chanR, uint tgt_adr, uint src_adr)
            (tx, dst1, dst2, src1, src2) = struct.unpack('>HHBHB', data[2:10])
            self.tt_uu_vc_grant_updt(tx, 0, self.uint24r(dst1, dst2), self.uint24r(src1, src2))
        elif b == 20:
            #tt_grp_dc_grant(byte svc, ushort chanT, ushort chanR, uint tgt_adr)
            (svc, tx, rx, dst1, dst2) =  struct.unpack('>BHHHB', data[2:10])
            self.tt_grp_dc_grant(svc, tx, rx, self.uint24r(dst1, dst2))
        elif b == 21:
            #tt_page_req(byte svc, ushort dataAccCtrl, uint tgt_adr)
            (svc, tmp, acl, dst1, dst2) =  struct.unpack('>BHHHB', data[2:10])
            self.tt_page_req(svc, acl, self.uint24r(dst1, dst2))
        elif b == 40:
            #tt_grp_aff(byte lg, byte gav, ushort ann_grp_adr, ushort grp_adr, uint tgt_adr)
            (b2, ann_group, group, dst1, dst2) = struct.unpack('>BHHHB', data[2:10])
            self.tt_grp_aff(b2>>7, b2&3, ann_group, group, self.uint24r(dst1, dst2))
        elif b == 41:
            #tt_sccb_exp(byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
            (rfss, site, tx, t1, rx, ss_class) = struct.unpack('>BBHBHB', data[2:10])
            self.tt_sccb_exp(rfss, site, tx, rx, ss_class)
        elif b == 44:
            #tt_reg_rsp(byte res, ushort system, uint src_id, uint src_adr)
            (b2, b3, srcid1, srcid2, src1, src2) = struct.unpack('>BBHBHB', data[2:10])
            system = ((b2 & 15) << 8) | b3
            self.tt_reg_rsp((b2>>4)&3, system, self.uint24r(srcid1, srcid2), self.uint24r(src1, src2));
        elif b == 47:
            #tt_dereg_rsp(uint wacn, ushort system, uint src_id)
            wacn = (data[3] << 12) | (data[4] << 4) | ((data[5] >> 4) & 15)
            system = ((data[5] & 15) << 8) | data[6]
            (src1,src2) = struct.unpack('>HB', data[7:10])
            self.tt_dereg_rsp(wacn, system, self.uint24r(src1, src2))
        elif b == 43:
            #tt_loc_reg_rsp(byte res, ushort grp_adr, byte rfss, byte site, uint tgt_adr)
            (b2, group, rfss, site, dst1, dst2) = struct.unpack('>BHBBHB', data[2:10])
            self.tt_loc_reg_rsp(b2&3, group, rfss, site, self.uint24r(dst1, dst2))
        elif b == 57:
            #tt_sccb(byte rfss, byte site, ushort chan1, byte ss_class1, ushort chan2, byte ss_class2)
            (rfss, site, chan1, ss_class1, chan2, ss_class2) = struct.unpack('>BBHBHB', data[2:10])
            self.tt_sccb(rfss, site, chan1, ss_class1, chan2, ss_class2)
        elif b == 58:
            #tt_rfss_sts(byte lra, byte flags, ushort system, byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
            (lra, b3, b4, rfss, site, tx, ss_class) = struct.unpack('>BBBBBHB', data[2:10])
            system = ((b3 & 15) << 8) | b4
            self.tt_rfss_sts(lra, (b3>>4)&3, system, rfss, site, tx, 0, ss_class)
        elif b == 59:
            #tt_net_sts(byte lra, uint wacn, ushort system, ushort chanT, ushort chanR, byte ss_class)
            (lra, b3, b4, b5, tx, ss_class) = struct.unpack('>BBBBBHB', data[2:10])
            wacn = (data[3] << 12) | (data[4] << 4) | ((data[5] >> 4) & 15)
            system = ((b5 & 15) << 8) | b6
            self.tt_net_sts(lra, wacn, system, tx, 0, ss_class)
        elif b == 60:
            #tt_adj_sts(byte lra, byte flags, ushort system, byte rfss, byte site, ushort chanT, ushort chanR, byte ss_class)
            (lra, b3, b4, rfss, site, tx, ss_class) = struct.unpack('>BBBBBHB', data[2:10])
            system = ((b3 & 15) << 8) | b4
            self.tt_adj_sts(lra, b3>>4, system, rfss, site, tx, 0, ss_class)

    def decodePDU(self, data):
        b = data[7]
        if data[2] or data[1] not in [253,255]:
            return
	
        if b == 0:
            srv = data[8]
            (src1, src2) = struct.unpack('>HB', data[3:6])
            (tx, rx, group) = struct.unpack('>HHH', data[14:20])
            self.tt_grp_vc_grant(srv, tx, rx, group, self.uint24r(src1, src2))
        elif b == 4:
            (dst1, dst2, tx, rx) = struct.unpack('>HBHH', data[19:26])
            (src1, src2) = struct.unpack('>HB', data[3:6])
            self.tt_uu_vc_grant(tx, rx, self.uint24r(dst1, dst2), self.uint24r(src1, src2))
        elif b == 6:
            (dst1, dst2, tx, rx) = struct.unpack('>HBHH', data[19:26])
            (src1, src2) = struct.unpack('>HB', data[3:6])
            self.tt_uu_vc_grant_updt(tx, rx, self.uint24r(dst1, dst2), self.uint24r(src1, src2))
        elif b == 40:
            (dst1, dst2) = struct.unpack('>HB', data[3:6])
            (ann_group, group) = struct.unpack('>HH', data[16:20])
            self.tt_grp_aff(data[20]>>7, data[20]&3, ann_group, group, self.uint24r(dst1, dst2))
        elif b == 44:
            system = ((data[12] & 15) << 8) | data[13]
            (src1, src2) = struct.unpack('>HB', data[3:6])
            (srcid1, srcid2) = struct.unpack('>HB', data[14:17])
            self.tt_reg_rsp(data[17]&3, system, self.uint24r(srcid1, srcid2), self.uint24r(src1, src2))
        elif b == 58:
            (lra, b4, b5) = struct.unpack('>BBB', data[3:6])
            (rfss, site, tx, rx, ss_class) = struct.unpack('>BBHHB', data[12:19])
            system = ((b4 & 15) << 8) | b5
            self.tt_rfss_sts(lra, (b4>>4)&3, system, rfss, site, tx, rx, ss_class)
        elif b == 59:
            wacn = (data[12] << 12) | (data[13] << 4) | ((data[14] >> 4) & 15)
            system = ((data[4] & 15) << 8) | data[5]
            (tx, rx, ss_class) = struct.unpack('>HHB', data[15:20])
            self.tt_net_sts(data[3], wacn, system, tx, rx, ss_class)
        elif b == 60:
            (lra, b4, b5, t1, rfss, site, t2, tx, rx, ss_class) = struct.unpack('>BBBHBBHHHB', data[3:17])
            system = ((b4 & 15) << 8) | b4
            self.tt_adj_sts(lra, b4>>4, system, rfss, site, tx, rx, ss_class)

    def decodeEvent(data):
        ccfreq = struct.unpack('I', data[:4])[0]
        iscontrol = data[16] == 0
        dbm = struct.unpack('b', data[17:18])[0]
        if not iscontrol:
            (src, dst, vcfreq) = struct.unpack('III', data[8:20])
        print("tt event:", ccfreq, iscontrol, dbm, "\n")

    def tt_grp_vc_grant(self, *k):
        print("tt_grp_vc_grant:", k, "\n")
    def tt_grp_vc_grant_updt(self, *k):
        print("tt_grp_vc_grant_updt:", k, "\n")
    def tt_iden_up(self, *k):
        print("tt_iden_up:", k, "\n")
    def tt_uu_vc_grant(self, *k):
        print("tt_uu_vc_grant:", k, "\n")
    def tt_uu_vc_grant_updt(self, *k):
        print("tt_uu_vc_grant_updt:", k, "\n")
    def tt_grp_dc_grant(self, *k):
        print("tt_grp_dc_grant:", k, "\n")
    def tt_page_req(self, *k):
        print("tt_page_req:", k, "\n")
    def tt_grp_aff(self, *k):
        print("tt_grp_aff:", k, "\n")
    def tt_sccb_exp(self, *k):
        print("tt_sccb_exp:", k, "\n")
    def tt_reg_rsp(self, *k):
        print("tt_reg_rsp:", k, "\n")
    def tt_dereg_rsp(self, *k):
        print("tt_dereg_rsp:", k, "\n")
    def tt_loc_reg_rsp(self, *k):
        print("tt_loc_reg_rsp:", k, "\n")
    def tt_sccb(self, *k):
        print("tt_sccb:", k, "\n")
    def tt_rfss_sts(self, *k):
        print("tt_rfss_sts:", k, "\n")
    def tt_net_sts(self, *k):
        print("tt_net_sts:", k, "\n")
    def tt_adj_sts(self, *k):
        print("tt_adj_sts:", k, "\n")
