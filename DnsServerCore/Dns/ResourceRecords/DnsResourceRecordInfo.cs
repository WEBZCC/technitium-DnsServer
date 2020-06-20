﻿/*
Technitium DNS Server
Copyright (C) 2020  Shreyas Zare (shreyas@technitium.com)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.IO;
using TechnitiumLibrary.Net.Dns;

namespace DnsServerCore.Dns.ResourceRecords
{
    class DnsResourceRecordInfo
    {
        #region variables

        bool _disabled;
        IReadOnlyList<DnsResourceRecord> _glueRecords;

        #endregion

        #region constructor

        public DnsResourceRecordInfo()
        { }

        public DnsResourceRecordInfo(BinaryReader bR)
        {
            switch (bR.ReadByte()) //version
            {
                case 1:
                    _disabled = bR.ReadBoolean();
                    break;

                case 2:
                    _disabled = bR.ReadBoolean();

                    int count = bR.ReadByte();
                    if (count > 0)
                    {
                        DnsResourceRecord[] glueRecords = new DnsResourceRecord[count];

                        for (int i = 0; i < glueRecords.Length; i++)
                            glueRecords[i] = new DnsResourceRecord(bR.BaseStream);

                        _glueRecords = glueRecords;
                    }

                    break;

                default:
                    throw new InvalidDataException("DnsResourceRecordInfo format version not supported.");
            }
        }

        #endregion

        #region public

        public void WriteTo(BinaryWriter bW)
        {
            bW.Write((byte)2); //version
            bW.Write(_disabled);

            if (_glueRecords == null)
            {
                bW.Write((byte)0);
            }
            else
            {
                bW.Write(Convert.ToByte(_glueRecords.Count));

                foreach (DnsResourceRecord glueRecord in _glueRecords)
                    glueRecord.WriteTo(bW.BaseStream);
            }
        }

        #endregion

        #region properties

        public bool Disabled
        {
            get { return _disabled; }
            set { _disabled = value; }
        }

        public IReadOnlyList<DnsResourceRecord> GlueRecords
        {
            get { return _glueRecords; }
            set { _glueRecords = value; }
        }

        #endregion
    }
}
