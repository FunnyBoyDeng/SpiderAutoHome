# SpiderAutoHome

简体中文 | [English](README.md)

[![Verify SpiderAutoHome](https://github.com/FunnyBoyDeng/SpiderAutoHome/actions/workflows/verify-spiderautohome.yml/badge.svg)](https://github.com/FunnyBoyDeng/SpiderAutoHome/actions/workflows/verify-spiderautohome.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)

SpiderAutoHome 是一个 C# 教学仓库，通过 [DotnetSpider](https://github.com/dotnetcore/DotnetSpider) 展示分页表单请求、多阶段详情/API 提取、品牌资源发现三种常见的数据提取流程。

项目源于 2018 年发布的中文实战教程。维护工作于 2026 年恢复，目标是保留原始教学价值，同时将示例迁移到受支持的 .NET 工具链，并记录 DotnetSpider 2.x 到 5.x 的真实迁移过程。

> 三个示例均已升级至 .NET 10 和 DotnetSpider 5.1.7。解析器、请求构造、JSON 模型、URL 规范化与文件名处理均有离线测试覆盖。第三方网站的当前页面结构与 API 契约可能已与历史示例不同。

## 当前状态

| 项目 | 用途 | 运行环境 | 状态 |
| --- | --- | --- | --- |
| `SpiderAutoHome` | 店铺/列表分页解析 | .NET 10 / DotnetSpider 5.1.7 | 可构建，已有离线解析测试 |
| `SpiderAutoHome.Tests` | 确定性的解析测试 | .NET 10 / xUnit v3 | 已接入 GitHub Actions |
| `SpiderAutoSkuData` | 多阶段商品详情/API 提取 | .NET 10 / DotnetSpider 5.1.7 | 可构建，已有类型化 JSON 与后续请求测试 |
| `SpiderAutoLogo` | 品牌/Logo 提取与显式启用的资源下载 | .NET 10 / DotnetSpider 5.1.7 | 可构建，已有解析、URL 与文件名测试 |

现代化路线图见 [GitHub issue #3](https://github.com/FunnyBoyDeng/SpiderAutoHome/issues/3)。

## 快速开始

需要安装 [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) 与 Git。

```bash
git clone https://github.com/FunnyBoyDeng/SpiderAutoHome.git
cd SpiderAutoHome
dotnet restore SpiderAutoHome.Modern.slnx --locked-mode
dotnet build SpiderAutoHome.Modern.slnx --configuration Release --no-restore
dotnet run --project SpiderAutoHome.Tests/SpiderAutoHome.Tests.csproj --configuration Release --no-build
```

测试只使用内存中的 HTML/JSON 样例，不访问第三方网站。`SpiderAutoHome.Modern.slnx` 与 Visual Studio `SpiderAutoHome.sln` 均可构建三个示例和测试项目。

运行主示例：

```bash
dotnet run --project SpiderAutoHome/SpiderAutoHome.csproj
```

运行多阶段 SKU 示例时，可以使用历史默认详情页，也可以通过环境变量提供其他获准访问的详情页：

```bash
SPIDERAUTOHOME_SKU_URL=https://example.com/permitted-detail \
  dotnet run --project SpiderAutoSkuData/SpiderAutoSkuData.csproj
```

Logo 下载默认关闭；启用后采用原子写入，并限制单个文件最大 10 MiB。还可以显式指定输出目录：

```bash
SPIDERAUTOHOME_DOWNLOAD_LOGOS=true \
SPIDERAUTOHOME_LOGO_DIR=./img \
  dotnet run --project SpiderAutoLogo/SpiderAutoLogo.csproj
```

以上命令会访问配置的数据源。运行前请确认目标、站点现行规则、请求频率及使用目的；学习和贡献时优先运行离线测试。

## 原始教程

1. [汽车之家店铺数据抓取 DotnetSpider 实战（一）](https://www.cnblogs.com/FunnyBoy/p/8453338.html) — `SpiderAutoHome`
2. [汽车之家店铺商品详情数据抓取 DotnetSpider 实战（二）](https://www.cnblogs.com/FunnyBoy/p/9029937.html) — `SpiderAutoSkuData`
3. [汽车之家汽车品牌 Logo 信息抓取 DotnetSpider 实战（三）](https://www.cnblogs.com/FunnyBoy/p/9097377.html) — `SpiderAutoLogo`

另有原始[演示视频](https://www.bilibili.com/video/av24022630/)。

## 维护与贡献

- [开发指南](DEVELOPMENT.md)
- [贡献指南](CONTRIBUTING.md)
- [安全策略](SECURITY.md)
- [维护者与响应目标](MAINTAINERS.md)
- [发布流程](RELEASING.md)
- [维护自动化计划](docs/MAINTAINER_AUTOMATION.md)
- [DotnetSpider 兼容性映射](DOTNETSPIDER_COMPATIBILITY.md)

CI 会在 Windows 与 Linux 上执行还原、构建和离线测试，同时检查文档内部链接，并把编译器及 NuGet 漏洞警告视为构建错误；CodeQL 扫描受支持的 C# 项目，Dependabot 持续检查 NuGet 与 GitHub Actions 依赖。

欢迎提交 issue 与 pull request，尤其是离线测试、文档、依赖维护、数据源结构变化报告和教程可访问性改进。

## 使用边界

本仓库用于讲解数据提取架构，并不授予对任何第三方系统的访问权限。仅处理你有权访问的数据与系统，并遵守适用条款、robots 规则、隐私要求、速率限制和数据使用限制。示例与测试数据中不得加入凭据、个人数据、访问控制绕过或反滥用规避逻辑。

## 许可证

[MIT](LICENSE) © 2018–2026 FunnyBoyDeng。
