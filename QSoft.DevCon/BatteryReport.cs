using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSoft.DevCon
{
    public class BatteryReport
    {
        // ── 靜態資料 (對應 WMI BatteryStaticData) ──
        public string DeviceName { get; private set; } = "";
        public string ManufactureName { get; private set; } = "";
        public string SerialNumber { get; private set; } = "";
        public string UniqueID { get; private set; } = "";
        public string Chemistry { get; private set; } = "";

        // ── 設計規格 (對應 BATTERY_INFORMATION) ──
        /// <summary>原廠設計容量 (mWh)。對應 WMI BatteryStaticData.DesignedCapacity</summary>
        public uint DesignedCapacity { get; private set; }

        /// <summary>目前完全充電容量 (mWh)。對應 WMI BatteryFullChargedCapacity.FullChargedCapacity</summary>
        public uint FullChargedCapacity { get; private set; }

        /// <summary>充放電循環次數。對應 WMI BatteryCycleCount.CycleCount</summary>
        public uint CycleCount { get; private set; }

        public uint DefaultAlert1 { get; private set; }
        public uint DefaultAlert2 { get; private set; }
        public uint CriticalBias { get; private set; }
        public BatteryCapabilities Capabilities { get; private set; }

        /// <summary>0 = 一次性電池, 1 = 可充電電池</summary>
        public byte Technology { get; private set; }

        // ── 即時狀態 (對應 WMI BatteryStatus) ──
        /// <summary>目前電壓 (mV)。對應 WMI BatteryStatus.Voltage</summary>
        public uint VoltageMillivolts { get; private set; }

        /// <summary>充放電速率 (mW)。正值=充電，負值=放電。對應 WMI BatteryStatus.DischargeRate</summary>
        public int Rate { get; private set; }

        /// <summary>目前剩餘容量 (mWh)。對應 WMI BatteryStatus.RemainingCapacity</summary>
        public uint RemainingCapacity { get; private set; }

        /// <summary>電源狀態旗標</summary>
        public PowerState PowerState { get; private set; }

        // ── 估計時間與溫度 ──
        /// <summary>估計剩餘時間 (秒)。0xFFFFFFFF 表示無法估計（已接上 AC）。對應 WMI BatteryRuntime.EstimatedRuntime</summary>
        public uint EstimatedTimeSeconds { get; private set; }

        /// <summary>溫度 (十分之一 Kelvin)</summary>
        public uint TemperatureTenthKelvin { get; private set; }

        // ── 粒度資訊 ──
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

        /// <summary>
        /// 從裝置路徑建立報表。Handle 由內部管理，呼叫端無需負責生命週期。
        /// </summary>
        public static BatteryReport FromDevicePath(string devicePath)
        {
            using var handle = devicePath.OpenHandle();
            if (handle == null || handle.IsInvalid)
                throw new InvalidOperationException($"無法開啟電池裝置：{devicePath}");

            var report = FromHandle(handle);
            report._devicePath = devicePath;
            return report;
        }

        /// <summary>
        /// 從已開啟的電池裝置 Handle 取得完整報表。
        /// <para>注意：Handle 由呼叫端負責管理，Report 不會持有 Handle。</para>
        /// </summary>
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

        /// <summary>
        /// 更新動態資料。Handle 由內部自行開啟與關閉，呼叫端無需管理。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 透過 <see cref="FromHandle"/> 建立的報表沒有裝置路徑，無法自動重新開啟 Handle。
        /// 請改用 <see cref="Update(SafeFileHandle)"/> 或改以 <see cref="FromDevicePath"/> 建立報表。
        /// </exception>
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
                desc = x.As().DeviceDesc(),
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

        // ════════════════════════════════════════════════════════
        //  格式化輸出（類似網頁的 PowerShell 輸出）
        // ════════════════════════════════════════════════════════

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("╔══════════════════════════════════════════════╗");
            sb.AppendLine("║            Battery Report                    ║");
            sb.AppendLine("╠══════════════════════════════════════════════╣");
            sb.AppendLine($"  Device Name         : {DeviceName}");
            sb.AppendLine($"  Manufacturer        : {ManufactureName}");
            sb.AppendLine($"  Serial Number       : {SerialNumber}");
            sb.AppendLine($"  Unique ID           : {UniqueID}");
            sb.AppendLine($"  Chemistry           : {Chemistry}");
            sb.AppendLine($"  Type                : {(IsRechargeable ? "Rechargeable" : "Non-rechargeable")}");
            sb.AppendLine("╠══ Capacity ═══════════════════════════════════╣");
            sb.AppendLine($"  Designed Capacity   : {DesignedCapacity,8} mWh  ({DesignedCapacityWh:F2} Wh)");
            sb.AppendLine($"  Full Charge Cap.    : {FullChargedCapacity,8} mWh  ({FullChargedCapacityWh:F2} Wh)");
            sb.AppendLine($"  Remaining Capacity  : {RemainingCapacity,8} mWh  ({RemainingCapacityWh:F2} Wh)");
            sb.AppendLine($"  Capacity Loss       : {CapacityLossMilliwattHours,8} mWh");
            sb.AppendLine($"  Cycle Count         : {CycleCount,8}");
            sb.AppendLine("╠══ Health ═════════════════════════════════════╣");
            sb.AppendLine($"  Battery Health      : {HealthPercent,7:F2} %");
            sb.AppendLine($"  Battery Degraded    : {DegradationPercent,7:F2} %");
            sb.AppendLine("╠══ Power ══════════════════════════════════════╣");
            sb.AppendLine($"  Voltage             : {VoltageMillivolts,8} mV   ({VoltageVolts:F3} V)");
            sb.AppendLine($"  Rate                : {Rate,8} mW   ({RateWatts:F2} W)");
            sb.AppendLine($"  Discharge Rate      : {DischargeRateMilliwatts,8} mW");
            sb.AppendLine($"  PowerState          : {PowerState, 8}");
            sb.AppendLine("╚══════════════════════════════════════════════╝");
            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════
        //  輔助方法
        // ════════════════════════════════════════════════════════

        /// <summary>實際執行動態資料更新的核心邏輯。</summary>
        private void UpdateCore(SafeFileHandle handle)
        {
            var src = handle.BatteryTag();

            // ── 即時狀態 ──
            var status = src.BatteryStatus();
            VoltageMillivolts = status.Voltage;
            Rate = status.Rate;
            RemainingCapacity = status.Capacity;
            PowerState = status.PowerState;

            // ── 估計時間與溫度 ──
            EstimatedTimeSeconds = src.BatteryEstimatedTime();
            TemperatureTenthKelvin = src.BatteryTemperature();

            // ── 半動態（充電後可能變動）──
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