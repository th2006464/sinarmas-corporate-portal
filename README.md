# 金光集团企业门户 (Sinar Mas Corporate Portal)

ASP.NET Core 8.0 Razor Pages 企业门户网站，为金光集团（金光中国食品）提供完整的前台展示和后台新闻管理系统。

> 在线地址：`https://www.garchina.com/portalwebsite`

---

## 技术架构

### 整体架构

```
┌─────────────────────────────────────────┐
│              Browser / Client            │
├─────────────────────────────────────────┤
│         IIS (AspNetCoreModuleV2)        │
│            web.config → Portal.exe       │
├─────────────────────────────────────────┤
│        ASP.NET Core 8.0 (Kestrel)       │
│  ┌─────────────┐  ┌──────────────────┐  │
│  │ Razor Pages  │  │ Cookie Auth      │  │
│  │ (18 pages)   │  │ (Admin only)     │  │
│  ├─────────────┤  ├──────────────────┤  │
│  │ NewsService  │  │ Static Files     │  │
│  │ (JSON CRUD)  │  │ (wwwroot/)       │  │
│  └─────────────┘  └──────────────────┘  │
├─────────────────────────────────────────┤
│           data/news.json (117篇)         │
└─────────────────────────────────────────┘
```

### 技术栈

| 层面 | 技术 | 版本 | 来源 |
|------|------|------|------|
| 运行时 | .NET | 8.0 | 服务器 Hosting Bundle |
| Web 框架 | ASP.NET Core Razor Pages | 8.0 | NuGet |
| CSS 框架 | Bootstrap | 5.3.8 | bootcdn CDN |
| JS 库 | jQuery | 3.7.1 | 本地 |
| 富文本编辑器 | TinyMCE | 6.8.3 | bootcdn CDN |
| 认证 | Cookie Authentication | — | ASP.NET Core 内置 |
| 数据存储 | JSON 文件 | — | data/news.json (~1MB) |
| Web 服务器 | IIS + Kestrel | — | inprocess 模式 |
| 部署模式 | 框架依赖 | — | 服务器提供 .NET 8 运行时 |

### 设计理念

- **CSS 变量体系**：统一品牌色 `#c41230`，全局字体、阴影、圆角变量
- **现代字体栈**：`system-ui, -apple-system, "Segoe UI", "PingFang SC", "Microsoft YaHei"`
- **卡片式布局**：产品、新闻采用 `border-radius + box-shadow` 卡片设计
- **Flexbox 双栏**：首页视频 + 公司介绍左右分栏，`align-items: flex-start` 各列独立高度
- **响应式**：桌面 50/50 → 平板 → 手机纵向堆叠
- **`<base href="~/"/>`**：主布局统一路径基准，支持任意子目录部署

---

## 项目结构

