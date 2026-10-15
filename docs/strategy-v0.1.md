# Strategy Specification — v0.1

## Status

Research hypothesis only. These rules are an initial operational definition for a scanner, not a claim of positive expectancy.

## Market and timeframe

- Market: Forex.
- Chart timeframe: M5.
- Context timeframes: H1 and H4.
- Initial use: one chart and one alert per Forex symbol.

## Higher-timeframe reference levels

For each context timeframe:

1. Detect pivot lows and pivot highs using configurable left/right confirmation bars.
2. Use only pivots that are confirmed on a completed higher-timeframe candle.
3. Set a zone half-width equal to a configurable fraction of that timeframe's ATR.
4. Keep the latest confirmed pivot low as the support reference and latest confirmed pivot high as the resistance reference.

The prototype chooses the touched H1 or H4 reference nearest to the current close when both are touched. The zone algorithm is deliberately simple and should be revisited after visual review.

## Long candidate: rejection at support

All conditions must hold:

1. Chart timeframe is M5.
2. Current M5 candle has closed.
3. The candle's low touches the configured support zone without exceeding the zone's outer boundary.
4. The candle closes above the support reference.
5. Candle is bullish (`close > open`).
6. Lower wick is at least `wickBodyRatio × body size`.
7. Close location within the candle range is at least `closeLocation`, where 0 is the low and 1 is the high.
8. The proposed entry is below the close and risk distance is positive.
9. The signal cooldown has elapsed.

## Short candidate: rejection at resistance

All conditions must hold:

1. Chart timeframe is M5.
2. Current M5 candle has closed.
3. The candle's high touches the configured resistance zone without exceeding the zone's outer boundary.
4. The candle closes below the resistance reference.
5. Candle is bearish (`close < open`).
6. Upper wick is at least `wickBodyRatio × body size`.
7. Close location within the candle range is at most `1 - closeLocation`.
8. The proposed entry is above the close and risk distance is positive.
9. The signal cooldown has elapsed.

## Indicative entry / stop / target

Long candidate:
- Entry = low + (close - low) × entryRetrace.
- Stop = signal low − M5 ATR × stopBufferAtr.
- Risk = entry − stop.
- Target = entry + risk × rewardRisk.

Short candidate:
- Entry = high − (high - close) × entryRetrace.
- Stop = signal high + M5 ATR × stopBufferAtr.
- Risk = stop − entry.
- Target = entry − risk × rewardRisk.

These formulas are prototype assumptions only. They do not prove that the limit order is likely to fill or that the target is reachable. Spread, slippage, broker rules and the time between signal and potential entry must be considered before using a plan.

## Default parameters in v0.1

- H1 context: 60 minutes.
- H4 context: 240 minutes.
- Pivot left/right: 3 / 3 bars.
- HTF ATR length: 14.
- Zone half-width: 0.15 × HTF ATR.
- Minimum rejection wick/body: 1.5.
- Close-location threshold: 0.65.
- Entry retracement fraction: 0.50.
- Signal cooldown: 6 M5 bars.
- M5 ATR length: 14.
- Stop buffer: 0.10 × M5 ATR.
- Target multiple: 2R.

These values are starting defaults for inspection, not recommended trading parameters. Avoid tuning them only to maximize one historical sample.

## Signal handling

- Emit candidate signals only after M5 bar close.
- Alert includes symbol, side, indicative entry, SL, TP and target R multiple.
- Cooldown is a simple global per-chart cooldown, not a robust setup identity or lifecycle.
- The initial version does not automatically cancel a plan after expiry or after price invalidates the setup.

## Validation checklist

Before treating the setup as useful:

- Compare the scanner's historical markers with manually selected examples.
- Identify false positives and missed examples.
- Verify higher-timeframe values do not repaint after reload.
- Check different pairs and market regimes.
- Use out-of-sample periods.
- Include realistic spread, commission and slippage.
- Track expectancy in R, profit factor, maximum drawdown, number of trades and fill assumptions.
- Keep a record of all rule changes to reduce hindsight bias.
