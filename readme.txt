# Binance 模拟盘自动化交易示例

该仓库包含一个使用 .NET 9 和 [Binance.Net](https://github.com/JKorf/Binance.Net) SDK 编写的示例控制台程序，用于演示如何基于简单的均线策略在本地执行币安现货的回测/模拟交易。

## 主要特性

- 使用 `Microsoft.Extensions.Hosting` 构建主机，自动装配日志、配置与依赖项。
- 封装 `Binance.Net` REST 客户端，按需从币安（或 TestNet）下载历史 K 线数据。
- 实现简单的短期 / 长期移动平均交叉策略。
- 根据策略信号执行买入/卖出并计入手续费，输出最终资产情况。

## 快速开始

1. **安装 .NET 9 SDK**

   本示例使用 .NET 9 预览版。请确保已安装与 `net9.0` 兼容的 SDK，并在仓库根目录下执行以下命令：

   ```bash
   dotnet restore src/BinanceSimulator/BinanceSimulator.csproj
   dotnet run --project src/BinanceSimulator/BinanceSimulator.csproj
   ```

2. **配置 API**

   将 `src/BinanceSimulator/appsettings.json` 中的 `apiKey` 与 `apiSecret` 替换为您在币安（或币安测试网）的 API Key。如果只想获取公共行情数据，可将其留空。

3. **调整策略**

   - `shortWindow` / `longWindow`：移动平均周期（单位：根 K 线）。
   - `threshold`：买卖触发的最小均线差值（相对长均线）。
   - `initialQuoteBalance`：初始资金（计价币）。

4. **注意事项**

   - Binance.Net 默认访问正式环境。若要连接测试网，将 `testNet` 设为 `true` 并在币安测试网申请 API Key。
   - 本程序仅作教学示例，不构成投资建议。实际交易前请务必在模拟环境充分测试。

## 后续扩展建议

- 接入 WebSocket 实时数据，替换为实时模拟或纸上交易。
- 支持多币对、多策略组合与风险控制。
- 将策略参数与交易记录持久化到数据库，支持可视化报表。
