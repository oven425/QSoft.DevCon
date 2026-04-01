using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QSoft.DevCon
{
    /// <summary>
    /// 統整所有透過 IOCTL 取得的電池資訊，提供健康度、衰退率、溫度轉換、剩餘時間等計算屬性。
    /// 對應 WMI: Win32_Battery / BatteryStaticData / BatteryStatus / BatteryFullChargedCapacity / BatteryCycleCount。
    /// </summary>
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
        //  計算屬性 — 剩餘電量
        // ════════════════════════════════════════════════════════

        /// <summary>目前剩餘電量百分比 (%)。對應 Win32_Battery.EstimatedChargeRemaining</summary>
        public double ChargeRemainingPercent =>
            FullChargedCapacity > 0 ? Math.Round((double)RemainingCapacity / FullChargedCapacity * 100, 2) : 0;

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
        //  計算屬性 — 電源狀態旗標
        // ════════════════════════════════════════════════════════

        /// <summary>是否接上 AC 電源。對應 WMI BatteryStatus.PowerOnline</summary>
        public bool IsPowerOnline => PowerState.HasFlag(PowerState.BATTERY_POWER_ON_LINE);

        /// <summary>是否處於臨界狀態</summary>
        public bool IsCritical => PowerState.HasFlag(PowerState.BATTERY_CRITICAL);

        /// <summary>是否正在放電。對應 WMI BatteryStatus.Discharging</summary>
        public bool IsDischarging => PowerState.HasFlag(PowerState.BATTERY_DISCHARGING);

        /// <summary>是否正在充電。對應 WMI BatteryStatus.Charging</summary>
        public bool IsCharging => PowerState.HasFlag(PowerState.BATTERY_CHARGING);

        // ════════════════════════════════════════════════════════
        //  計算屬性 — 溫度
        // ════════════════════════════════════════════════════════

        /// <summary>溫度 (°C)。轉換公式：(tenthKelvin / 10) − 273.15</summary>
        public double TemperatureCelsius =>
            TemperatureTenthKelvin > 0 ? Math.Round(TemperatureTenthKelvin / 10.0 - 273.15, 2) : double.NaN;

        /// <summary>溫度 (°F)</summary>
        public double TemperatureFahrenheit =>
            !double.IsNaN(TemperatureCelsius) ? Math.Round(TemperatureCelsius * 9.0 / 5.0 + 32, 2) : double.NaN;

        // ════════════════════════════════════════════════════════
        //  計算屬性 — 估計剩餘時間
        // ════════════════════════════════════════════════════════

        /// <summary>是否能估計剩餘時間（0xFFFFFFFF 表示不可估計）</summary>
        public bool CanEstimateRunTime => EstimatedTimeSeconds != 0xFFFFFFFF;

        /// <summary>估計剩餘執行時間。無法估計時回傳 null。</summary>
        public TimeSpan? EstimatedRunTime =>
            CanEstimateRunTime ? TimeSpan.FromSeconds(EstimatedTimeSeconds) : null;

        /// <summary>估計剩餘時間 (分鐘)。對應 Win32_Battery.EstimatedRunTime。</summary>
        public double? EstimatedRunTimeMinutes =>
            CanEstimateRunTime ? Math.Round(EstimatedTimeSeconds / 60.0, 1) : null;

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

        private BatteryReport() { }

        /// <summary>
        /// 從已開啟的電池裝置 Handle 取得完整報表。
        /// </summary>
        public static BatteryReport FromHandle(SafeFileHandle handle)
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
                    using var handle = path.devpath.OpenHandle();
                    if (handle != null && !handle.IsInvalid)
                    {
                        list.Add(FromHandle(handle));
                    }
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
            sb.AppendLine($"  Cycle Count         : {CycleCount}");
            sb.AppendLine("╠══ Health ═════════════════════════════════════╣");
            sb.AppendLine($"  Battery Health      : {HealthPercent,7:F2} %");
            sb.AppendLine($"  Battery Degraded    : {DegradationPercent,7:F2} %");
            sb.AppendLine($"  Charge Remaining    : {ChargeRemainingPercent,7:F2} %");
            sb.AppendLine("╠══ Power ══════════════════════════════════════╣");
            sb.AppendLine($"  Voltage             : {VoltageMillivolts,8} mV   ({VoltageVolts:F3} V)");
            sb.AppendLine($"  Rate                : {Rate,8} mW   ({RateWatts:F2} W)");
            sb.AppendLine($"  Discharge Rate      : {DischargeRateMilliwatts,8} mW");
            sb.AppendLine("╠══ State ══════════════════════════════════════╣");
            sb.AppendLine($"  Power Online (AC)   : {IsPowerOnline}");
            sb.AppendLine($"  Charging            : {IsCharging}");
            sb.AppendLine($"  Discharging         : {IsDischarging}");
            sb.AppendLine($"  Critical            : {IsCritical}");
            sb.AppendLine("╠══ Runtime & Temperature ══════════════════════╣");

            if (CanEstimateRunTime)
            {
                var rt = EstimatedRunTime!.Value;
                sb.AppendLine($"  Est. Run Time       : {rt.Hours}h {rt.Minutes}m {rt.Seconds}s  ({EstimatedRunTimeMinutes:F1} min)");
            }
            else
            {
                sb.AppendLine($"  Est. Run Time       : N/A (AC connected)");
            }

            if (!double.IsNaN(TemperatureCelsius))
                sb.AppendLine($"  Temperature         : {TemperatureCelsius}°C / {TemperatureFahrenheit}°F");
            else
                sb.AppendLine($"  Temperature         : N/A");

            sb.AppendLine("╚══════════════════════════════════════════════╝");
            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════
        //  輔助方法
        // ════════════════════════════════════════════════════════

#if NET8_0_OR_GREATER
        private static string ParseChemistry(BufferChemistry chemistry)
        {
            try
            {
                ReadOnlySpan<byte> span = chemistry;
                var str = Encoding.ASCII.GetString(span).TrimEnd('\0');
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
                var str = Encoding.ASCII.GetString(chemistry).TrimEnd('\0');
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