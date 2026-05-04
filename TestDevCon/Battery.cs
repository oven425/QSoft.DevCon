using Microsoft.Management.Infrastructure;
using QSoft.DevCon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestDevCon
{
    //test case
    //https://garytown.com/gathering-battery-information-via-powershell-wmi
    public class BatteryTest
    {
        // ── 對應 WMI: Get-CimInstance -Namespace ROOT\WMI -ClassName "BatteryStaticData" ──
        // DesignedCapacity, SerialNumber, ManufactureName
        [Fact]
        public void Test_BatteryStaticData()
        {
            var ioctl = BatteryReport.GetAll()
                .Select(x => new
                {
                    DesignedCapacity = x.DesignedCapacity,
                    SerialNumber = x.SerialNumber,
                    ManufactureName = x.ManufactureName,
                }).ToArray();
            using CimSession session = CimSession.Create(null);
            var wmi = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryStaticData")
                .Select(x => new
                {
                    DesignedCapacity = (uint)x.CimInstanceProperties["DesignedCapacity"].Value,
                    SerialNumber = x.CimInstanceProperties["SerialNumber"].Value as string,
                    ManufactureName = x.CimInstanceProperties["ManufactureName"].Value as string,
                }).ToArray();

            

            Assert.Equal(wmi.Length, ioctl.Length);
            for (int i = 0; i < wmi.Length; i++)
            {
                Assert.Equal(wmi[i].DesignedCapacity, ioctl[i].DesignedCapacity);
                Assert.Equal(wmi[i].SerialNumber, ioctl[i].SerialNumber);
                Assert.Equal(wmi[i].ManufactureName, ioctl[i].ManufactureName);
            }
        }

        // ── 對應 WMI: Get-CimInstance -Namespace ROOT\WMI -ClassName "BatteryCycleCount" ──
        // CycleCount
        [Fact]
        public void Test_BatteryCycleCount()
        {
            using CimSession session = CimSession.Create(null);
            var wmi = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryCycleCount")
                .Select(x => (uint)x.CimInstanceProperties["CycleCount"].Value)
                .ToArray();

            var ioctl = BatteryReport.GetAll()
                .Select(x => x.CycleCount)
                .ToArray();

            Assert.Equal(wmi.Length, ioctl.Length);
            for (int i = 0; i < wmi.Length; i++)
            {
                Assert.Equal(wmi[i], ioctl[i]);
            }
        }

        // ── 對應 WMI: Get-CimInstance -Namespace ROOT\WMI -ClassName "BatteryFullChargedCapacity" ──
        // FullChargedCapacity
        [Fact]
        public void Test_BatteryFullChargedCapacity()
        {
            using CimSession session = CimSession.Create(null);
            var wmi = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryFullChargedCapacity")
                .Select(x => (uint)x.CimInstanceProperties["FullChargedCapacity"].Value)
                .ToArray();

            var ioctl = BatteryReport.GetAll()
                .Select(x => x.FullChargedCapacity)
                .ToArray();

            Assert.Equal(wmi.Length, ioctl.Length);
            for (int i = 0; i < wmi.Length; i++)
            {
                Assert.Equal(wmi[i], ioctl[i]);
            }
        }

        // ── 對應 WMI: Get-CimInstance -Namespace ROOT\WMI -ClassName "BatteryStatus" ──
        // DischargeRate, Discharging, Charging, PowerOnline, Voltage, RemainingCapacity
        [Fact]
        public void Test_BatteryStatus()
        {
            using CimSession session = CimSession.Create(null);
            var wmi = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryStatus WHERE Active = True")
                .Select(x => new
                {
                    Voltage = (uint)x.CimInstanceProperties["Voltage"].Value,
                    DischargeRate = (uint)x.CimInstanceProperties["DischargeRate"].Value,
                    RemainingCapacity = (uint)x.CimInstanceProperties["RemainingCapacity"].Value,
                    Charging = (bool)x.CimInstanceProperties["Charging"].Value,
                    Discharging = (bool)x.CimInstanceProperties["Discharging"].Value,
                    PowerOnline = (bool)x.CimInstanceProperties["PowerOnline"].Value,
                }).ToArray();

            var ioctl = BatteryReport.GetAll()
                .Select(x => new
                {
                    Voltage = x.VoltageMillivolts,
                    DischargeRate = x.DischargeRateMilliwatts,
                    RemainingCapacity = x.RemainingCapacity,
                    //Charging = x.IsCharging,
                    //Discharging = x.IsDischarging,
                    //PowerOnline = x.IsPowerOnline,
                }).ToArray();

            Assert.Equal(wmi.Length, ioctl.Length);
            for (int i = 0; i < wmi.Length; i++)
            {
                Assert.Equal(wmi[i].Voltage, ioctl[i].Voltage);
                Assert.Equal(wmi[i].DischargeRate, ioctl[i].DischargeRate);
                Assert.Equal(wmi[i].RemainingCapacity, ioctl[i].RemainingCapacity);
                //Assert.Equal(wmi[i].Charging, ioctl[i].Charging);
                //Assert.Equal(wmi[i].Discharging, ioctl[i].Discharging);
                //Assert.Equal(wmi[i].PowerOnline, ioctl[i].PowerOnline);
            }
        }

        // ── 對應 WMI: Get-CimInstance -Namespace ROOT\WMI -ClassName "BatteryRuntime" ──
        // EstimatedRuntime (秒)
        [Fact]
        public void Test_BatteryRuntime()
        {
            using CimSession session = CimSession.Create(null);
            var wmi = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryRuntime WHERE Active = True")
                .Select(x => (uint)x.CimInstanceProperties["EstimatedRuntime"].Value)
                .ToArray();

            var ioctl = BatteryReport.GetAll()
                .Select(x => x.EstimatedTimeSeconds)
                .ToArray();

            Assert.Equal(wmi.Length, ioctl.Length);
            for (int i = 0; i < wmi.Length; i++)
            {
                Assert.Equal(wmi[i], ioctl[i]);
            }
        }

        // ── 對應 WMI: Get-CimInstance -ClassName Win32_Battery ──
        // EstimatedChargeRemaining (%)
        [Fact]
        public void Test_Win32_Battery()
        {
            //using CimSession session = CimSession.Create(null);
            //var wmi = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_Battery")
            //    .Select(x => new
            //    {
            //        EstimatedChargeRemaining = (ushort)x.CimInstanceProperties["EstimatedChargeRemaining"].Value,
            //    }).ToArray();

            //var ioctl = BatteryReport.GetAll()
            //    .Select(x => new
            //    {
            //        // WMI 回傳整數 %，IOCTL 計算出的是 double，取整數比對
            //        EstimatedChargeRemaining = (ushort)Math.Round(x.ChargeRemainingPercent),
            //    }).ToArray();

            //Assert.Equal(wmi.Length, ioctl.Length);
            //for (int i = 0; i < wmi.Length; i++)
            //{
            //    // 允許 ±1% 的誤差（取樣時間差）
            //    Assert.InRange(ioctl[i].EstimatedChargeRemaining,
            //        (ushort)Math.Max(0, wmi[i].EstimatedChargeRemaining - 1),
            //        (ushort)(wmi[i].EstimatedChargeRemaining + 1));
            //}
        }

        // ── 對應網頁的衰退計算: 100 - (FullChargedCapacity / DesignedCapacity * 100) ──
        [Fact]
        public void Test_Degradation()
        {
            using CimSession session = CimSession.Create(null);
            var wmi_designed = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryStaticData")
                .Select(x => (uint)x.CimInstanceProperties["DesignedCapacity"].Value).ToArray();
            var wmi_full = session.QueryInstances(@"root\wmi", "WQL", "SELECT * FROM BatteryFullChargedCapacity")
                .Select(x => (uint)x.CimInstanceProperties["FullChargedCapacity"].Value).ToArray();

            var ioctl = BatteryReport.GetAll();

            Assert.Equal(wmi_designed.Length, ioctl.Length);
            for (int i = 0; i < ioctl.Length; i++)
            {
                // 網頁公式: degraded = 100 - (full / designed * 100)
                double wmi_degradation = Math.Round(100.0 - ((double)wmi_full[i] / wmi_designed[i] * 100), 2);
                Assert.Equal(wmi_degradation, ioctl[i].DegradationPercent);
            }
        }

        // ── 完整報表輸出 ──
        [Fact]
        public void Test_FullReport()
        {
            var reports = BatteryReport.GetAll();
            Assert.NotEmpty(reports);
            foreach (var r in reports)
            {
                System.Diagnostics.Trace.WriteLine(r.ToString());
            }
        }
    }
}
