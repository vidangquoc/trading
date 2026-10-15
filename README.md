# Trading

A personal trading-tools project. The first tool is a cTrader Automate C# Forex Setup Scanner designed to reduce continuous chart-watching and make a discretionary price-action process easier to review.

## Current project

**Forex Setup Scanner v0.1** — cTrader custom indicator prototype.

- Market: Forex
- Platform: cTrader
- Language: C# with cTrader Automate API
- Primary chart: M5
- Context timeframes: H1 and H4
- Setup hypothesis: rejection/bounce at support or rejection at resistance
- Entry workflow: calculate an indicative pending-order plan; do not submit orders
- Alerts: live popup notifications and log details, plus historical chart markers/labels
- Execution: manual only; no automatic order placement

## Repository layout

- `ctrader/ForexSetupScanner.cs` — cTrader custom indicator prototype
- `docs/architecture.md` — module boundaries and technical direction
- `docs/strategy-v0.1.md` — first-pass setup rules and limitations
- `docs/roadmap.md` — staged implementation and validation plan
- `pine/forex_setup_scanner_v0_1.pine` — retired TradingView prototype retained in Git history only; no longer the target implementation

## Status

Prototype / research only. The C# code has not yet been compiled in cTrader Automate or validated against historical data. The setup rules are hypotheses, not a demonstrated trading edge. Inspect signals visually and validate historical results with realistic spread, commission, slippage and order-fill assumptions before relying on them. No live orders are placed by this project.

## Quick start

1. Open cTrader and create/open an Automate custom indicator project.
2. Add `ctrader/ForexSetupScanner.cs` to the project.
3. Build in cTrader Automate and resolve any API/compiler differences against the installed cTrader version.
4. Attach the indicator to a Forex **M5** chart.
5. Review H1/H4 pivot reference levels, M5 candidate markers, chart labels and the Automate log.
6. Add the indicator to separate M5 charts for each pair you want to monitor.

Treat every signal as a candidate for manual review, not as a recommendation to trade. Read the documents in `docs/` before changing the setup rules.
