# Roadmap

## Phase 0 — Capture current decisions

- [x] Switch from TradingView/Pine Script to cTrader Automate and C#.
- [x] Focus on Forex, M5 setup detection, H1/H4 zone references.
- [x] First setup family: bounce/rejection at support and rejection at resistance.
- [x] First release is an indicator with candidate markers and indicative pending-entry planning.
- [x] No automatic order placement.
- [x] Store assumptions, constraints and limitations in version-controlled docs.

## Phase 1 — Compile and visually inspect cTrader v0.1

- [ ] Import ctrader/ForexSetupScanner.cs into a cTrader Automate custom indicator project.
- [ ] Build using the installed cTrader version and fix compiler/API issues.
- [ ] Attach to M5 charts for several familiar Forex pairs.
- [ ] Inspect H1/H4 pivot levels and M5 candidate markers.
- [ ] Compare long/short candidates with the user's manual price-action reading.
- [ ] Check HTF bar alignment, historical lookahead/repainting, and chart reload behavior.
- [ ] Verify log output and the exact notification options available in the installed cTrader version.

## Phase 2 — Improve zones and candidate lifecycle

- [ ] Decide whether nearby H1/H4 pivots should be merged into zones.
- [ ] Add explainable zone recency/touch-strength measures only if useful.
- [ ] Give each candidate a stable identity and status: new, active, invalidated, expired, or dismissed.
- [ ] Define setup expiry and invalidation rules.
- [ ] Prevent duplicate alerts for the same candidate rather than relying only on cooldown.
- [ ] Consider spread, trading-session and news filters if supported by reliable data.

## Phase 3 — Make the strategy testable

- [ ] Freeze a version of the setup specification before evaluating performance.
- [ ] Implement a separate cTrader cBot/backtest only after the indicator logic is reviewed.
- [ ] Define how limit-order fills are simulated and how unfilled orders expire.
- [ ] Account for spread, commission and slippage as realistically as possible.
- [ ] Report trade count, expectancy in R, profit factor, drawdown and time-in-market.
- [ ] Separate development and out-of-sample periods; avoid tuning only to past results.

## Phase 4 — Live alert-only trial

- [ ] Run the indicator without automatic execution.
- [ ] Log candidate time, price at alert, whether the plan would have filled, and subsequent outcome.
- [ ] Compare live behavior with historical expectations.
- [ ] Confirm the workflow actually reduces screen time.

## Phase 5 — Optional automation, only if explicitly chosen later

- [ ] Consider a separate cBot only if alert-only evidence and operational needs justify it.
- [ ] Keep order execution disabled until explicitly enabled and reviewed.
- [ ] Add hard risk limits, idempotency, order-state reconciliation, error handling and a kill switch before any live execution experiment.

## Release principle

Do not add complexity or optimize parameters until the previous phase has produced inspectable evidence. The objective is a repeatable, low-attention workflow, not more signals.