```
企业门户/
├── Program.cs                    # 应用入口（含 UsePathBase + 浏览器自动打开）
├── Portal.csproj                 # 项目配置（排除 release/publish/bin/obj）
├── appsettings.json              # 配置（含 Admin 账号密码）
├── web.config                    # IIS 配置（含 PATH_BASE 环境变量）
├── publish.ps1                   # 一键发布脚本
├── start_dev.ps1                 # 本地启动 release
├── start_dev_src.ps1             # 本地启动源代码
├── cleanup.bat                   # 清理残留进程
│
├── data/
│   └── news.json                 # 117 篇新闻（完整 HTML 内容 + 本地图片路径）
│
├── Pages/                        # Razor Pages (18 个页面)
│   ├── _ViewImports.cshtml       # Razor 全局导入
│   ├── _ViewStart.cshtml         # 默认 Layout
│   │
│   ├── Shared/
│   │   └── _Layout.cshtml        # 主布局（<base href="~/"/> + 导航/页脚/浮动栏）
│   │
│   ├── Index.cshtml(.cs)         # 首页（Banner轮播 + 双栏布局 + 新闻列表）
│   ├── About.cshtml(.cs)         # 关于我们（5频道：intro/gar/history/culture/csr）
│   ├── Products.cshtml(.cs)      # 产品领域（魔法士/三鲜伊面/椰语清澜）
│   ├── Contact.cshtml(.cs)       # 联系我们（联系方式 + 在线留言表单）
│   ├── Join.cshtml(.cs)          # 加入我们（人才理念/人才招聘）
│   ├── Search.cshtml(.cs)        # 搜索结果
│   │
│   ├── News/
│   │   ├── List.cshtml(.cs)      # 新闻列表（分页10条/页，摘要200字符）
│   │   └── Detail.cshtml(.cs)    # 新闻详情（图片居中，支持视频）
│   │
│   └── Admin/
│       ├── Index.cshtml(.cs)     # /admin → 重定向 /admin/login
│       ├── Login.cshtml(.cs)     # 登录页（Cookie认证，IgnoreAntiforgeryToken）
│       ├── Logout.cshtml(.cs)    # 退出登录
│       ├── Dashboard.cshtml(.cs) # 新闻管理仪表盘（列表/删除，Url.Content路径）
│       └── NewsForm.cshtml(.cs)  # 发布/编辑新闻（TinyMCE CDN，base64图片上传）
│
├── Services/
│   └── NewsService.cs            # 新闻数据服务（读/写/搜索/分页/CRUD）
│
├── PortalStarter/                # 启动器项目（独立 exe，设PORT+开浏览器）
│   ├── Program.cs
│   └── PortalStarter.csproj
│
└── wwwroot/                      # 静态资源
    ├── css/
    │   └── style.css             # 自定义样式（CSS变量 + Flex/Grid + 响应式）
    ├── images/                   # 451 张图片（新闻图片 + 网站素材）
    └── js/
        ├── jquery-3.7.1.min.js
        └── tinymce/              # 本地 TinyMCE（仅 skins + langs，实际从 CDN 加载）
```

---

## 页面路由表

| 路由 | 页面 | 认证 | 说明 |
|------|------|------|------|
| `/` | 首页 | 否 | Banner + 视频/介绍双栏 + 新闻 |
| `/about?channel=intro` | 关于我们 | 否 | 5个频道切换 |
| `/products` | 产品领域 | 否 | 3个产品卡片 |
| `/news/list?page=1` | 新闻列表 | 否 | 10条/页，12页 |
| `/news/detail/{id}` | 新闻详情 | 否 | 图文混排 + 视频 |
| `/contact` | 联系我们 | 否 | 联系方式 + 留言表单 |
| `/join` | 加入我们 | 否 | 人才理念/招聘 |
| `/search?key=xxx` | 搜索 | 否 | 标题+内容搜索 |
| `/admin` | 后台入口 | 否 | → 302 重定向 /admin/login |
| `/admin/login` | 后台登录 | 否 | admin/admin123 |
| `/admin/logout` | 退出登录 | 是 | 清除 Cookie |
| `/admin/dashboard` | 新闻管理 | 是 | 列表/删除 |
| `/admin/newsform` | 发布新闻 | 是 | TinyMCE 编辑器 |
| `/admin/newsform/{id}` | 编辑新闻 | 是 | 预填内容 |

---

## CSS 设计系统

### 变量定义 (`:root`)

```css
--color-primary: #c41230;        /* 品牌红 */
--color-primary-hover: #a00e26;
--font-family: system-ui, ...;   /* 现代字体栈 */
--font-size-base: 1rem;          /* 16px */
--shadow-md: 0 4px 12px rgba(0,0,0,0.08);
--radius: 8px;
--transition: 0.2s ease;
```

### 布局策略

| 区域 | 技术 | 说明 |
|------|------|------|
| 首页双栏 | `display: flex; flex: 1 1 50%` | 视频左/介绍右，移动端堆叠 |
| 产品卡片 | `display: grid; auto-fill, minmax(300px, 1fr)` | 自适应列数 |
| 页脚链接 | `display: grid; repeat(5, 1fr)` | 5列均匀分布 |
| 新闻图片 | `margin: 20px auto; display: block` | 居中 |
| 浮动栏 | `position: fixed; right: 16px` | 电话/二维码/回顶 |

---

## 本地运行

```powershell
# 源码运行
dotnet run
# 访问 http://localhost:5000

# Release 运行
.\start_dev.ps1

# 清理残留进程（调试后卡顿时使用）
.\cleanup.bat
```

---

## 部署

### 发布

```powershell
.\publish.ps1
```

