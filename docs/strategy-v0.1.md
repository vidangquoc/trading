# Strategy Specification — v0.1

## Status

Research hypothesis only. These rules are an initial operational definition for the cTrader scanner, not a claim of positive expectancy.

## Market and timeframe

- Market: Forex.
- Platform: cTrader.
- Chart timeframe: M5.
- Context timeframes: H1 and H4.
- Initial use: one M5 chart and indicator instance per Forex symbol.
- First release: visual candidates and plan details in the cTrader Automate log; no order execution.

## Higher-timeframe reference levels

For H1 and H4 independently:

1. Detect pivot lows and pivot highs using configurable left/right confirmation bars.
2. Use only pivots confirmed by completed higher-timeframe candles.
3. Set zone half-width to a configurable fraction of that timeframe's ATR.
4. Keep the latest confirmed pivot low as support and latest confirmed pivot high as resistance.
5. If both H1 and H4 zones are touched, select the reference nearest to the signal candle close.

This is intentionally simple. Later revisions may merge nearby pivots or score zones, but only after visual review.

## Long candidate: rejection at support

All conditions must hold:

1. Chart timeframe is M5.
2. The signal candle has closed.
3. Its low touches the selected support zone without exceeding the outer boundary.
4. It closes above the support reference.
5. It is bullish (close > open).
6. Lower wick is at least wickBodyRatio × body size.
7. Close location within the candle range is at least closeLocation, where 0 is the low and 1 is the high.
8. Proposed entry is below the close and risk distance is positive.
9. The signal cooldown has elapsed.

## Short candidate: rejection at resistance

All conditions must hold:

1. Chart timeframe is M5.
2. The signal candle has closed.
3. Its high touches the selected resistance zone without exceeding the outer boundary.
4. It closes below the resistance reference.
5. It is bearish (close < open).
6. Upper wick is at least wickBodyRatio × body size.
7. Close location within the candle range is at most 1 - closeLocation.
8. Proposed entry is above the close and risk distance is positive.
9. The signal cooldown has elapsed.

## Indicative entry / stop / target

Long candidate:

- Entry = low + (close - low) × entryRetracement.
- Stop = signal low − M5 ATR × stopBufferAtr.
- Risk = entry − stop.
- Target = entry + risk × rewardRisk.

Short candidate:

- Entry = high − (high - close) × entryRetracement.
- Stop = signal high + M5 ATR × stopBufferAtr.
- Risk = stop − entry.
- Target = entry − risk × rewardRisk.

These are prototype assumptions, not proof that a limit order will fill or that the target is reachable. Spread, slippage, broker rules and delay between signal and entry must be considered.

## Default parameters

- H1 and H4 context.
- Pivot left/right: 3 / 3 bars.
- HTF ATR period: 14.
- Zone half-width: 0.15 × HTF ATR.
- Minimum rejection wick/body: 1.5.
- Close-location threshold: 0.65.
- Entry retracement fraction: 0.50.
- Signal cooldown: 6 M5 bars.
- M5 ATR period: 14.
- Stop buffer: 0.10 × M5 ATR.
- Target multiple: 2R.

Defaults are starting values for inspection, not recommended trading parameters. Avoid tuning only to maximize one historical sample.

## Signal handling

- Emit candidate signals only after an M5 candle closes.
- Include symbol, direction, indicative entry, SL, TP and target R multiple in log output.
- Draw a direction marker and optional plan label on the chart.
- Cooldown is a simple per-chart mechanism, not robust setup identity or lifecycle management.
- The initial version does not expire or invalidate a plan after it has been reported.
- No orders are sent to the broker.

## Validation checklist

- Compile the indicator in cTrader Automate.
- Compare historical markers with manually selected examples.
- Check that H1/H4 values do not repaint or use future bars when reviewing historical M5 candles.
- Check behavior around H1/H4 bar transitions and chart reloads.
- Test several Forex pairs and market regimes.
- Use out-of-sample periods.
- Include realistic spread, commission, slippage and limit-order fill assumptions.
- Track expectancy in R, profit factor, maximum drawdown, trade count and fill assumptions.
- Keep a record of every rule change to reduce hindsight bias.
