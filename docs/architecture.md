# Forex Setup Scanner — Architecture

## Goal

Reduce time spent watching Forex charts by detecting a small, explicit set of price-action candidates and presenting a pending-order plan for manual review. The first release is a cTrader custom indicator, not an automated trading system.

## User context and constraints

- Market: Forex.
- Trading approach: price action, support/resistance and candlestick patterns.
- Results have been inconsistent; the scanner is a research aid, not a claim of profitability.
- Pending orders are commonly used.
- Continuous M5 monitoring is tiring and time-consuming.
- Platform: cTrader.
- Implementation: C# using cTrader Automate API.
- Main signal timeframe: M5.
- Context timeframes: H1 and H4.
- Scope: a few Forex pairs, one M5 chart/indicator instance per symbol initially.
- No automatic order placement in the initial release.

## Logical modules

1. **Market data**
   - Use cTrader bars for the active symbol and separate H1/H4 series.
   - Evaluate the last completed M5 candle, not its still-forming candle.
   - Use only completed HTF bars for zone/pivot context and ATR calculations.
   - Check historical-time alignment carefully to avoid lookahead/future leakage.

2. **Market context / zone references**
   - Baseline: latest confirmed H1 and H4 pivot highs/lows.
   - Zone half-width: configurable fraction of the corresponding timeframe's ATR.
   - If both timeframe levels are touched, select the level nearest the signal candle close.
   - This is a simple baseline, not a mature zone clustering/scoring model.

3. **Setup detection**
   - Long candidate: touch support zone, close above support, bullish candle, sufficient lower wick and close near the candle high.
   - Short candidate: touch resistance zone, close below resistance, bearish candle, sufficient upper wick and close near the candle low.
   - Only evaluate closed M5 bars.

4. **Entry planner**
   - Calculate indicative limit-entry, stop-loss and take-profit levels from the signal candle and ATR buffer.
   - Present values in chart labels and the cTrader Automate log.
   - Never submit, modify or cancel a broker order in v0.1.

5. **Alerting / display**
   - Draw candidate arrows and optional entry/SL/TP labels.
   - Print candidate details to the Automate log.
   - Verify available notification mechanisms and sound/popup behavior in the installed cTrader version before claiming push alerts are supported.
   - Prevent immediate repeated signals with a simple cooldown; this is not a complete candidate lifecycle.

6. **Validation**
   - Build and compile in the installed cTrader Automate environment.
   - Inspect signals visually on several familiar Forex pairs.
   - Compare markers with manually selected examples.
   - Later, implement a separate cBot/backtest only after indicator rules are reviewed and frozen.
   - Validate out-of-sample with realistic spread, commission, slippage and limit-order fill assumptions.

## Implementation choice

Use a cTrader custom indicator in C# first. This matches the user's alert-and-review workflow and avoids enabling trading automation prematurely. Attach an instance to each desired symbol's M5 chart.

## Known limitations

- C# prototype has not yet been compiled in cTrader Automate; API/compiler compatibility must be checked in the user's installed version.
- Pivot detection uses the latest confirmed pivot high/low per timeframe; nearby pivots are not merged and zones are not scored by touch count or strength.
- The zone-width and candle-rejection rules are heuristics that need review against the user's own examples.
- The pending-entry calculation is a deterministic retracement of the signal candle, not a validated execution method.
- Candidate expiry, invalidation after alert, order-fill simulation and broker-specific spread filters are not implemented.
- Monitoring multiple symbols initially requires separate M5 charts and indicator instances.
- Data availability and bar alignment must be checked on the chosen broker's cTrader feed.
- Historical performance, even if positive, does not guarantee future profitability.

## Safety boundary

No live order execution is in scope. Do not turn this indicator into an order-placing cBot until the strategy is specified, tested, and separately approved. Never assume an alert means a limit order was filled or that an indicative price is executable.
