# Trading

A personal trading-tools project. The initial work focuses on a TradingView Pine Script v6 Forex Setup Scanner to reduce continuous chart-watching and make a discretionary price-action process testable.

## Current project

**Forex Setup Scanner v0.1** — indicator prototype for TradingView.

- Market: Forex
- Platform: TradingView
- Language: Pine Script v6
- Primary chart: M5
- Context timeframes: H1 and H4
- Setup hypothesis: rejection/bounce at support or rejection at resistance
- Entry workflow: calculate an indicative pending-order plan; do not submit orders
- Alerts: closed-bar candidates
- Execution: manual only; no broker connection or automatic trading

## Repository layout

- `pine/forex_setup_scanner_v0_1.pine` — initial indicator prototype
- `docs/architecture.md` — module boundaries and technical direction
- `docs/strategy-v0.1.md` — explicit first-pass setup rules and limitations
- `docs/roadmap.md` — staged implementation and validation plan

## Status

Prototype / research only. The setup rules are hypotheses, not a demonstrated trading edge. Inspect signals visually, check Pine Editor compilation, and validate historical results including realistic spread, commission, slippage and execution assumptions before relying on them. No live orders are placed by this project.

## Quick start

1. Open a Forex chart in TradingView and set it to **5 minutes**.
2. Open Pine Editor and paste `pine/forex_setup_scanner_v0_1.pine`.
3. Save and add the indicator to the chart.
4. Inspect H1/H4 pivot levels and M5 candidate markers.
5. Create an alert using **Any alert() function call**.
6. Treat every signal as a candidate for manual review, not as a recommendation to trade.

See the documents in `docs/` before changing the setup rules.
