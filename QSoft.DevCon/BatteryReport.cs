using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSoft.DevCon.Battery
{
    public class BatteryReport
    {
        public string DeviceName { get; private set; } = "";
        public string ManufactureName { get; private set; } = "";
        public string SerialNumber { get; private set; } = "";
        public string UniqueID { get; private set; } = "";
        public string Chemistry { get; private set; } = "";

        public uint DesignedCapacity { get; private set; }
        public uint FullChargedCapacity { get; private set; }
        public uint CycleCount { get; private set; }

        public uint DefaultAlert1 { get; private set; }
        public uint DefaultAlert2 { get; private set; }
        public uint CriticalBias { get; private set; }
        public BatteryCapabilities Capabilities { get; private set; }

        public byte Technology { get; private set; }

        public uint VoltageMillivolts { get; private set; }

        public int Rate { get; private set; }

        public uint RemainingCapacity { get; private set; }

        public PowerState PowerState { get; private set; }

        public uint EstimatedTimeSeconds { get; private set; }

        /// <summary>溫度 (十分之一 Kelvin)</summary>
        public uint TemperatureTenthKelvin { get; private set; }

        public BATTERY_REPORTING_SCALE[] GranularityInformation { get; private set; } = [];

        // ════════════════════════════════════════════════════════
        //  計算屬性 — 健康度與衰退
        // ════════════════════════════════════════════════════════

        /// <summary>電池健康度 (%)：FullChargedCapacity / DesignedCapacity × 100</summary>
        public double HealthPercent =>
            DesignedCapacity > 0 ? Math.Round((double)FullChargedCapacity / DesignedCapacity * 100, 2) : 0;

        /// <summary>電池衰退百分比 (%)：100 − HealthPercent</summary>
        public double DegradationPercent =>
            DesignedCapacity > 0 ? Math.Round(100.0 - HealthPercent, 2) : 0;

        /// <summary>容量損失 (mWh)：DesignedCapacity − FullChargedCapacity</summary>
        public uint CapacityLossMilliwattHours =>
            DesignedCapacity > FullChargedCapacity ? DesignedCapacity - FullChargedCapacity : 0;

        // ════════════════════════════════════════════════════════
        //  計算屬性 — 電壓 / 功率
        // ════════════════════════════════════════════════════════

        /// <summary>電壓 (V)</summary>
        public double VoltageVolts => VoltageMillivolts / 1000.0;

        /// <summary>充放電速率 (W)</summary>
        public double RateWatts => Rate / 1000.0;

        /// <summary>放電速率 (mW)，正在放電時回傳正值，否則 0</summary>
        public uint DischargeRateMilliwatts => Rate < 0 ? (uint)(-Rate) : 0;


        // ════════════════════════════════════════════════════════
        //  計算屬性 — 單位轉換 (Wh)
        // ════════════════════════════════════════════════════════

        /// <summary>設計容量 (Wh)</summary>
        public double DesignedCapacityWh => DesignedCapacity / 1000.0;

        /// <summary>完全充電容量 (Wh)</summary>
        public double FullChargedCapacityWh => FullChargedCapacity / 1000.0;

        /// <summary>剩餘容量 (Wh)</summary>
        public double RemainingCapacityWh => RemainingCapacity / 1000.0;

        /// <summary>是否為可充電電池</summary>
        public bool IsRechargeable => Technology == 1;


        // ════════════════════════════════════════════════════════
        //  工廠方法
        // ════════════════════════════════════════════════════════

        // 儲存裝置路徑，供 Update() 內部自行開關 handle
        private string _devicePath = "";

        private BatteryReport() { }
        public static BatteryReport FromDevicePath(string devicePath)
        {
            using var handle = devicePath.OpenHandle();
            if (handle == null || handle.IsInvalid)
                throw new InvalidOperationException($"無法開啟電池裝置：{devicePath}");

            var report = FromHandle(handle);
            report._devicePath = devicePath;
            return report;
        }

        static BatteryReport FromHandle(SafeFileHandle handle)
        {
            var (h, tag) = handle.BatteryTag();
            var src = (h, tag);

            var info = src.BatteryInfo();
            var status = src.BatteryStatus();

            return new BatteryReport
            {
                DeviceName = src.BatteryDeviceName(),
                ManufactureName = src.BatteryManufactureName(),
                SerialNumber = src.BatterySerialNumber(),
                UniqueID = src.BatteryUniqueID(),
                Chemistry = ParseChemistry(info.Chemistry),

                DesignedCapacity = info.DesignedCapacity,
                FullChargedCapacity = info.FullChargedCapacity,
                CycleCount = info.CycleCount,
                DefaultAlert1 = info.DefaultAlert1,
                DefaultAlert2 = info.DefaultAlert2,
                CriticalBias = info.CriticalBias,
                Capabilities = info.Capabilities,
                Technology = info.Technology,

                VoltageMillivolts = status.Voltage,
                Rate = status.Rate,
                RemainingCapacity = status.Capacity,
                PowerState = status.PowerState,

                EstimatedTimeSeconds = src.BatteryEstimatedTime(),
                TemperatureTenthKelvin = src.BatteryTemperature(),
                GranularityInformation = src.BatteryGranularityInformation()
            };
        }

        public void Update()
        {
            if (string.IsNullOrEmpty(_devicePath))
                throw new InvalidOperationException(
                    "此報表沒有裝置路徑。請使用 FromDevicePath() 建立報表，或改呼叫 Update(SafeFileHandle)。");

            using var handle = _devicePath.OpenHandle();
            if (handle == null || handle.IsInvalid)
                throw new InvalidOperationException($"無法開啟電池裝置：{_devicePath}");

            UpdateCore(handle);
        }

        /// <summary>
        /// 列舉系統中所有電池裝置並分別產生報表。
        /// </summary>
        public static BatteryReport[] GetAll()
        {
            var list = new List<BatteryReport>();
            var guid = "Battery".GetClassGuids().FirstOrDefault();
            var batterys = guid.DevicesFromInterface().Select(x => new
            {
                devpath = x.DevicePath(),
            });
            foreach (var path in batterys)
            {
                try
                {
                    list.Add(FromDevicePath(path.devpath));
                }
                catch
                {
                    // 個別電池讀取失敗不影響其他電池
                }
            }
            return [.. list];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔═════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║            Battery Report                                   ║");
            sb.AppendLine("╠═════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"  Device Name         : {DeviceName}");
            sb.AppendLine($"  Manufacturer        : {ManufactureName}");
            sb.AppendLine($"  Serial Number       : {SerialNumber}");
            sb.AppendLine($"  Unique ID           : {UniqueID}");
            sb.AppendLine($"  Chemistry           : {Chemistry}");
            sb.AppendLine($"  Type                : {(IsRechargeable ? "Rechargeable" : "Non-rechargeable")}");
            sb.AppendLine("╠══ Capacity ══════════════════════════════════════════════════╣");
            sb.AppendLine($"  Designed Capacity   : {DesignedCapacity,8} mWh  ({DesignedCapacityWh:F2} Wh)");
            sb.AppendLine($"  Full Charge Cap.    : {FullChargedCapacity,8} mWh  ({FullChargedCapacityWh:F2} Wh)");
            sb.AppendLine($"  Remaining Capacity  : {RemainingCapacity,8} mWh  ({RemainingCapacityWh:F2} Wh)");
            sb.AppendLine($"  Capacity Loss       : {CapacityLossMilliwattHours,8} mWh");
            sb.AppendLine($"  Cycle Count         : {CycleCount,8}");
            sb.AppendLine("╠══ Health ════════════════════════════════════════════════════╣");
            sb.AppendLine($"  Battery Health      : {HealthPercent,7:F2} %");
            sb.AppendLine($"  Battery Degraded    : {DegradationPercent,7:F2} %");
            sb.AppendLine("╠══ Power ═════════════════════════════════════════════════════╣");
            sb.AppendLine($"  Voltage             : {VoltageMillivolts,8} mV   ({VoltageVolts:F3} V)");
            sb.AppendLine($"  Rate                : {Rate,8} mW   ({RateWatts:F2} W)");
            sb.AppendLine($"  Discharge Rate      : {DischargeRateMilliwatts,8} mW");
            sb.AppendLine($"  PowerState          : {PowerState, 8}");
            sb.AppendLine("╚═════════════════════════════════════════════════════════════╝");
            return sb.ToString();
        }

        private void UpdateCore(SafeFileHandle handle)
        {
            var src = handle.BatteryTag();

            var status = src.BatteryStatus();
            VoltageMillivolts = status.Voltage;
            Rate = status.Rate;
            RemainingCapacity = status.Capacity;
            PowerState = status.PowerState;

            EstimatedTimeSeconds = src.BatteryEstimatedTime();
            TemperatureTenthKelvin = src.BatteryTemperature();

            var info = src.BatteryInfo();
            FullChargedCapacity = info.FullChargedCapacity;
            CycleCount = info.CycleCount;
        }

#if NET8_0_OR_GREATER
        private static string ParseChemistry(BufferChemistry chemistry)
        {
            try
            {
                var str = Encoding.ASCII.GetString(chemistry);
                return string.IsNullOrWhiteSpace(str) ? "Unknown" : str;
            }
            catch
            {
                return "Unknown";
            }
        }
#else
        private static string ParseChemistry(byte[] chemistry)
        {
            try
            {
                var str = Encoding.ASCII.GetString(chemistry);
                return string.IsNullOrWhiteSpace(str) ? "Unknown" : str;
            }
            catch
            {
                return "Unknown";
            }
        }
#endif
    }
}