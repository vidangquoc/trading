# Roadmap

## Phase 0 — Capture current decisions

- [x] Target TradingView and Pine Script v6.
- [x] Focus on Forex, M5 execution context, H1/H4 zone references.
- [x] First setup family: rejection/bounce at support and rejection at resistance.
- [x] First release is an indicator with alerts and indicative pending-entry planning.
- [x] No automatic order placement.
- [x] Store the strategy assumptions and known limitations in version-controlled docs.

## Phase 1 — Compile and visually inspect v0.1

- [ ] Paste `pine/forex_setup_scanner_v0_1.pine` into TradingView Pine Editor.
- [ ] Resolve any compiler errors against the live Pine v6 compiler.
- [ ] Inspect H1/H4 pivot lines on several familiar Forex pairs.
- [ ] Compare long/short markers with the user's manual price-action reading.
- [ ] Collect examples of good signals, false positives and missed signals.
- [ ] Confirm behavior after chart reload and at H1/H4 bar transitions.

## Phase 2 — Improve zone and candidate lifecycle

- [ ] Decide whether zones should combine nearby H1/H4 pivots.
- [ ] Add zone touch counts/recency or other explainable strength measures only if useful.
- [ ] Give each candidate a stable identity and track status: new, active, invalidated, expired, or manually dismissed.
- [ ] Define setup expiry and invalidation rules.
- [ ] Prevent duplicate alerts for the same candidate, rather than relying only on a global cooldown.
- [ ] Consider spread filters and session/news filters if supported by reliable data.

## Phase 3 — Make the strategy testable

- [ ] Freeze a version of the setup specification before evaluating performance.
- [ ] Convert the agreed entry/exit rules to a Pine `strategy()` without lookahead bias.
- [ ] Define how limit-order fills are simulated and how unfilled orders expire.
- [ ] Account for commission, spread and slippage as realistically as the platform allows.
- [ ] Report trade count, expectancy in R, profit factor, drawdown and time-in-market.
- [ ] Separate development and out-of-sample periods; avoid tuning solely to past results.

## Phase 4 — Live alert-only trial

- [ ] Run alerts without automatic execution.
- [ ] Log candidate, alert time, price at alert, whether the plan would have filled, and subsequent outcome.
- [ ] Compare real-time behavior with historical expectations.
- [ ] Confirm that alert volume actually reduces screen time.

## Phase 5 — Optional external integration

- [ ] Only consider a webhook receiver or cTrader integration if alert-only results and operational requirements justify it.
- [ ] Keep order execution disabled by default.
- [ ] Add explicit risk limits, idempotency, order-state reconciliation, error handling and a kill switch before any live execution experiment.

## Release principle

Do not add complexity or optimize parameters until the previous phase has produced inspectable evidence. The objective is a repeatable, low-attention workflow, not a larger number of signals.
