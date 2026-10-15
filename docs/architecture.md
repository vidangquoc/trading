# Forex Setup Scanner — Architecture

## Goal

Reduce the time spent watching Forex charts by scanning for a small, explicitly defined set of price-action setups and alerting only when a candidate is detected. The first release is an indicator and planning aid, not an automated trading system.

## User context and constraints

- The user trades Forex.
- Current discretionary method: price action, support/resistance and candlestick patterns.
- The user's current results are inconsistent overall.
- The user commonly places pending orders.
- Continuous monitoring of the M5 chart is tiring and time-consuming.
- Target platform: TradingView.
- Target language: Pine Script v6.
- Scope: a few Forex pairs, initially one chart/alert per symbol.
- No automatic order placement in the initial release.

## Logical modules

1. **Market data**
   - Use the symbol's TradingView chart data.
   - Run the indicator on M5.
   - Request H1/H4 context through `request.security()`.
   - Only use confirmed higher-timeframe values for signals to reduce repainting risk.

2. **Market context / zone references**
   - Initial prototype: most recent confirmed H1 and H4 pivot highs/lows.
   - Initial zone width: configurable fraction of the corresponding HTF ATR.
   - This is a simple baseline, not a mature support/resistance clustering algorithm.

3. **Setup detection**
   - Long candidate: price touches a support reference and a bullish rejection candle meets configurable wick/body and close-location conditions.
   - Short candidate: price touches a resistance reference and a bearish rejection candle meets corresponding conditions.
   - Signals are evaluated on confirmed M5 bars only.

4. **Entry planner**
   - Calculate indicative entry, stop-loss and take-profit levels from the signal candle and ATR buffer.
   - The levels are suggestions for manual review only.
   - The script does not submit, modify or cancel broker orders.

5. **Alerting**
   - Use Pine's `alert()` and create a TradingView alert with **Any alert() function call**.
   - Include symbol, direction, entry, SL, TP and target R multiple.
   - A cooldown reduces immediate repeated signals but is not a complete per-setup deduplication/expiry lifecycle.

6. **Validation and research**
   - Inspect candidate quality visually.
   - Record signals and outcomes.
   - Convert the rules to a Pine `strategy()` only after the indicator logic is reviewed.
   - Validate with out-of-sample data and realistic transaction costs.

## Initial implementation choice

Start with a single Pine Script v6 indicator on TradingView rather than a separate service or broker API integration. This keeps the first experiment simple and close to the chart where the user makes decisions.

## Known limitations

- Initial S/R detection uses only the latest confirmed pivot high/low per timeframe; it does not merge nearby pivots or score zones by touches/strength.
- The same pivot reference is not guaranteed to be a useful zone in all market regimes.
- The initial rejection rules are heuristic and need review against the user's own discretionary examples.
- The pending-entry calculation is a deterministic retracement of the signal candle, not an optimized or validated execution method.
- The prototype does not manage candidate expiry, invalidation after alert, order fill, open positions or broker-specific spread.
- One indicator instance works on one chart symbol. Monitoring several pairs requires separate charts/alerts; Pine is not a general-purpose unlimited multi-symbol scanner.
- TradingView data feeds and broker execution prices can differ.
- A good historical result is not a guarantee of future profitability.

## Safety boundary

No live order execution is in scope until the setup is specified, tested, and reviewed. Never assume an alert has been filled or that an indicative price is executable.
