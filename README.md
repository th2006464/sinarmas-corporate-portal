# 金光集团企业门户 (Sinar Mas Corporate Portal)

ASP.NET Core 8.0 Razor Pages 企业门户网站，包含前台展示、新闻管理后台，支持子目录部署。

## 技术栈

| 层面 | 技术 |
|------|------|
| 框架 | ASP.NET Core 8.0 (Razor Pages) |
| 前端 | Bootstrap 5.3.8 (bootcdn), jQuery 3.7.1 |
| 富文本 | TinyMCE 6.8.3 |
| 认证 | ASP.NET Core Cookie Authentication |
| 数据 | 基于文件的 JSON 存储 (117 篇新闻) |
| 部署 | IIS + 框架依赖部署 (.NET 8 Hosting Bundle) |

## 项目结构

```
企业门户/
├── Program.cs                    # 应用入口
├── Portal.csproj                 # 项目配置
├── appsettings.json              # 配置（含后台账号密码）
├── web.config                    # IIS 配置
├── publish.ps1                   # 一键发布脚本
├── start_dev.ps1                 # 本地开发启动
├── data/news.json                # 117 篇新闻数据 (~1MB)
├── Pages/                        # Razor Pages
│   ├── Shared/_Layout.cshtml     # 主布局
│   ├── Index.cshtml              # 首页（轮播+双栏+新闻）
│   ├── About.cshtml              # 关于我们（5频道）
│   ├── Products.cshtml           # 产品领域
│   ├── Contact.cshtml            # 联系我们（含留言表单）
│   ├── Join.cshtml               # 加入我们
│   ├── Search.cshtml             # 搜索结果
│   ├── News/                     # 新闻列表/详情
│   └── Admin/                    # 后台管理
│       ├── Login.cshtml          # 登录页
│       ├── Dashboard.cshtml      # 新闻管理仪表盘
│       ├── NewsForm.cshtml       # 发布/编辑新闻 (TinyMCE)
│       └── Logout.cshtml         # 退出登录
├── Services/NewsService.cs       # 新闻数据服务（CRUD）
└── wwwroot/                      # 静态资源
    ├── css/style.css             # 自定义样式（CSS 变量体系）
    ├── images/                   # 451 张图片
    └── js/                       # jQuery + TinyMCE
```

## 本地运行

```powershell
# 方式一：源码运行
dotnet run

# 方式二：使用已发布的 release
.\start_dev.ps1
```

访问 `http://localhost:5000`

## 部署到 IIS

### 前提条件
服务器需安装 [.NET 8 Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)

### 发布步骤
```powershell
.\publish.ps1
```

将 `release/` 文件夹复制到服务器，IIS 中创建站点指向该目录。应用程序池选择"无托管代码"。

### 子目录部署（如 `/portalwebsite`）

`web.config` 已内置 `PATH_BASE` 环境变量：
```xml
<environmentVariable name="PATH_BASE" value="/portalwebsite" />
```

如果部署在根目录，删除这一行即可。

## 后台管理

| 项目 | 信息 |
|------|------|
| 地址 | `/admin/login` |
| 默认账号 | `admin` |
| 默认密码 | `admin123` |
| 修改密码 | 编辑 `appsettings.json` 中 `Admin:Password` |

功能：发布/编辑/删除新闻，TinyMCE 富文本编辑器，支持图文混排。

## 踩坑记录

### 1. 子目录部署路径问题

**现象**：部署在 `https://www.garchina.com/portalwebsite` 后，所有 CSS/JS/图片返回 404，页面完全无法加载。

**原因**：页面中所有资源路径使用绝对路径（如 `/css/style.css`），浏览器向域名根目录 `garchina.com/css/style.css` 请求，而非 `garchina.com/portalwebsite/css/style.css`。

**解决方案**：
1. `_Layout.cshtml` 添加 `<base href="~/"/>`，Razor 自动解析为应用基路径
2. 所有 `src`/`href` 去掉前导 `/`，改为相对路径（`css/style.css` 而非 `/css/style.css`）
3. `news.json` 中 398 个图片路径同步改为相对路径
4. `Program.cs` 添加 `UsePathBase` 读取 `PATH_BASE` 环境变量，确保 Cookie 认证跳转也包含子目录前缀

### 2. 进程残留导致系统卡死

**现象**：调试关闭窗口后，电脑明显变卡，复制粘贴操作卡死 30 秒。

**原因**：`Thread.Sleep(800)` 阻塞启动回调线程，`Process.Start` 打开浏览器后进程对象未释放。每次关闭窗口后 dotnet 进程残留（占用 ~280MB 内存），多次累积耗尽系统资源。

**解决方案**：
1. `Thread.Sleep` 替换为 `await Task.Delay`，不阻塞线程池
2. `Process.Start` 用 `using` 确保释放
3. 添加 `Console.CancelKeyPress` 实现 Ctrl+C 干净退出
4. 浏览器进程不等待，启动后立即释放引用

### 3. 首页布局

**现象**：视频区和公司介绍区上下堆叠，页面过长。

**解决**：Flexbox 双栏布局 (`display: flex; flex: 1 1 50%`)，左侧视频、右侧介绍，移动端回退为纵向堆叠。

### 4. 页脚 Grid 不生效

**现象**：页脚链接全部排成一列。

**原因**：CSS Grid 应用在 `.foot_link` 上，但其直接子元素只有一个 `<ul>`，`<li>` 是孙子元素。

**解决**：Grid 选择器改为 `.footer .foot_link ul`，5 列均匀分布。

## 数据来源

新闻内容（117 篇）和图片（451 张）抓取自金光集团官网 `http://www.sinarmas-agri.com.cn`，已转换为本地静态数据。
