using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ForexSetupScanner : Indicator
    {
        [Parameter("H1 Pivot Left/Right", DefaultValue = 3, MinValue = 1, Group = "HTF Zones")]
        public int PivotBars { get; set; }

        [Parameter("HTF ATR Period", DefaultValue = 14, MinValue = 1, Group = "HTF Zones")]
        public int HtfAtrPeriod { get; set; }

        [Parameter("Zone Half-Width (HTF ATR)", DefaultValue = 0.15, MinValue = 0.01, Step = 0.01, Group = "HTF Zones")]
        public double ZoneAtrMultiplier { get; set; }

        [Parameter("Minimum Wick / Body", DefaultValue = 1.5, MinValue = 0.1, Step = 0.1, Group = "M5 Setup")]
        public double WickBodyRatio { get; set; }

        [Parameter("Close Location Threshold", DefaultValue = 0.65, MinValue = 0.5, MaxValue = 0.95, Step = 0.05, Group = "M5 Setup")]
        public double CloseLocation { get; set; }

        [Parameter("Entry Retracement", DefaultValue = 0.5, MinValue = 0.1, MaxValue = 0.9, Step = 0.05, Group = "Trade Plan")]
        public double EntryRetracement { get; set; }

        [Parameter("M5 ATR Period", DefaultValue = 14, MinValue = 1, Group = "Trade Plan")]
        public int M5AtrPeriod { get; set; }

        [Parameter("SL Buffer (M5 ATR)", DefaultValue = 0.1, MinValue = 0, Step = 0.05, Group = "Trade Plan")]
        public double StopBufferAtr { get; set; }

        [Parameter("Target R Multiple", DefaultValue = 2.0, MinValue = 0.5, Step = 0.25, Group = "Trade Plan")]
        public double RewardRisk { get; set; }

        [Parameter("Cooldown Bars", DefaultValue = 6, MinValue = 0, Group = "Alerts")]
        public int CooldownBars { get; set; }

        [Parameter("Show Plan Labels", DefaultValue = true, Group = "Display")]
        public bool ShowPlanLabels { get; set; }

        [Parameter("Show Popup Alerts", DefaultValue = true, Group = "Alerts")]
        public bool ShowPopupAlerts { get; set; }

        [Output("H1 Support", LineColor = "SeaGreen", Thickness = 1, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries H1Support { get; set; }

        [Output("H1 Resistance", LineColor = "IndianRed", Thickness = 1, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries H1Resistance { get; set; }

        [Output("H4 Support", LineColor = "Teal", Thickness = 2, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries H4Support { get; set; }

        [Output("H4 Resistance", LineColor = "Orange", Thickness = 2, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries H4Resistance { get; set; }

        private Bars _h1;
        private Bars _h4;
        private int _lastProcessedClosedIndex = -1;
        private int _lastSignalIndex = -1000000;
        private double _lastH1Support = double.NaN;
        private double _lastH1Resistance = double.NaN;
        private double _lastH4Support = double.NaN;
        private double _lastH4Resistance = double.NaN;

        protected override void Initialize()
        {
            _h1 = MarketData.GetBars(TimeFrame.Hour, SymbolName);
            _h4 = MarketData.GetBars(TimeFrame.Hour4, SymbolName);
            Print("Forex Setup Scanner initialized for {0}. Use an M5 chart. Indicator only; it does not place orders.", SymbolName);
        }

        public override void Calculate(int index)
        {
            if (index < 2)
                return;

            // Historical indices are closed candles. On the live bar, evaluate
            // the previous candle so the still-forming candle never triggers.
            int closed = index == Bars.Count - 1 ? index - 1 : index;
            if (closed < 1)
                return;

            UpdateHigherTimeframeLevels(closed);

            if (TimeFrame != cAlgo.API.TimeFrame.Minute5)
            {
                if (index == Bars.Count - 1)
                    Print("Forex Setup Scanner: switch {0} to M5 for signals.", SymbolName);
                return;
            }

            if (closed <= _lastProcessedClosedIndex)
                return;

            _lastProcessedClosedIndex = closed;
            EvaluateClosedBar(closed, index == Bars.Count - 1);
        }

        private void UpdateHigherTimeframeLevels(int chartIndex)
        {
            DateTime chartTime = Bars.OpenTimes[chartIndex];
            _lastH1Support = LatestConfirmedPivot(_h1, chartTime, false);
            _lastH1Resistance = LatestConfirmedPivot(_h1, chartTime, true);
            _lastH4Support = LatestConfirmedPivot(_h4, chartTime, false);
            _lastH4Resistance = LatestConfirmedPivot(_h4, chartTime, true);

            H1Support[chartIndex] = _lastH1Support;
            H1Resistance[chartIndex] = _lastH1Resistance;
            H4Support[chartIndex] = _lastH4Support;
            H4Resistance[chartIndex] = _lastH4Resistance;
        }

        private double LatestConfirmedPivot(Bars source, DateTime chartTime, bool highPivot)
        {
            if (source == null || source.Count < PivotBars * 2 + 2)
                return double.NaN;

            int timeIndex = source.OpenTimes.GetIndexByTime(chartTime);
            if (timeIndex < 0)
                return double.NaN;

            // Do not read the still-forming HTF candle.
            int lastClosed = Math.Min(timeIndex - 1, source.Count - 1);
            int lastCandidate = lastClosed - PivotBars;
            if (lastCandidate < PivotBars)
                return double.NaN;

            for (int i = lastCandidate; i >= PivotBars; i--)
            {
                bool pivot = true;
                double value = highPivot ? source.HighPrices[i] : source.LowPrices[i];

                for (int j = i - PivotBars; j <= i + PivotBars; j++)
                {
                    if (j == i)
                        continue;

                    if (highPivot && source.HighPrices[j] >= value)
                    {
                        pivot = false;
                        break;
                    }

                    if (!highPivot && source.LowPrices[j] <= value)
                    {
                        pivot = false;
                        break;
                    }
                }

                if (pivot)
                    return value;
            }

            return double.NaN;
        }

        private double AtrAtIndex(Bars source, int period, int index)
        {
            if (source == null || index < period)
                return double.NaN;

            double sum = 0;
            int start = index - period + 1;
            for (int i = start; i <= index; i++)
            {
                double previousClose = source.ClosePrices[i - 1];
                double tr = Math.Max(source.HighPrices[i] - source.LowPrices[i],
                    Math.Max(Math.Abs(source.HighPrices[i] - previousClose),
                             Math.Abs(source.LowPrices[i] - previousClose)));
                sum += tr;
            }

            return sum / period;
        }

        private double AtrAtTime(Bars source, int period, DateTime chartTime)
        {
            if (source == null)
                return double.NaN;

            int timeIndex = source.OpenTimes.GetIndexByTime(chartTime);
            if (timeIndex < 0)
                return double.NaN;

            // Align historical signals to the last completed HTF bar at that time.
            int index = timeIndex - 1;
            if (index < period)
                return double.NaN;

            double sum = 0;
            int start = index - period + 1;
            for (int i = start; i <= index; i++)
            {
                double previousClose = source.ClosePrices[i - 1];
                double tr = Math.Max(source.HighPrices[i] - source.LowPrices[i],
                    Math.Max(Math.Abs(source.HighPrices[i] - previousClose),
                             Math.Abs(source.LowPrices[i] - previousClose)));
                sum += tr;
            }

            return sum / period;
        }

        private void EvaluateClosedBar(int i, bool allowLiveNotification)
        {
            double open = Bars.OpenPrices[i];
            double high = Bars.HighPrices[i];
            double low = Bars.LowPrices[i];
            double close = Bars.ClosePrices[i];
            double range = high - low;
            if (range <= 0)
                return;

            double body = Math.Abs(close - open);
            double safeBody = Math.Max(body, Symbol.PipSize * 0.1);
            double lowerWick = Math.Min(open, close) - low;
            double upperWick = high - Math.Max(open, close);
            double closePosition = (close - low) / range;

            DateTime signalTime = Bars.OpenTimes[i];
            double h1Atr = AtrAtTime(_h1, HtfAtrPeriod, signalTime);
            double h4Atr = AtrAtTime(_h4, HtfAtrPeriod, signalTime);
            double h1Width = h1Atr * ZoneAtrMultiplier;
            double h4Width = h4Atr * ZoneAtrMultiplier;

            bool touchH1Support = IsFinite(_lastH1Support) && IsFinite(h1Width) && low <= _lastH1Support + h1Width && low >= _lastH1Support - h1Width;
            bool touchH4Support = IsFinite(_lastH4Support) && IsFinite(h4Width) && low <= _lastH4Support + h4Width && low >= _lastH4Support - h4Width;
            bool touchH1Resistance = IsFinite(_lastH1Resistance) && IsFinite(h1Width) && high >= _lastH1Resistance - h1Width && high <= _lastH1Resistance + h1Width;
            bool touchH4Resistance = IsFinite(_lastH4Resistance) && IsFinite(h4Width) && high >= _lastH4Resistance - h4Width && high <= _lastH4Resistance + h4Width;

            double support = SelectTouchedLevel(touchH1Support, _lastH1Support, touchH4Support, _lastH4Support, close);
            double resistance = SelectTouchedLevel(touchH1Resistance, _lastH1Resistance, touchH4Resistance, _lastH4Resistance, close);

            bool bullishRejection = close > open && lowerWick >= safeBody * WickBodyRatio && closePosition >= CloseLocation;
            bool bearishRejection = close < open && upperWick >= safeBody * WickBodyRatio && closePosition <= 1.0 - CloseLocation;
            bool longSetup = IsFinite(support) && close > support && bullishRejection;
            bool shortSetup = IsFinite(resistance) && close < resistance && bearishRejection;

            if (i - _lastSignalIndex <= CooldownBars)
                return;

            double m5Atr = AtrAtIndex(Bars, M5AtrPeriod, i);
            if (!IsFinite(m5Atr))
                return;

            if (longSetup)
            {
                double entry = low + (close - low) * EntryRetracement;
                double sl = low - m5Atr * StopBufferAtr;
                double risk = entry - sl;
                double tp = entry + risk * RewardRisk;
                if (risk > Symbol.TickSize && entry < close)
                    EmitSignal(i, true, entry, sl, tp, allowLiveNotification);
            }
            else if (shortSetup)
            {
                double entry = high - (high - close) * EntryRetracement;
                double sl = high + m5Atr * StopBufferAtr;
                double risk = sl - entry;
                double tp = entry - risk * RewardRisk;
                if (risk > Symbol.TickSize && entry > close)
                    EmitSignal(i, false, entry, sl, tp, allowLiveNotification);
            }
        }

        private double SelectTouchedLevel(bool touchedA, double levelA, bool touchedB, double levelB, double close)
        {
            if (touchedA && touchedB)
                return Math.Abs(close - levelA) <= Math.Abs(close - levelB) ? levelA : levelB;
            if (touchedA)
                return levelA;
            if (touchedB)
                return levelB;
            return double.NaN;
        }

        private void EmitSignal(int i, bool isLong, double entry, double sl, double tp, bool allowLiveNotification)
        {
            _lastSignalIndex = i;
            string side = isLong ? "BUY LIMIT CANDIDATE" : "SELL LIMIT CANDIDATE";
            string message = string.Format("{0} | {1} | M5 | Entry={2} | SL={3} | TP={4} | R:R=1:{5}",
                side, SymbolName, entry.ToString("F" + Symbol.Digits), sl.ToString("F" + Symbol.Digits),
                tp.ToString("F" + Symbol.Digits), RewardRisk.ToString("0.##"));

            // Historical candidates are drawn for review but must not generate
            // popup/log alerts while the indicator is loading old bars.
            if (allowLiveNotification)
            {
                Print(message);
                if (ShowPopupAlerts)
                    Notifications.ShowPopup("Forex Setup Scanner", message, PopupNotificationState.Success);
            }

            string id = "FSS_" + SymbolName + "_" + i;
            Chart.DrawIcon(id + "_icon", isLong ? ChartIconType.UpArrow : ChartIconType.DownArrow,
                i, isLong ? Bars.LowPrices[i] : Bars.HighPrices[i], isLong ? Color.Lime : Color.Red);

            if (ShowPlanLabels)
            {
                string label = side + "\nEntry: " + entry.ToString("F" + Symbol.Digits) +
                    "\nSL: " + sl.ToString("F" + Symbol.Digits) +
                    "\nTP: " + tp.ToString("F" + Symbol.Digits) +
                    "\nR:R = 1:" + RewardRisk.ToString("0.##");
                Chart.DrawText(id + "_label", label, i, isLong ? Bars.LowPrices[i] : Bars.HighPrices[i],
                    isLong ? Color.Lime : Color.Red);
            }
        }

        private bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}