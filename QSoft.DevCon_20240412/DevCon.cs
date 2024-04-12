using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static QSoft.DevCon.DeviceInfo;
using static QSoft.DevCon.DevMgr;
using static QSoft.DevCon.SetupApi;

namespace QSoft.DevCon
{
    [Obsolete("No supoort, please use new DevMgrExtension static method")]
    public class DevMgr
    {
        public static IEnumerable<(IntPtr, SP_DEVINFO_DATA)> Devices()
        {
            Guid guid = Guid.Empty;
            uint index = 0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);
            while(true)
            {
                SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                if (SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo) == false)
                {
                    var err = Marshal.GetLastWin32Error();
                    yield break;
                }
                else
                {
                    yield return (hDevInfo, devinfo);
                }
            }
        }
        List<string> Split(byte[] src)
        {
            List<string> dst = new List<string>();
            int begin_idx = 0;
            for (int i = 0; i < src.Length; i++)
            {
                if (src[i] == 0)
                {
                    if (begin_idx == i)
                    {
                        break;
                    }
                    var harwareidss = Encoding.UTF8.GetString(src, begin_idx, i - begin_idx);
                    dst.Add(harwareidss);
                    begin_idx = i + 1;
                }
            }
            return dst;
        }

        public class AAA
        {
            public Guid DeviceInfo { set; get; }
            public string DevicePath { set; get; }
            public string FriendlyName { set; get; }
            public string Description { set; get; }
        }
        public List<AAA> AllPath()
        {
            Guid GUID_DEVICE_BATTERY = new Guid("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
            List<AAA> paths = new List<AAA>();
            Guid DiskGUID = GUID_DEVICE_BATTERY;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES | DIGCF_DEVICEINTERFACE);
            SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            DeviceInterfaceData.cbSize = Marshal.SizeOf(DeviceInterfaceData.GetType());

            for (int i = 0; SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref DiskGUID, i, ref DeviceInterfaceData); i++)
            {
                int dwSize = 0;
                int requiredSize = 0;
                var nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, IntPtr.Zero, 0, ref dwSize, IntPtr.Zero);
                SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                var buffer = Marshal.AllocHGlobal(dwSize);
                Marshal.WriteInt32(buffer, 8);//4 + Marshal.SystemDefaultCharSize*2;
                nStatus = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref DeviceInterfaceData, buffer, dwSize, ref dwSize, ref devinfo);

                var oiu = Marshal.PtrToStringAuto(IntPtr.Add(buffer, 4));
                Marshal.FreeHGlobal(buffer);
                var err = Marshal.GetLastWin32Error();
                AAA aaa = new AAA();
                aaa.DeviceInfo = devinfo.ClassGuid;
                aaa.DevicePath = oiu;
                StringBuilder friendlyname = new StringBuilder(2048);
                SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_FRIENDLYNAME, IntPtr.Zero, friendlyname, friendlyname.Capacity, IntPtr.Zero);
                aaa.FriendlyName = friendlyname.ToString();
                StringBuilder devicedesc = new StringBuilder(2048);
                SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_DEVICEDESC, IntPtr.Zero, devicedesc, devicedesc.Capacity, IntPtr.Zero);
                aaa.Description = devicedesc.ToString();
                paths.Add(aaa);
                err = 0;
            }

            return paths;
        }
        static Lazy<IntPtr> m_hDev = new Lazy<IntPtr>(() =>
        {
            Guid DiskGUID = Guid.Empty;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);
            return hDevInfo;
        });
        public IEnumerable<DeviceInfo> AllDevice()
        {
            Guid DiskGUID = Guid.Empty;
            //IntPtr hDevInfo = SetupDiGetClassDevs(ref DiskGUID, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);
            var hDevInfo = m_hDev.Value;
            int err = 0;
            uint index = 0;
            while (true)
            {
                SP_DEVINFO_DATA devinfo = new SP_DEVINFO_DATA();
                devinfo.cbSize = (uint)Marshal.SizeOf(devinfo);
                if (SetupDiEnumDeviceInfo(hDevInfo, index, ref devinfo) == false)
                {
                    err = Marshal.GetLastWin32Error();
                    yield break;
                }
                else
                {
                    DeviceInfo dev = new DeviceInfo(m_hDev.Value, devinfo);

                    //StringBuilder hardwareid = new StringBuilder(2048);
                    //SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_HARDWAREID, IntPtr.Zero, hardwareid, hardwareid.Capacity, IntPtr.Zero);
                    //dev.HardwareID = hardwareid.ToString();
                    byte[] buf = new byte[2048];
                    
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_HARDWAREID, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
                    dev.HardwareIDs.AddRange(Split(buf));

                    StringBuilder friendlyname = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_FRIENDLYNAME, IntPtr.Zero, friendlyname, friendlyname.Capacity, IntPtr.Zero);
                    dev.FriendlyName = friendlyname.ToString();
                    StringBuilder devicedesc = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_DEVICEDESC, IntPtr.Zero, devicedesc, devicedesc.Capacity, IntPtr.Zero);
                    dev.Description = devicedesc.ToString();

                    StringBuilder deviceclass = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_CLASS, IntPtr.Zero, deviceclass, deviceclass.Capacity, IntPtr.Zero);
                    dev.Class = deviceclass.ToString();
                    if(string.IsNullOrEmpty(dev.Class) == true)
                    {
                        System.Diagnostics.Trace.WriteLine("");
                    }
                    StringBuilder deviceclassguid = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_CLASSGUID, IntPtr.Zero, deviceclassguid, deviceclassguid.Capacity, IntPtr.Zero);
                    //System.Diagnostics.Trace.WriteLine($"class name:{deviceclass.ToString()} {deviceclassguid.ToString()}");
                    if(string.IsNullOrEmpty(deviceclassguid.ToString()) == true)
                    {
                        dev.ClassGuid = Guid.Empty;
                    }
                    else
                    {
                        var guid = Guid.Parse(deviceclassguid.ToString());
                        dev.ClassGuid = guid;
                        StringBuilder classdesc = new StringBuilder(2048);
                        SetupDiGetClassDescription(ref guid, classdesc, classdesc.Capacity, IntPtr.Zero);
                        dev.ClassDescription = classdesc.ToString();
                    }

                    StringBuilder location = new StringBuilder(2048);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_LOCATION_INFORMATION, IntPtr.Zero, location, location.Capacity, IntPtr.Zero);
                    dev.Location = location.ToString();

                    Array.Clear(buf, 0, buf.Length);
                    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, SPDRP_LOCATION_PATHS, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
                    dev.LocationPaths.AddRange(this.Split(buf));



                    StringBuilder instanceid = new StringBuilder(2048);
                    SetupDiGetDeviceInstanceId(hDevInfo, ref devinfo, instanceid, instanceid.Capacity, IntPtr.Zero);
                    dev.InstanceId = instanceid.ToString();                

                    yield return dev;
                }
                index++;
            }

            //return dds;
        }

        const ulong CR_SUCCESS = 0x0;

        //string GetValueString(IntPtr hDevInfo, ref SP_DEVINFO_DATA devinfo, uint spdrp)
        //{
        //    int property_type = 0;
        //    int requiresize = 0;
        //    var hr = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, out property_type, IntPtr.Zero, 0, out requiresize);
        //    var err = Marshal.GetLastWin32Error();
        //    if(requiresize == 0)
        //    {
        //        return "";
        //    }
        //    byte[] buf = new byte[requiresize];
        //    hr = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, buf, buf.Length, IntPtr.Zero);
        //    err = Marshal.GetLastWin32Error();
        //    var sss = Encoding.UTF8.GetString(buf, 0, buf.Length);

        //    StringBuilder strb = new StringBuilder(requiresize);
            
        //    SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
        //    return sss;

        //    //StringBuilder strb = new StringBuilder(requiresize);
        //    //SetupDiGetDeviceRegistryProperty(hDevInfo, ref devinfo, spdrp, IntPtr.Zero, strb, strb.Capacity, IntPtr.Zero);
        //    //return strb.ToString();
        //}

        public int Enable(Func<DeviceInfo, bool> func)
        {
            int count = 0;
            foreach (var dev in this.AllDevice())
            {
                if (func(dev) == true)
                {
                    ChangeState(dev.m_DevInfo, true);
                    //dev.ChangeState(true, m_hDev.Value);
                }
            }

            return count;
        }

        internal void ChangeState(SP_DEVINFO_DATA devinfo, bool isenable)
        {
            uint status;
            uint problem;
            var hr = CM_Get_DevNode_Status(out status, out problem, devinfo.DevInst, 0);
            var enable = status & DN_STARTED;
            if(isenable == true && enable != DN_STARTED)
            {
                return;
            }
            var disable = status & DN_DISABLEABLE;
            if (isenable == false && disable != DN_DISABLEABLE)
            {
                return;
            }

            SP_PROPCHANGE_PARAMS params1 = new SP_PROPCHANGE_PARAMS();
            params1.ClassInstallHeader.cbSize = Marshal.SizeOf(params1.ClassInstallHeader.GetType());
            params1.ClassInstallHeader.InstallFunction = DIF_PROPERTYCHANGE;
            params1.Scope = DICS_FLAG_GLOBAL;
            params1.StateChange = isenable==true?DICS_ENABLE: DICS_DISABLE;

            // setup proper parameters            
            //if (!SetupDiSetClassInstallParams(hDevInfo, ptrToDevInfoData, ClassInstallParams, Marshal.SizeOf(params1.GetType())))
            if (!SetupDiSetClassInstallParams(m_hDev.Value, devinfo, params1, Marshal.SizeOf(params1.GetType())))
            {
                int errorcode = Marshal.GetLastWin32Error();
                errorcode = 0;
            }

            // use parameters
            if (!SetupDiCallClassInstaller((uint)DIF_PROPERTYCHANGE, m_hDev.Value, ref devinfo))
            {
                int errorcode = Marshal.GetLastWin32Error(); // error here  
                var msg = GetLastErrorMessage(errorcode);
                throw new Exception(msg);
            }
        }

        string GetLastErrorMessage(int error)
        {
            IntPtr lpBuff = IntPtr.Zero;
            string sMsg = "";
            if (0 != FormatMessage(FORMAT_MESSAGE.ALLOCATE_BUFFER
                   | FORMAT_MESSAGE.FROM_SYSTEM
                   | FORMAT_MESSAGE.IGNORE_INSERTS,
                   IntPtr.Zero,
                   error,
                   0,
                   ref lpBuff,
                   0,
                   IntPtr.Zero))
            {
                sMsg = Marshal.PtrToStringUni(lpBuff);            //結果爲“重疊 I/O 操作在進行中”，完全正確
                Marshal.FreeHGlobal(lpBuff);
            }
            return sMsg;
        }

        public int Disable(Func<DeviceInfo, bool> func)
        {
            int count = 0;
            foreach (var dev in this.AllDevice())
            {
                if (func(dev) == true)
                {
                    ChangeState(dev.m_DevInfo, false);
                }
            }

            return count;
        }

        //static Guid BUS1394_CLASS_GUID = new Guid("6BDD1FC1-810F-11d0-BEC7-08002BE2092F");
        //static Guid GUID_61883_CLASS = new Guid("7EBEFBC0-3200-11d2-B4C2-00A0C9697D07");
        //public static Guid GUID_DEVICE_APPLICATIONLAUNCH_BUTTON = new Guid("629758EE-986E-4D9E-8E47-DE27F8AB054D");
        //public static Guid GUID_DEVICE_BATTERY = new Guid("72631E54-78A4-11D0-BCF7-00AA00B7B32A");
        //public static Guid GUID_DEVICE_LID = new Guid("4AFA3D52-74A7-11d0-be5e-00A0C9062857");
        //public static Guid GUID_DEVICE_MEMORY = new Guid("3FD0F03D-92E0-45FB-B75C-5ED8FFB01021");
        //public static Guid GUID_DEVICE_MESSAGE_INDICATOR = new Guid("CD48A365-FA94-4CE2-A232-A1B764E5D8B4");
        //public static Guid GUID_DEVICE_PROCESSOR = new Guid("97FADB10-4E33-40AE-359C-8BEF029DBDD0");
        //public static Guid GUID_DEVICE_SYS_BUTTON = new Guid("4AFA3D53-74A7-11d0-be5e-00A0C9062857");
        //public static Guid GUID_DEVICE_THERMAL_ZONE = new Guid("4AFA3D51-74A7-11d0-be5e-00A0C9062857");
        //public static Guid GUID_BTHPORT_DEVICE_INTERFACE = new Guid("0850302A-B344-4fda-9BE9-90576B8D46F0");
        //public static Guid GUID_DEVINTERFACE_BRIGHTNESS = new Guid("FDE5BBA4-B3F9-46FB-BDAA-0728CE3100B4");
        //public static Guid GUID_DEVINTERFACE_DISPLAY_ADAPTER = new Guid("5B45201D-F2F2-4F3B-85BB-30FF1F953599");
        //public static Guid GUID_DEVINTERFACE_I2C = new Guid("2564AA4F-DDDB-4495-B497-6AD4A84163D7");
        //public static Guid GUID_DEVINTERFACE_IMAGE = new Guid("6BDD1FC6-810F-11D0-BEC7-08002BE2092F");
        //public static Guid GUID_DEVINTERFACE_MONITOR = new Guid("E6F07B5F-EE97-4a90-B076-33F57BF4EAA7");
        //public static Guid GUID_DEVINTERFACE_OPM = new Guid("BF4672DE-6B4E-4BE4-A325-68A91EA49C09");
        //public static Guid GUID_DEVINTERFACE_VIDEO_OUTPUT_ARRIVAL = new Guid("1AD9E4F0-F88D-4360-BAB9-4C2D55E564CD");
        //public static Guid GUID_DISPLAY_DEVICE_ARRIVAL = new Guid("1CA05180-A699-450A-9A0C-DE4FBE3DDD89");
        //public static Guid GUID_DEVINTERFACE_HID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
        //public static Guid GUID_DEVINTERFACE_KEYBOARD = new Guid("884b96c3-56ef-11d1-bc8c-00a0c91405dd");
        //public static Guid GUID_DEVINTERFACE_MOUSE = new Guid("378DE44C-56EF-11D1-BC8C-00A0C91405DD");
        //public static Guid GUID_DEVINTERFACE_MODEM = new Guid("2C7089AA-2E0E-11D1-B114-00C04FC2AAE4");
        //public static Guid GUID_DEVINTERFACE_NET = new Guid("CAC88484-7515-4C03-82E6-71A87ABAC361");
        //public static Guid GUID_DEVINTERFACE_SENSOR = new Guid(0XBA1BB692, 0X9B7A, 0X4833, 0X9A, 0X1E, 0X52, 0X5E, 0XD1, 0X34, 0XE7, 0XE2);
        //public static Guid GUID_DEVINTERFACE_COMPORT = new Guid("86E0D1E0-8089-11D0-9CE4-08003E301F73");
        //public static Guid GUID_DEVINTERFACE_PARALLEL = new Guid("97F76EF0-F883-11D0-AF1F-0000F800845C");
        //public static Guid GUID_DEVINTERFACE_PARCLASS = new Guid("811FC6A5-F728-11D0-A537-0000F8753ED1");
        //public static Guid GUID_DEVINTERFACE_SERENUM_BUS_ENUMERATOR = new Guid("4D36E978-E325-11CE-BFC1-08002BE10318");
        //public static Guid GUID_DEVINTERFACE_CDCHANGER = new Guid("53F56312-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_CDROM = new Guid("53F56308-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_DISK = new Guid("53F56307-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_FLOPPY = new Guid("53F56311-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_MEDIUMCHANGER = new Guid("53F56310-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_PARTITION = new Guid("53F5630A-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_STORAGEPORT = new Guid("2ACCFE60-C130-11D2-B082-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_TAPE = new Guid("53F5630B-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_VOLUME = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_DEVINTERFACE_WRITEONCEDISK = new Guid("53F5630C-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_IO_VOLUME_DEVICE_INTERFACE = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid MOUNTDEV_MOUNTED_DEVICE_GUID = new Guid("53F5630D-B6BF-11D0-94F2-00A0C91EFB8B");
        //public static Guid GUID_AVC_CLASS = new Guid("095780C3-48A1-4570-BD95-46707F78C2DC");
        //public static Guid GUID_VIRTUAL_AVC_CLASS = new Guid("616EF4D0-23CE-446D-A568-C31EB01913D0");
        //public static Guid KSCATEGORY_ACOUSTIC_ECHO_CANCEL = new Guid("BF963D80-C559-11D0-8A2B-00A0C9255AC1");
        //public static Guid KSCATEGORY_AUDIO = new Guid("6994AD04-93EF-11D0-A3CC-00A0C9223196");
        //public static Guid KSCATEGORY_AUDIO_DEVICE = new Guid("FBF6F530-07B9-11D2-A71E-0000F8004788");
        //public static Guid KSCATEGORY_AUDIO_GFX = new Guid("9BAF9572-340C-11D3-ABDC-00A0C90AB16F");
        //public static Guid KSCATEGORY_AUDIO_SPLITTER = new Guid("9EA331FA-B91B-45F8-9285-BD2BC77AFCDE");
        //public static Guid KSCATEGORY_BDA_IP_SINK = new Guid("71985F4A-1CA1-11d3-9CC8-00C04F7971E0");
        //public static Guid KSCATEGORY_BDA_NETWORK_EPG = new Guid("71985F49-1CA1-11d3-9CC8-00C04F7971E0");
        //public static Guid KSCATEGORY_BDA_NETWORK_PROVIDER = new Guid("71985F4B-1CA1-11d3-9CC8-00C04F7971E0");
        //public static Guid KSCATEGORY_BDA_NETWORK_TUNER = new Guid("71985F48-1CA1-11d3-9CC8-00C04F7971E0");
        //public static Guid KSCATEGORY_BDA_RECEIVER_COMPONENT = new Guid("FD0A5AF4-B41D-11d2-9C95-00C04F7971E0");
        //public static Guid KSCATEGORY_BDA_TRANSPORT_INFORMATION = new Guid("A2E3074F-6C3D-11d3-B653-00C04F79498E");
        //public static Guid KSCATEGORY_BRIDGE = new Guid("085AFF00-62CE-11CF-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_CAPTURE = new Guid("65E8773D-8F56-11D0-A3B9-00A0C9223196");
        //public static Guid KSCATEGORY_CLOCK = new Guid("53172480-4791-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_COMMUNICATIONSTRANSFORM = new Guid("CF1DDA2C-9743-11D0-A3EE-00A0C9223196");
        //public static Guid KSCATEGORY_CROSSBAR = new Guid("A799A801-A46D-11D0-A18C-00A02401DCD4");
        //public static Guid KSCATEGORY_DATACOMPRESSOR = new Guid("1E84C900-7E70-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_DATADECOMPRESSOR = new Guid("2721AE20-7E70-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_DATATRANSFORM = new Guid("2EB07EA0-7E70-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_DRM_DESCRAMBLE = new Guid("FFBB6E3F-CCFE-4D84-90D9-421418B03A8E");
        //public static Guid KSCATEGORY_ENCODER = new Guid("19689BF6-C384-48fd-AD51-90E58C79F70B");
        //public static Guid KSCATEGORY_ESCALANTE_PLATFORM_DRIVER = new Guid("74F3AEA8-9768-11D1-8E07-00A0C95EC22E");
        //public static Guid KSCATEGORY_FILESYSTEM = new Guid("760FED5E-9357-11D0-A3CC-00A0C9223196");
        //public static Guid KSCATEGORY_INTERFACETRANSFORM = new Guid("CF1DDA2D-9743-11D0-A3EE-00A0C9223196");
        //public static Guid KSCATEGORY_MEDIUMTRANSFORM = new Guid("CF1DDA2E-9743-11D0-A3EE-00A0C9223196");
        //public static Guid KSCATEGORY_MICROPHONE_ARRAY_PROCESSOR = new Guid("830A44F2-A32D-476B-BE97-42845673B35A");
        //public static Guid KSCATEGORY_MIXER = new Guid("AD809C00-7B88-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_MULTIPLEXER = new Guid("7A5DE1D3-01A1-452c-B481-4FA2B96271E8");
        //public static Guid KSCATEGORY_NETWORK = new Guid("67C9CC3C-69C4-11D2-8759-00A0C9223196");
        //public static Guid KSCATEGORY_PREFERRED_MIDIOUT_DEVICE = new Guid("D6C50674-72C1-11D2-9755-0000F8004788");
        //public static Guid KSCATEGORY_PREFERRED_WAVEIN_DEVICE = new Guid("D6C50671-72C1-11D2-9755-0000F8004788");
        //public static Guid KSCATEGORY_PREFERRED_WAVEOUT_DEVICE = new Guid("D6C5066E-72C1-11D2-9755-0000F8004788");
        //public static Guid KSCATEGORY_PROXY = new Guid("97EBAACA-95BD-11D0-A3EA-00A0C9223196");
        //public static Guid KSCATEGORY_QUALITY = new Guid("97EBAACB-95BD-11D0-A3EA-00A0C9223196");
        //public static Guid KSCATEGORY_REALTIME = new Guid("EB115FFC-10C8-4964-831D-6DCB02E6F23F");
        //public static Guid KSCATEGORY_RENDER = new Guid("65E8773E-8F56-11D0-A3B9-00A0C9223196");
        //public static Guid KSCATEGORY_SPLITTER = new Guid("0A4252A0-7E70-11D0-A5D6-28DB04C10000");
        //public static Guid KSCATEGORY_SYNTHESIZER = new Guid("DFF220F3-F70F-11D0-B917-00A0C9223196");
        //public static Guid KSCATEGORY_SYSAUDIO = new Guid("A7C7A5B1-5AF3-11D1-9CED-00A024BF0407");
        //public static Guid KSCATEGORY_TEXT = new Guid("6994AD06-93EF-11D0-A3CC-00A0C9223196");
        //public static Guid KSCATEGORY_TOPOLOGY = new Guid("DDA54A40-1E4C-11D1-A050-405705C10000");
        //public static Guid KSCATEGORY_TVAUDIO = new Guid("A799A802-A46D-11D0-A18C-00A02401DCD4");
        //public static Guid KSCATEGORY_TVTUNER = new Guid("A799A800-A46D-11D0-A18C-00A02401DCD4");
        //public static Guid KSCATEGORY_VBICODEC = new Guid("07DAD660-22F1-11D1-A9F4-00C04FBBDE8F");
        //public static Guid KSCATEGORY_VIDEO = new Guid("6994AD05-93EF-11D0-A3CC-00A0C9223196");
        //public static Guid KSCATEGORY_VIRTUAL = new Guid("3503EAC4-1F26-11D1-8AB0-00A0C9223196");
        //public static Guid KSCATEGORY_VPMUX = new Guid("A799A803-A46D-11D0-A18C-00A02401DCD4");
        //public static Guid KSCATEGORY_WDMAUD = new Guid("3E227E76-690D-11D2-8161-0000F8775BF1");
        //public static Guid KSMFT_CATEGORY_AUDIO_DECODER = new Guid("9ea73fb4-ef7a-4559-8d5d-719d8f0426c7");
        //public static Guid KSMFT_CATEGORY_AUDIO_EFFECT = new Guid("11064c48-3648-4ed0-932e-05ce8ac811b7");
        //public static Guid KSMFT_CATEGORY_AUDIO_ENCODER = new Guid("91c64bd0-f91e-4d8c-9276-db248279d975");
        //public static Guid KSMFT_CATEGORY_DEMULTIPLEXER = new Guid("a8700a7a-939b-44c5-99d7-76226b23b3f1");
        //public static Guid KSMFT_CATEGORY_MULTIPLEXER = new Guid("059c561e-05ae-4b61-b69d-55b61ee54a7b");
        //public static Guid KSMFT_CATEGORY_OTHER = new Guid("90175d57-b7ea-4901-aeb3-933a8747756f");
        //public static Guid KSMFT_CATEGORY_VIDEO_DECODER = new Guid("d6c02d4b-6833-45b4-971a-05a4b04bab91");
        //public static Guid KSMFT_CATEGORY_VIDEO_EFFECT = new Guid("12e17c21-532c-4a6e-8a1c-40825a736397");
        //public static Guid KSMFT_CATEGORY_VIDEO_ENCODER = new Guid("f79eac7d-e545-4387-bdee-d647d7bde42a");
        //public static Guid KSMFT_CATEGORY_VIDEO_PROCESSOR = new Guid("302ea3fc-aa5f-47f9-9f7a-c2188bb16302");
        //public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        //public static Guid GUID_DEVINTERFACE_USB_HOST_CONTROLLER = new Guid("3ABF6F2D-71C4-462A-8A92-1E6861E6AF27");
        //public static Guid GUID_DEVINTERFACE_USB_HUB = new Guid("F18A0E88-C30C-11D0-8815-00A0C906BED8");
        //public static Guid GUID_DEVINTERFACE_WPD = new Guid("6AC27878-A6FA-4155-BA85-F98F491D4F33");
        //public static Guid GUID_DEVINTERFACE_WPD_PRIVATE = new Guid("BA0C718F-4DED-49B7-BDD3-FABE28661211");
        //public static Guid GUID_DEVINTERFACE_SIDESHOW = new Guid("152E5811-FEB9-4B00-90F4-D32947AE1681");[DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, ref SP_DEVINFO_DATA devInfo, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport(@"setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, IntPtr deviceInfoData);
        enum FORMAT_MESSAGE : uint
        {
            ALLOCATE_BUFFER = 0x00000100,
            IGNORE_INSERTS = 0x00000200,
            FROM_SYSTEM = 0x00001000,
            ARGUMENT_ARRAY = 0x00002000,
            FROM_HMODULE = 0x00000800,
            FROM_STRING = 0x00000400
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern int FormatMessage(FORMAT_MESSAGE dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageZId, ref IntPtr lpBuffer, int nSize, IntPtr Arguments);
        [StructLayout(LayoutKind.Sequential)]
        struct SP_DEVICE_INTERFACE_DATA
        {
            public Int32 cbSize;
            public Guid interfaceClassGuid;
            public Int32 flags;
            private UIntPtr reserved;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, IntPtr Enumerator, IntPtr hwndParent, uint Flags);
        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiEnumDeviceInfo(IntPtr DeviceInfoSet, uint MemberIndex, ref SP_DEVINFO_DATA DeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetClassDescription(ref Guid ClassGuid, StringBuilder ClassDescription, int ClassDescriptionSize, IntPtr RequiredSize);
        
        [StructLayout(LayoutKind.Sequential)]
        internal class SP_PROPCHANGE_PARAMS
        {
            public SP_CLASSINSTALL_HEADER ClassInstallHeader = new SP_CLASSINSTALL_HEADER();
            public int StateChange;
            public int Scope;
            public int HwProfile;
        };
        [StructLayout(LayoutKind.Sequential)]
        internal class SP_CLASSINSTALL_HEADER
        {
            public int cbSize;
            public int InstallFunction;
        };
        const int DIGCF_DEFAULT = 0x1;
        const int DIGCF_PRESENT = 0x2;
        const int DIGCF_ALLCLASSES = 0x4;
        const int DIGCF_PROFILE = 0x8;
        const int DIGCF_DEVICEINTERFACE = 0x10;
        const int DICS_ENABLE = 0x00000001;
        const int DICS_DISABLE = 0x00000002;
        const int DICS_PROPCHANGE = 0x00000003;
        const int DICS_START = 0x00000004;
        const int DICS_STOP = 0x00000005;
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out UInt32 propertyRegDataType, byte[] propertyBuffer, uint propertyBufferSize, out UInt32 requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, byte[] propertyBuffer, int propertyBufferSize, IntPtr requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, out int propertyRegDataType, IntPtr propertyBuffer, int propertyBufferSize, out int requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, StringBuilder propertyBuffer, int propertyBufferSize, IntPtr requiredSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiGetDeviceRegistryProperty(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, uint property, IntPtr propertyRegDataType, string[] propertyBuffer, int propertyBufferSize, IntPtr requiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetupDiGetDeviceInstanceId(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, StringBuilder DeviceInstanceId, int DeviceInstanceIdSize, IntPtr RequiredSize);

        [DllImport("cfgmgr32.dll", SetLastError = true)]
        static extern int CM_Get_DevNode_Status(out UInt32 status, out UInt32 probNum, UInt32 devInst, int flags);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetupDiSetClassInstallParams(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SP_PROPCHANGE_PARAMS ClassInstallParams, int ClassInstallParamsSize);
        [DllImport("setupapi.dll", SetLastError = true)]
        static extern bool SetupDiCallClassInstaller(UInt32 InstallFunction, IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData);
        static int DIF_PROPERTYCHANGE = (0x00000012);
        static int DICS_FLAG_GLOBAL = (0x00000001);
        uint SPDRP_DEVICEDESC = 0x00000000; // DeviceDesc (R/W)
        uint SPDRP_HARDWAREID = (0x00000001);  // HardwareID (R/W)
        uint SPDRP_COMPATIBLEIDS = (0x00000002);  // CompatibleIDs (R/W)
        uint SPDRP_UNUSED0 = (0x00000003);  // unused
        uint SPDRP_SERVICE = (0x00000004); // Service (R/W)
        uint SPDRP_UNUSED1 = (0x00000005); // unused
        uint SPDRP_UNUSED2 = (0x00000006); // unused
        uint SPDRP_CLASS = (0x00000007); // Class (R--tied to ClassGUID)
        uint SPDRP_CLASSGUID = (0x00000008); // ClassGUID (R/W)
        uint SPDRP_DRIVER = (0x00000009); // Driver (R/W)
        uint SPDRP_CONFIGFLAGS = (0x0000000A); // ConfigFlags (R/W)
        uint SPDRP_MFG = (0x0000000B);// Mfg (R/W)
        uint SPDRP_FRIENDLYNAME = (0x0000000C);// FriendlyName (R/W)
        uint SPDRP_LOCATION_INFORMATION = (0x0000000D); // LocationInformation (R/W)
        uint SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = (0x0000000E); // PhysicalDeviceObjectName (R)
        uint SPDRP_CAPABILITIES = (0x0000000F);  // Capabilities (R)
        uint SPDRP_UI_NUMBER = (0x00000010); // UiNumber (R)
        uint SPDRP_UPPERFILTERS = (0x00000011); // UpperFilters (R/W)
        uint SPDRP_LOWERFILTERS = (0x00000012); // LowerFilters (R/W)
        int SPDRP_BUSTYPEGUID = (0x00000013); // BusTypeGUID (R)
        int SPDRP_LEGACYBUSTYPE = (0x00000014);  // LegacyBusType (R)
        int SPDRP_BUSNUMBER = (0x00000015); // BusNumber (R)
        int SPDRP_ENUMERATOR_NAME = (0x00000016); // Enumerator Name (R)
        int SPDRP_SECURITY = (0x00000017); // Security (R/W, binary form)
        int SPDRP_SECURITY_SDS = (0x00000018); // Security (W, SDS form)
        int SPDRP_DEVTYPE = (0x00000019); // Device Type (R/W)
        int SPDRP_EXCLUSIVE = (0x0000001A); // Device is exclusive-access (R/W)
        int SPDRP_CHARACTERISTICS = (0x0000001B); // Device Characteristics (R/W)
        int SPDRP_ADDRESS = (0x0000001C);// Device Address (R)
        int SPDRP_UI_NUMBER_DESC_FORMAT = (0X0000001D); // UiNumberDescFormat (R/W)
        int SPDRP_DEVICE_POWER_DATA = (0x0000001E); // Device Power Data (R)
        int SPDRP_REMOVAL_POLICY = (0x0000001F); // Removal Policy (R)
        int SPDRP_REMOVAL_POLICY_HW_DEFAULT = (0x00000020);// Hardware Removal Policy (R)
        uint SPDRP_REMOVAL_POLICY_OVERRIDE = (0x00000021); // Removal Policy Override (RW)
        uint SPDRP_INSTALL_STATE = (0x00000022);// Device Install State (R)
        uint SPDRP_LOCATION_PATHS = (0x00000023); // Device Location Paths (R)
        int SPDRP_BASE_CONTAINERID = (0x00000024); // Base ContainerID (R)

        int SPDRP_MAXIMUM_PROPERTY = (0x00000025);// Upper bound on ordinals


        uint DN_ROOT_ENUMERATED = 0x00000001; // Was enumerated by ROOT
        uint DN_DRIVER_LOADED=0x00000002; // Has Register_Device_Driver
        uint DN_ENUM_LOADED = 0x00000004; // Has Register_Enumerator
        uint DN_STARTED = 0x00000008; // Is currently configured
        uint DN_MANUAL = 0x00000010; // Manually installed
        uint DN_NEED_TO_ENUM = 0x00000020; // May need reenumeration
        uint DN_NOT_FIRST_TIME = 0x00000040; // Has received a config
        uint DN_HARDWARE_ENUM = 0x00000080; // Enum generates hardware ID
        uint DN_LIAR = 0x00000100; // Lied about can reconfig once
        uint DN_HAS_MARK = 0x00000200; // Not CM_Create_DevInst lately
        uint DN_HAS_PROBLEM = 0x00000400; // Need device installer
        uint DN_FILTERED = 0x00000800; // Is filtered
        uint DN_MOVED = 0x00001000; // Has been moved
        uint DN_DISABLEABLE = 0x00002000; // Can be disabled
        uint DN_REMOVABLE = 0x00004000; // Can be removed
        uint DN_PRIVATE_PROBLEM = 0x00008000; // Has a private problem
        uint DN_MF_PARENT = 0x00010000; // Multi function parent
        uint DN_MF_CHILD = 0x00020000; // Multi function child
        uint DN_WILL_BE_REMOVED = 0x00040000; // DevInst is being removed
    }


    



}