生成 `release/` 文件夹（框架依赖部署，约 6MB + 图片）。

### IIS 配置

1. 服务器安装 [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)
2. IIS 创建站点，物理路径指向 `release/`
3. 应用程序池 → .NET CLR 版本 → **无托管代码**
4. `web.config` 已配置 `AspNetCoreModuleV2` + `hostingModel="inprocess"`

### 子目录部署

`web.config` 内置 `PATH_BASE` 环境变量：

```xml
<environmentVariable name="PATH_BASE" value="/portalwebsite" />
```

`_Layout.cshtml` 中 `<base href="~/"/>` 自动解析为当前应用基路径。所有资源使用相对路径。

部署在根目录时，删除 `PATH_BASE` 环境变量即可。

---

## 后台管理

| 项目 | 信息 |
|------|------|
| 地址 | `/admin/login` |
| 账号 | `admin` |
| 密码 | `admin123` |
| 修改密码 | 编辑 `appsettings.json` → `Admin:Password` |

功能：
- 发布/编辑/删除新闻
- TinyMCE 6.8.3 富文本编辑器（CDN 加载，含全部插件）
- 图片粘贴/拖拽上传（base64 内嵌）
- 前台预览链接

---

## 踩坑记录

### 1. 子目录部署 → 全部 404

**现象**：部署到 `/portalwebsite` 后页面完全无法加载。

**根因**：所有资源使用绝对路径 `/css/style.css`，浏览器向域名根目录请求。

**解决**：
- `<base href="~/"/>` + 全部改为相对路径
- `news.json` 398 个图片路径 `/images/` → `images/`
- `Program.cs` 添加 `UsePathBase(PATH_BASE)` 确保服务端跳转也正确

### 2. 进程残留 → 系统卡死

**现象**：关闭调试窗口后电脑卡死，复制粘贴冻结 30 秒。

**根因**：`Thread.Sleep` 阻塞 + `Process.Start` 未释放，dotnet 进程残留 280MB×N。

**解决**：`Thread.Sleep` → `Task.Delay`，`using var proc`，`CancelKeyPress` 干净退出。

### 3. TinyMCE 编辑器空白

**现象**：后台编辑页"内容*"下方一大片空白，没有编辑框。

**根因**：`wwwroot/js/tinymce/` 只有 `skins/` 和 `langs/`，缺少 `plugins/` 目录。TinyMCE 加载 link/image/table 等插件全部 404。

**解决**：改用 CDN 加载 TinyMCE（bootcdn），自带全部插件文件。

### 4. 后台页面路径错误（双层 Admin）

**现象**：编辑链接变成 `/Admin/Admin/NewsForm/565`。

**根因**：后台页面 `Layout = null` 无 `<base>`，相对路径从当前 URL 解析。Dashboard 中 `admin/newsform` → `/Admin/admin/newsform`。

**解决**：所有后台链接改用 `@Url.Content("~/Admin/...")` 生成绝对路径。

### 5. 页脚 Grid 只显示一列

**根因**：CSS Grid 应用于 `.foot_link`，但直接子元素只有 `<ul>`，`<li>` 是孙子元素。

**解决**：选择器改为 `.footer .foot_link ul`，`grid-template-columns: repeat(5, 1fr)`。

### 6. release/ 嵌套递归

**现象**：`release/release/release/` 多层嵌套。

**根因**：`dotnet publish` 把项目根目录的 `release/` 子目录一并打包进 `publish/`。

**解决**：`Portal.csproj` 添加 `<Content Remove="release\**;publish\**;bin\**;obj\**" />`。

### 7. 新闻摘要含 HTML 标签

**现象**：摘要显示 `<p>...` 等标签字符。

**解决**：渲染前 `Regex.Replace(summary, "<[^>]+>", "")` 剥离标签 + `HtmlDecode`，截断提升至 200 字符。

---

## 数据来源

117 篇新闻 + 451 张图片从金光集团官网爬取：
- 列表页：`http://www.sinarmas-agri.com.cn/media_newslist.aspx?page=1~20`
- 详情页：`http://www.sinarmas-agri.com.cn/media_newsinfo.aspx?id={id}`
- 图片路径已替换为本地相对路径
- 日期范围：2020-07-01 ~ 2026-05-13
